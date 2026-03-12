# Lab 4: Output Caching

## Objective

Learn how to use server-side Output Caching to store and serve entire HTTP responses, including cache variation, revalidation with ETags, cache eviction, and cache policies.

## Prerequisites

- .NET 10 SDK
- VS Code with REST Client extension (for `.http` files), or another HTTP testing tool

## Getting Started

Open the `starter/PieShopApi` project:

```bash
cd starter/PieShopApi
dotnet run --launch-profile https
```

The API will be available at `https://localhost:7043`.

Use the included `PieShopApi.http` file for testing.

---

## Part 1 — Basic Output Cache

### Step 1: Add output caching services

In `Program.cs`, add output caching services:

```csharp
builder.Services.AddOutputCache();
```

### Step 2: Add the output cache middleware

Add the middleware **after** `MapControllers()`:

```csharp
app.UseOutputCache();
```

### Step 3: Add the `[OutputCache]` attribute

Open `Controllers/DateTimeController.cs` and add `[OutputCache]` to the `GetOutputCache` method:

```csharp
[HttpGet]
[Route("fromoutputcache")]
[OutputCache]
public async Task<IActionResult> GetOutputCache()
```

### Step 4: Test

1. Call `GET /datetime/fromoutputcache` — first response takes **5 seconds**
2. Call again immediately — **instant** response with the same cached date/time
3. Wait ~1 minute (default expiration) — next call takes 5 seconds again

---

## Part 2 — Varying by Header

### Step 5: Update the attribute to vary by header

Change `[OutputCache]` to include header variation:

```csharp
[OutputCache(Duration = 120, VaryByHeaderNames = new string[] { "X-CacheKey" })]
```

### Step 6: Test with different header values

Send requests with different `X-CacheKey` header values. Same header value → cached response. Different value → new cache entry (5-second delay).

---

## Part 3 — Cache Revalidation (ETag)

### Step 7: Add the `[OutputCache]` attribute to the revalidation endpoint

The `GetOutputCacheRevalidation` method already sets an ETag header. Add `[OutputCache]` to it:

```csharp
[OutputCache]
```

### Step 8: Test

1. Call `GET /datetime/fromoutputcacherevalidation` — note the `ETag` response header
2. Call again with `If-None-Match` set to that ETag → **304 Not Modified**

---

## Part 4 — Cache Eviction

### Step 9: Add tag-based caching

Add `[OutputCache(Tags = new string[] { "tag-datetime" })]` to the eviction endpoint.

### Step 10: Inject `IOutputCacheStore`

Add `IOutputCacheStore` to the controller constructor and use it in the `Purge` method.

### Step 11: Test

1. Call `GET /datetime/fromoutputcacheeviction` — 5 seconds
2. Call again — cached (instant)
3. Call `POST /datetime/purge/tag-datetime` — cache cleared
4. Call `GET /datetime/fromoutputcacheeviction` — 5 seconds again

---

## Part 5 — Output Cache Policies

### Step 12: Add cache policies

Replace `builder.Services.AddOutputCache()` with:

```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(10)));
    options.AddPolicy("CacheForThirtySeconds", builder =>
        builder.Expire(TimeSpan.FromSeconds(30)));
});
```

### Step 13: Test with policies

- `[OutputCache]` uses the base policy (10-second expiration)
- `[OutputCache(PolicyName = "CacheForThirtySeconds")]` uses the named policy (30 seconds)

---

## Verification

When you have completed the lab:

- Basic output caching works (first call slow, subsequent instant)
- Varying by header produces separate cache entries
- ETag revalidation returns 304 Not Modified
- Cache eviction by tag works
- Base and named policies control cache duration
