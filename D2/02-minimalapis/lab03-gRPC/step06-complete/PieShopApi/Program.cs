using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PieShop.Grpc;
using Polly;
using Polly.Registry;
using PieShopApi;
using PieShopApi.Data;
using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();

builder.Services.AddSingleton(sp =>
    new SlowQueryInterceptor(
        sp.GetRequiredService<ILogger<SlowQueryInterceptor>>(),
        TimeSpan.FromMilliseconds(100)));

builder.Services.AddDbContext<PieShopDbContext>((sp, options) =>
    options.UseSqlite("Data Source=pieshop.db")
           .AddInterceptors(sp.GetRequiredService<SlowQueryInterceptor>()));

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("PieList", policy =>
        policy.Expire(TimeSpan.FromMinutes(2))
              .SetVaryByQuery("filter", "orderBy", "afterId", "pageSize")
              .Tag("pies"));
});

builder.Services.AddResiliencePipeline("db-pipeline", pipelineBuilder =>
{
    pipelineBuilder
        .AddRetry(new Polly.Retry.RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(200),
            BackoffType = DelayBackoffType.Exponential,
            ShouldHandle = new PredicateBuilder().Handle<DbUpdateException>()
        })
        .AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30),
            MinimumThroughput = 5,
            BreakDuration = TimeSpan.FromSeconds(15),
            ShouldHandle = new PredicateBuilder().Handle<DbUpdateException>()
        })
        .AddTimeout(TimeSpan.FromSeconds(5));
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("writes", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromMinutes(1);
    });

    options.AddSlidingWindowLimiter("reads", limiter =>
    {
        limiter.PermitLimit = 60;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.SegmentsPerWindow = 6;
    });
});

builder.Services.AddGrpcClient<PieCatalogService.PieCatalogServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:5001");
})
.AddStandardResilienceHandler();

var app = builder.Build();

app.UseOutputCache();
app.UseRateLimiter();

// --- Direct DB endpoints ---
var pieGroup = app.MapGroup("/pies");

pieGroup.MapGet("/", async ([AsParameters] PieQuery query, PieShopDbContext db) =>
{
    IQueryable<PieEntity> pies = db.Pies.Include(p => p.Category);

    if (!string.IsNullOrEmpty(query.Filter))
        pies = pies.Where(p => p.Name.Contains(query.Filter));

    pies = query.OrderBy?.ToLower() switch
    {
        "name" => pies.OrderBy(p => p.Name),
        "price" => pies.OrderBy(p => p.Price),
        "price,desc" => pies.OrderByDescending(p => p.Price),
        _ => pies.OrderBy(p => p.PieId)
    };

    if (query.AfterId.HasValue)
        pies = pies.Where(p => p.PieId > query.AfterId.Value);

    var results = await pies
        .Take(query.PageSize)
        .Select(p => new PieDto(p.PieId, p.Name, p.ShortDescription, p.Price, p.IsPieOfTheWeek, p.Category.Name))
        .ToListAsync();

    return TypedResults.Ok(results);
})
.WithName("Pies_GetAll")
.CacheOutput("PieList")
.RequireRateLimiting("reads");

