# Lab 02: Persistent & Resilient Pie Shop

## Lab Overview

In Lab 01, you built a Pie Shop API with full CRUD, `TypedResults`, query binding, validation, and response customization — all backed by an **in-memory repository**. That is fine for prototyping, but production APIs need:

- **Persistent storage** with Entity Framework Core
- **Caching** to reduce database load
- **Resilience** to handle transient failures gracefully
- **Rate limiting** to protect against abuse

This lab replaces the in-memory repository with EF Core backed by SQLite, then layers on output caching, resilience pipelines, and rate limiting.

> **Estimated time:** 75 minutes

## Learning Objectives

By the end of this lab, you will be able to:

- Configure EF Core with SQLite, Fluent API, migrations, and seed data
- Write composable LINQ queries with filtering, sorting, and keyset pagination
- Use compiled queries and interceptors for performance and observability
- Apply output caching with tag-based invalidation
- Build resilience pipelines with retry, circuit-breaker, and timeout using `Microsoft.Extensions.Resilience`
- Configure fixed-window and sliding-window rate limiting

## Prerequisites

- .NET 10 SDK installed
- **Completed Lab 01** (or use `lab02/start/PieShopApi/` which contains the Lab 01 solution)

> **Starter solutions available.** Each step has a matching starter project in the `lab02/` folder.
> ```shell
> cp -r lab02/step02-complete/PieShopApi .
> cd PieShopApi && dotnet run
> ```

---

## Step 1 — EF Core Setup & Migrations

> **Starting point:** `lab02/start/PieShopApi/`

### 1a — Add NuGet Packages

```shell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### 1b — Create Entity Classes

Replace the `Pie` and `Category` records with proper EF Core entities. Create a new `Data/` folder for data-layer code.

Create `Data/PieEntity.cs`:

```csharp
namespace PieShopApi.Data;

public class PieEntity
{
    public int PieId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsPieOfTheWeek { get; set; }
    public int CategoryId { get; set; }
    public CategoryEntity Category { get; set; } = null!;
}
```

Create `Data/CategoryEntity.cs`:

```csharp
namespace PieShopApi.Data;

public class CategoryEntity
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PieEntity> Pies { get; set; } = [];
}
```

### 1c — Create the DbContext

Create `Data/PieShopDbContext.cs`. Requirements:

- Expose `DbSet<PieEntity>` and `DbSet<CategoryEntity>`
- Accept `DbContextOptions<PieShopDbContext>` in the constructor
- Override `OnModelCreating` to apply configuration from the current assembly

```csharp
using Microsoft.EntityFrameworkCore;

namespace PieShopApi.Data;

public class PieShopDbContext(DbContextOptions<PieShopDbContext> options) : DbContext(options)
{
    public DbSet<PieEntity> Pies => Set<PieEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PieShopDbContext).Assembly);
    }
}
```

### 1d — Define Fluent API Configuration

Create `Data/PieEntityConfiguration.cs`. Use `IEntityTypeConfiguration<PieEntity>` to configure:

- `PieId` as the primary key
- `Name` as required with max length 100
- `Price` with column type `decimal(8,2)`
- A required relationship to `CategoryEntity` via `CategoryId`

Try writing it yourself before checking the hint.

<details>
<summary>Hint: Fluent API configuration</summary>

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PieShopApi.Data;

public class PieEntityConfiguration : IEntityTypeConfiguration<PieEntity>
{
    public void Configure(EntityTypeBuilder<PieEntity> builder)
    {
        builder.HasKey(p => p.PieId);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.ShortDescription).HasMaxLength(200);
        builder.Property(p => p.Price).HasColumnType("decimal(8,2)");
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Pies)
               .HasForeignKey(p => p.CategoryId);
    }
}
```
</details>

Also create `Data/CategoryEntityConfiguration.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PieShopApi.Data;

public class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.HasKey(c => c.CategoryId);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
    }
}
```

### 1e — Seed Data

Create `Data/SeedData.cs` with a static method that seeds initial categories and pies. Apply the seed in the configuration classes using `builder.HasData(...)`.

<details>
<summary>Hint: Seed data approach</summary>

Add to `CategoryEntityConfiguration.Configure`:
```csharp
builder.HasData(
    new CategoryEntity { CategoryId = 1, Name = "Fruit Pies", Description = "Pies made with fresh fruit" },
    new CategoryEntity { CategoryId = 2, Name = "Cheese Cakes", Description = "Creamy cheesecakes" },
    new CategoryEntity { CategoryId = 3, Name = "Seasonal Pies", Description = "Special seasonal favorites" }
);
```

