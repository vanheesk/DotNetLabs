using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
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

var app = builder.Build();

app.UseOutputCache();

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
.CacheOutput("PieList");

pieGroup.MapGet("/{id:int:min(1)}", async Task<Results<Ok<PieDto>, NotFound>> (int id, PieShopDbContext db) =>
{
    var pie = await PieQueries.GetById(db, id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(new PieDto(pie.PieId, pie.Name, pie.ShortDescription, pie.Price, pie.IsPieOfTheWeek, pie.Category.Name));
})
.WithName("Pies_GetById")
.CacheOutput(p => p.Tag("pies"));

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
.WithName("Pies_Create");

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
.WithName("Pies_Update");

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
.WithName("Pies_Delete");

app.Run();
