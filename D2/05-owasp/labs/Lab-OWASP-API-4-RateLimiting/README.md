# Lab 9: Unrestricted Resource Consumption (OWASP API4:2023)

## Objective

Understand **Unrestricted Resource Consumption** and learn to protect APIs from abuse using ASP.NET Core's built-in **rate limiting middleware**, pagination, and request size limits.

---

## Background

**API4:2023** covers APIs that don't limit the resources a client can consume. Without limits, attackers can:

- **Overwhelm the API** with excessive requests (DoS)
- **Extract large datasets** by requesting unbounded results
- **Consume server resources** through expensive operations
- **Abuse free-tier quotas** to bypass payment requirements

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Identify Missing Rate Limits

The starter API has no rate limiting — any client can make unlimited requests.

### Tasks

1. Run the starter project.
2. Use a loop to make rapid requests to `GET /api/products`:
   ```bash
   for i in {1..100}; do curl -s http://localhost:5000/api/products > /dev/null & done
   ```
3. Observe that all 100 requests succeed — no throttling.
4. Try `GET /api/products?pageSize=10000` — no limit on result size.
5. Discuss the impact of unlimited access on system resources.

---

## Exercise 2 – Add Fixed Window Rate Limiting

ASP.NET Core has built-in rate limiting middleware.

### Tasks

1. Add the rate limiter service:
   ```csharp
   builder.Services.AddRateLimiter(options =>
   {
       options.AddFixedWindowLimiter("fixed", opt =>
       {
           opt.PermitLimit = 10;
           opt.Window = TimeSpan.FromMinutes(1);
           opt.QueueLimit = 0;
       });
   });
   ```
2. Add `app.UseRateLimiter()` to the pipeline.
3. Apply the rate limiter to endpoints: `.RequireRateLimiting("fixed")`.
4. Test by making more than 10 requests per minute — verify you get **429 Too Many Requests**.

---

## Exercise 3 – Add Sliding Window & Token Bucket Policies

Different endpoints may need different rate limiting strategies.

### Tasks

1. Add a **sliding window** policy for general API access:
   ```csharp
   options.AddSlidingWindowLimiter("sliding", opt =>
   {
       opt.PermitLimit = 20;
       opt.Window = TimeSpan.FromMinutes(1);
       opt.SegmentsPerWindow = 4;
   });
   ```
2. Add a **token bucket** policy for burst-friendly endpoints:
   ```csharp
   options.AddTokenBucketLimiter("token-bucket", opt =>
   {
       opt.TokenLimit = 10;
       opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
       opt.TokensPerPeriod = 2;
   });
   ```
3. Apply different policies to different endpoints.
4. Customise the 429 response to include a `Retry-After` header.

---

## Exercise 4 – Add Pagination Limits

### Tasks

1. Add `page` and `pageSize` query parameters to the products endpoint.
2. Enforce a **maximum page size** (e.g., 50 items).
3. Return pagination metadata in the response (`totalItems`, `page`, `pageSize`, `totalPages`).
4. Return `400 Bad Request` for invalid pagination parameters.

---

## Exercise 5 – Request Size Limits

### Tasks

1. Configure maximum request body size:
   ```csharp
   builder.WebHost.ConfigureKestrel(options =>
   {
       options.Limits.MaxRequestBodySize = 1_048_576; // 1 MB
   });
   ```
2. Add a file upload endpoint and test with files exceeding the limit.
3. Verify that oversized requests are rejected with `413 Payload Too Large`.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- **Rate limit all API endpoints** — different strategies for different needs
- **Enforce pagination** — never return unbounded result sets
- **Limit request sizes** — prevent resource exhaustion via large payloads
- **Return proper 429 responses** with `Retry-After` headers
- ASP.NET Core's built-in rate limiting is production-ready
