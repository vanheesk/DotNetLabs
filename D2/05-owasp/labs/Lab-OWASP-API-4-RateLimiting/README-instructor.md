# Lab 9: Unrestricted Resource Consumption (OWASP API4:2023) — Instructor Guide

## Teaching Notes

Rate limiting is one of the most practical and impactful security measures. Students often overlook it because they focus on authentication and authorization.

### Key Points to Emphasise

1. **Rate limiting is essential for all production APIs** — not just for billing
2. **Different strategies suit different scenarios** — fixed window, sliding window, token bucket
3. **Pagination prevents data exfiltration** — never return unbounded results
4. **ASP.NET Core has built-in rate limiting** — no need for third-party packages
5. **429 responses should include `Retry-After`** — helps legitimate clients

### Demo Flow

1. Show unprotected API handling 100+ rapid requests
2. Add fixed window rate limiter and show 429 responses
3. Compare different rate limiting strategies
4. Add pagination and show bounded results

---

## Exercise 2 – Solution

```csharp
using System.Threading.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "60";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests",
            retryAfterSeconds = 60
        }, cancellationToken);
    };
});

app.UseRateLimiter();
```

---

## Exercise 3 – Solution

```csharp
options.AddSlidingWindowLimiter("sliding", opt =>
{
    opt.PermitLimit = 20;
    opt.Window = TimeSpan.FromMinutes(1);
    opt.SegmentsPerWindow = 4;
    opt.QueueLimit = 0;
});

options.AddTokenBucketLimiter("token-bucket", opt =>
{
    opt.TokenLimit = 10;
    opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
    opt.TokensPerPeriod = 2;
    opt.QueueLimit = 5;
});
```

---

## Exercise 4 – Solution

```csharp
app.MapGet("/api/products", (int page = 1, int pageSize = 10) =>
{
    if (page < 1) return Results.BadRequest("Page must be >= 1");
    if (pageSize < 1 || pageSize > 50) return Results.BadRequest("Page size must be between 1 and 50");

    var totalItems = products.Count;
    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    var items = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    return Results.Ok(new
    {
        items,
        pagination = new { page, pageSize, totalItems, totalPages }
    });
});
```

---

## Exercise 5 – Solution

```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1_048_576; // 1 MB
});
```

---

## Common Student Issues

1. **Forgetting `app.UseRateLimiter()`** — services alone don't apply the middleware
2. **Rate limiter placement** — must be before the endpoint routing
3. **Testing rate limits** — students may need to use parallel curl or a tool like `hey`
4. **Understanding window types** — explain fixed vs sliding with diagrams