Add to `PieEntityConfiguration.Configure`:
```csharp
builder.HasData(
    new PieEntity { PieId = 1, Name = "Apple Pie", ShortDescription = "Classic apple pie with cinnamon", Price = 12.95m, IsPieOfTheWeek = true, CategoryId = 1 },
    new PieEntity { PieId = 2, Name = "Blueberry Cheesecake", ShortDescription = "Creamy cheesecake with blueberry topping", Price = 14.50m, CategoryId = 2 },
    new PieEntity { PieId = 3, Name = "Cherry Pie", ShortDescription = "Traditional cherry pie with lattice crust", Price = 11.75m, CategoryId = 1 },
    new PieEntity { PieId = 4, Name = "Strawberry Pie", ShortDescription = "Fresh strawberry pie with whipped cream", Price = 13.25m, IsPieOfTheWeek = true, CategoryId = 1 },
    new PieEntity { PieId = 5, Name = "Pumpkin Pie", ShortDescription = "Seasonal pumpkin pie with nutmeg", Price = 10.95m, CategoryId = 3 },
    new PieEntity { PieId = 6, Name = "Pecan Pie", ShortDescription = "Rich pecan pie with caramel", Price = 15.00m, CategoryId = 3 },
    new PieEntity { PieId = 7, Name = "Lemon Meringue", ShortDescription = "Tangy lemon filling with meringue top", Price = 12.50m, IsPieOfTheWeek = true, CategoryId = 1 },
    new PieEntity { PieId = 8, Name = "Chocolate Cream Pie", ShortDescription = "Decadent chocolate cream pie", Price = 13.95m, CategoryId = 2 }
);
```
</details>

### 1f — Register DbContext and Run Migrations

In `Program.cs`, replace the `InMemoryPieRepository` registration with:

```csharp
builder.Services.AddDbContext<PieShopDbContext>(options =>
    options.UseSqlite("Data Source=pieshop.db"));
```

Create and apply the initial migration:

```shell
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
dotnet ef database update
```

### 1g — Update Endpoints to Use DbContext

Replace `IPieRepository` injection with `PieShopDbContext` in your endpoints. For now, use simple queries — we will optimize in the next step.

Example for `GET /{id}`:

```csharp
pieGroup.MapGet("/{id:int:min(1)}", async (int id, PieShopDbContext db) =>
{
    var pie = await db.Pies.FindAsync(id);
    return pie is null
        ? Results.NotFound()
        : Results.Ok(pie);
})
.WithName("Pies_GetById");
```

Update all CRUD endpoints similarly, converting from synchronous repository calls to async EF Core queries.

> **Checkpoint:** Run the app. All CRUD endpoints work against the SQLite database. Data persists across restarts.

---

## Step 2 — Advanced LINQ Queries

> **Falling behind?** Start from `lab02/step01-complete/PieShopApi/`.

### 2a — Composable IQueryable Filtering

Replace the `GET /pies` endpoint with a composable `IQueryable<PieEntity>` pipeline:

```csharp
pieGroup.MapGet("/", async ([AsParameters] PieQuery query, PieShopDbContext db) =>
{
    IQueryable<PieEntity> pies = db.Pies.Include(p => p.Category);

    if (!string.IsNullOrEmpty(query.Filter))
        pies = pies.Where(p => p.Name.Contains(query.Filter));

    // TODO: Add sorting and keyset pagination
    
    var results = await pies.ToListAsync();
    return TypedResults.Ok(results);
});
```

### 2b — Sorting

Add dynamic sorting. Support `name`, `price`, `price,desc`:

```csharp
pies = query.OrderBy?.ToLower() switch
{
    "name" => pies.OrderBy(p => p.Name),
    "price" => pies.OrderBy(p => p.Price),
    "price,desc" => pies.OrderByDescending(p => p.Price),
    _ => pies.OrderBy(p => p.PieId) // default sort for keyset pagination
};
```

### 2c — Keyset Pagination

In Day 1 (and in the MVC labs), you used **offset-based** pagination (`Skip/Take`). Offset pagination has a performance problem: `Skip(1000)` still reads 1000 rows.

**Keyset pagination** uses a "cursor" — the ID of the last item seen:

```csharp
if (query.AfterId.HasValue)
    pies = pies.Where(p => p.PieId > query.AfterId.Value);

var results = await pies.Take(query.PageSize).ToListAsync();
```

Update `PieQuery` to support keyset pagination:

```csharp
public record PieQuery(string? Filter, string? OrderBy, int? AfterId, int PageSize = 10);
```

### 2d — Projections with Select