pieGroup.MapGet("/{id:int:min(1)}", async Task<Results<Ok<PieDto>, NotFound>> (int id, PieShopDbContext db) =>
{
    var pie = await PieQueries.GetById(db, id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(new PieDto(pie.PieId, pie.Name, pie.ShortDescription, pie.Price, pie.IsPieOfTheWeek, pie.Category.Name));
})
.WithName("Pies_GetById")
.CacheOutput(p => p.Tag("pies"))
.RequireRateLimiting("reads");

pieGroup.MapPost("/", async (
    CreatePieRequest request,
    PieShopDbContext db,
    IOutputCacheStore cache,
    ResiliencePipelineProvider<string> resilience,
    CancellationToken ct) =>
{
    var pipeline = resilience.GetPipeline("db-pipeline");

    var entity = new PieEntity
    {
        Name = request.Name,
        ShortDescription = request.ShortDescription,
        Price = request.Price,
        IsPieOfTheWeek = request.IsPieOfTheWeek,
        CategoryId = request.CategoryId
    };

    await pipeline.ExecuteAsync(async token =>
    {
        db.Pies.Add(entity);
        await db.SaveChangesAsync(token);
    }, ct);

    await cache.EvictByTagAsync("pies", ct);
    return TypedResults.CreatedAtRoute(entity, "Pies_GetById", new { id = entity.PieId });
})
.WithName("Pies_Create")
.RequireRateLimiting("writes");

pieGroup.MapPut("/{id:int:min(1)}", async Task<Results<Ok<PieEntity>, NotFound>> (
    int id,
    UpdatePieRequest request,
    PieShopDbContext db,
    IOutputCacheStore cache,
    ResiliencePipelineProvider<string> resilience,
    CancellationToken ct) =>
{
    var entity = await db.Pies.FindAsync(id);
    if (entity is null) return TypedResults.NotFound();

    var pipeline = resilience.GetPipeline("db-pipeline");

    entity.Name = request.Name;
    entity.ShortDescription = request.ShortDescription;
    entity.Price = request.Price;
    entity.IsPieOfTheWeek = request.IsPieOfTheWeek;
    entity.CategoryId = request.CategoryId;

    await pipeline.ExecuteAsync(async token =>
    {
        await db.SaveChangesAsync(token);
    }, ct);

    await cache.EvictByTagAsync("pies", ct);
    return TypedResults.Ok(entity);
})
.WithName("Pies_Update")
.RequireRateLimiting("writes");

pieGroup.MapDelete("/{id:int:min(1)}", async Task<Results<NoContent, NotFound>> (
    int id,
    PieShopDbContext db,
    IOutputCacheStore cache,
    ResiliencePipelineProvider<string> resilience,
    CancellationToken ct) =>
{
    var entity = await db.Pies.FindAsync(id);
    if (entity is null) return TypedResults.NotFound();

    var pipeline = resilience.GetPipeline("db-pipeline");

    await pipeline.ExecuteAsync(async token =>
    {
        db.Pies.Remove(entity);
        await db.SaveChangesAsync(token);
    }, ct);

    await cache.EvictByTagAsync("pies", ct);
    return TypedResults.NoContent();
})
.WithName("Pies_Delete")
.RequireRateLimiting("writes");

// --- gRPC Catalog endpoints ---
var catalogGroup = app.MapGroup("/catalog");

catalogGroup.MapGet("/pies", async (PieCatalogService.PieCatalogServiceClient client, CancellationToken ct) =>
{
    var reply = await client.ListPiesAsync(
        new ListPiesRequest { PageSize = 20 },
        cancellationToken: ct);

    return TypedResults.Ok(reply.Pies.Select(p => new PieDto(
        p.PieId, p.Name, p.ShortDescription, (decimal)p.Price, p.IsPieOfTheWeek, p.CategoryName)));
})
.WithName("Catalog_ListPies");

catalogGroup.MapGet("/pies/{id:int}", async (int id, PieCatalogService.PieCatalogServiceClient client, CancellationToken ct) =>
{
    try
    {
        var reply = await client.GetPieAsync(
            new GetPieRequest { PieId = id },
            cancellationToken: ct);

        return Results.Ok(new PieDto(
            reply.PieId, reply.Name, reply.ShortDescription, (decimal)reply.Price, reply.IsPieOfTheWeek, reply.CategoryName));
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
    {
        return Results.NotFound();
    }
})
.WithName("Catalog_GetPie");

// Deadline-aware endpoint: demonstrates setting a gRPC deadline and handling DeadlineExceeded.
catalogGroup.MapGet("/pies/slow", async (PieCatalogService.PieCatalogServiceClient client, CancellationToken ct) =>
{
    try
    {
        // Set a tight 2-second deadline — if the gRPC service is slow, this will fire.
        var deadline = DateTime.UtcNow.AddSeconds(2);
        var reply = await client.ListPiesAsync(
            new ListPiesRequest { PageSize = 100 },
            deadline: deadline,
            cancellationToken: ct);

        return Results.Ok(reply.Pies.Select(p => new PieDto(
            p.PieId, p.Name, p.ShortDescription, (decimal)p.Price, p.IsPieOfTheWeek, p.CategoryName)));
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
    {
        return Results.Problem(
            title: "Upstream gRPC service took too long",
            detail: ex.Message,
            statusCode: StatusCodes.Status504GatewayTimeout);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(StatusCodes.Status499ClientClosedRequest);
    }
})
.WithName("Catalog_ListPiesSlow");

app.Run();