Instead of returning full entities (which may include navigation properties you don't want), project to DTOs:

```csharp
var results = await pies
    .Take(query.PageSize)
    .Select(p => new PieDto(p.PieId, p.Name, p.ShortDescription, p.Price, p.IsPieOfTheWeek, p.Category.Name))
    .ToListAsync();
```

Create the DTO:

```csharp
public record PieDto(int PieId, string Name, string? ShortDescription, decimal Price, bool IsPieOfTheWeek, string CategoryName);
```

> **Checkpoint:** `GET /pies?filter=apple&afterId=2&pageSize=5` returns projected DTOs. No N+1 queries.

---

## Step 3 — Compiled Queries & Interceptors

> **Falling behind?** Start from `lab02/step02-complete/PieShopApi/`.

### 3a — Compiled Queries

The `GET /pies/{id}` endpoint is a hot path. Compiled queries pre-compile the expression tree so EF Core doesn't re-translate the LINQ every time.

Define a compiled query:

```csharp
public static class PieQueries
{
    public static readonly Func<PieShopDbContext, int, Task<PieEntity?>> GetById =
        EF.CompileAsyncQuery((PieShopDbContext db, int id) =>
            db.Pies.Include(p => p.Category).FirstOrDefault(p => p.PieId == id));
}
```

Use it in the endpoint:

```csharp
pieGroup.MapGet("/{id:int:min(1)}", async (int id, PieShopDbContext db) =>
{
    var pie = await PieQueries.GetById(db, id);
    return pie is null
        ? Results.NotFound()
        : Results.Ok(new PieDto(pie.PieId, pie.Name, pie.ShortDescription, pie.Price, pie.IsPieOfTheWeek, pie.Category.Name));
})
.WithName("Pies_GetById");
```

### 3b — Slow Query Interceptor

Create `Data/SlowQueryInterceptor.cs` that logs warnings for queries taking longer than a threshold:

```csharp
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PieShopApi.Data;

public class SlowQueryInterceptor(ILogger<SlowQueryInterceptor> logger, TimeSpan threshold) : DbCommandInterceptor
{
    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Duration > threshold)
        {
            logger.LogWarning(
                "Slow query detected ({Duration}ms): {CommandText}",
                eventData.Duration.TotalMilliseconds,
                command.CommandText);
        }
        return ValueTask.FromResult(result);
    }
}
```

Register it:

```csharp
builder.Services.AddSingleton(sp =>
    new SlowQueryInterceptor(
        sp.GetRequiredService<ILogger<SlowQueryInterceptor>>(),
        TimeSpan.FromMilliseconds(100)));

builder.Services.AddDbContext<PieShopDbContext>((sp, options) =>
    options.UseSqlite("Data Source=pieshop.db")
           .AddInterceptors(sp.GetRequiredService<SlowQueryInterceptor>()));
```

> **Checkpoint:** The compiled query works for `GET /pies/{id}`. Slow queries are logged with their SQL text.

---

## Step 4 — Output Caching

> **Falling behind?** Start from `lab02/step03-complete/PieShopApi/`.

Output caching stores complete HTTP responses in memory. When the same request arrives again, the cached response is returned without hitting your handler or the database.

### 4a — Add Output Caching Services

```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("PieList", policy =>
        policy.Expire(TimeSpan.FromMinutes(2))
              .SetVaryByQuery("filter", "orderBy", "afterId", "pageSize")
              .Tag("pies"));
});
```

Add the middleware (after routing, before endpoints):

```csharp
app.UseOutputCache();
```

### 4b — Apply Caching to Read Endpoints

```csharp
pieGroup.MapGet("/", async ([AsParameters] PieQuery query, PieShopDbContext db) =>
{
    // ... existing query logic
})
.CacheOutput("PieList");

pieGroup.MapGet("/{id:int:min(1)}", async (int id, PieShopDbContext db) =>
{
    // ... existing logic
})
.CacheOutput(p => p.Tag("pies"));
```

### 4c — Invalidate Cache on Writes

When a pie is created, updated, or deleted, the cached list is stale. Evict it:

```csharp
pieGroup.MapPost("/", async (CreatePieRequest request, PieShopDbContext db, IOutputCacheStore cache, CancellationToken ct) =>
{
    // ... save to DB
    await cache.EvictByTagAsync("pies", ct);
    return TypedResults.CreatedAtRoute(/* ... */);
});
```

Do the same for PUT and DELETE endpoints.

### 4d — Verify Caching

1. Call `GET /pies` twice — the second call should be nearly instant (check response headers for `Age`)
2. Create a new pie via POST
3. Call `GET /pies` again — the cache was invalidated, so you see the new pie

> **Checkpoint:** Read endpoints are cached. Write operations invalidate the cache. You can demonstrate the performance impact.

---

## Step 5 — Resilience with Microsoft.Extensions.Resilience

> **Falling behind?** Start from `lab02/step04-complete/PieShopApi/`.

Production APIs must handle transient failures — database timeouts, connection drops, etc. Instead of wrapping every call in try/catch with retry logic, use a centralized resilience pipeline.

### 5a — Add NuGet Package

```shell
dotnet add package Microsoft.Extensions.Resilience
```

### 5b — Define a Resilience Pipeline

Add the following using directives at the top of `Program.cs`:

```csharp
using Polly;
using Polly.Registry;
```

Then register the pipeline:

```csharp
builder.Services.AddResiliencePipeline("db-pipeline", pipelineBuilder =>
{
    pipelineBuilder
        .AddRetry(new Polly.Retry.RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(200),
            BackoffType = Polly.DelayBackoffType.Exponential,
            ShouldHandle = new PredicateBuilder().Handle<Microsoft.EntityFrameworkCore.DbUpdateException>()
        })
        .AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30),
            MinimumThroughput = 5,
            BreakDuration = TimeSpan.FromSeconds(15),
            ShouldHandle = new PredicateBuilder().Handle<Microsoft.EntityFrameworkCore.DbUpdateException>()
        })
        .AddTimeout(TimeSpan.FromSeconds(5));
});
```

### 5c — Use the Pipeline in Endpoints

Inject `ResiliencePipelineProvider<string>` and execute database operations through the pipeline:

```csharp
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
});
```

Apply the same pattern to PUT and DELETE endpoints.

### 5d — Observe the Pipeline

Add logging to see retry/circuit-breaker activity:

```csharp
builder.Services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Debug));
```

The resilience library logs its activity automatically. You should see log messages when retries occur.

> **Checkpoint:** Write endpoints use the resilience pipeline. You can explain when the circuit breaker would trip and what happens to subsequent requests.

---

## Step 6 — Rate Limiting

> **Falling behind?** Start from `lab02/step05-complete/PieShopApi/`.

Rate limiting protects your API from abuse and excessive load. This prepares you for the OWASP labs (Module 2.5, Lab API-4) where you will explore rate limiting from a security perspective.

### 6a — Add Rate Limiting Services

```csharp
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

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
```

Add the middleware (**after** `UseOutputCache`, **before** endpoint mapping):

```csharp
app.UseRateLimiter();
```

### 6b — Apply Policies to Endpoints

```csharp
pieGroup.MapGet("/", /* handler */).RequireRateLimiting("reads");
pieGroup.MapGet("/{id:int:min(1)}", /* handler */).RequireRateLimiting("reads");
pieGroup.MapPost("/", /* handler */).RequireRateLimiting("writes");
pieGroup.MapPut("/{id:int:min(1)}", /* handler */).RequireRateLimiting("writes");
pieGroup.MapDelete("/{id:int:min(1)}", /* handler */).RequireRateLimiting("writes");
```

### 6c — Test Rate Limiting

Send more than 10 POST requests within a minute:

```shell
for i in $(seq 1 15); do
  curl -s -o /dev/null -w "%{http_code}\n" -X POST https://localhost:<port>/pies \
    -H "Content-Type: application/json" \
    -d '{"name":"Test Pie '$i'","price":9.99}'
done
```

After 10 requests, you should receive `429 Too Many Requests`.

> **Checkpoint:** Write endpoints are limited to 10/min. Read endpoints are limited to 60/min. You see `429` responses when limits are exceeded.

---

## Lab Checkpoint

Your Pie Shop API now has:

- ✅ EF Core with SQLite, Fluent API, migrations, and seed data
- ✅ Composable LINQ queries with keyset pagination and projections
- ✅ Compiled queries for hot paths and slow-query interceptor
- ✅ Output caching with tag-based invalidation
- ✅ Resilience pipelines with retry, circuit-breaker, and timeout
- ✅ Rate limiting with fixed and sliding window policies

---

## Reflection Questions

1. When would you choose keyset pagination over offset pagination? What are the trade-offs?
2. How does output caching compare to response caching? When would you use each?
3. What happens to in-flight requests when the circuit breaker opens?
4. How would you implement per-user rate limiting instead of global limits?

---

## Lab Complete

> **Reference solution:** `lab02/step06-complete/PieShopApi/` contains the final state of this lab.

In the next lab, you will build a **gRPC service** for the Pie Shop and call it from your API, demonstrating inter-service communication.
