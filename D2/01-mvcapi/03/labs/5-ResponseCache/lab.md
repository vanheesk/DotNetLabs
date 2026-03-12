# Lab 5: Response Caching

## Objective

Learn how to configure client-side (browser) caching using `[ResponseCache]` attributes, `VaryByQueryKeys`, and cache profiles.

## Prerequisites

- .NET 10 SDK
- Browser with developer tools (F12)

## Project Structure

- **PieShopApi** — ASP.NET Core Web API on `https://localhost:7043`
- **PieShopBlazorClient** — Blazor Web App on `https://localhost:7282`

## Getting Started

Open **two terminals** side by side.

Terminal 1 (API):
```bash
cd starter/PieShopApi
dotnet run --launch-profile https
```

Terminal 2 (Blazor client):
```bash
cd starter/PieShopBlazorClient
dotnet run --launch-profile https
```

---

## Part 1 — Time-based Response Caching

### Step 1: Add the `[ResponseCache]` attribute

Open `Controllers/DateTimeController.cs` and add the response cache attribute to the `Get` method:

```csharp
[ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
```

### Step 2: Test with browser cache enabled

1. Open `https://localhost:7282` in the browser
2. Open **Dev Tools** (F12) → **Network** tab
3. Make sure **"Disable cache"** is **off**
4. Click the **"Get the date/time"** button
5. Inspect the response `Cache-Control` header: `public, max-age=120`
6. Click the button multiple times → **same result** (browser uses cached response)

### Step 3: Test with browser cache disabled

1. Check the **"Disable cache"** checkbox in Dev Tools
2. Click the button multiple times → **new response every time**
3. Key takeaway: the client controls response caching; the server only sets the header

---

## Part 2 — Response Caching by Query String

### Step 4: Test with different query string parameters

Use the second section of the Blazor page. Try various combinations of `id` and `name`:

| id | name | Expected |
|----|------|----------|
| 1 | Koen | New response |
| 1 | Gill | Same as above — cached! |
| 2 | Gill | Same — cached! |

All responses are the same because the default cache doesn't vary by query params.

### Step 5: Add response caching services and middleware

In `Program.cs`, add:

```csharp
builder.Services.AddResponseCaching();
```

And before `MapControllers()`:

```csharp
app.UseResponseCaching();
```

### Step 6: Add `VaryByQueryKeys`

Update the second method's attribute:

```csharp
[ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "id" })]
```

### Step 7: Test again (use incognito to clear cache)

| id | name | Expected |
|----|------|----------|
| 1 | Koen | New response |
| 1 | Gill | Same as above — same `id` |
| 2 | Gill | Different — different `id` |
| 2 | Koen | Same as previous — same `id` |

---

## Part 3 — Response Cache Profiles

### Step 8: Define a cache profile

Replace `builder.Services.AddControllers()` with:

```csharp
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Cache2Minutes", new Microsoft.AspNetCore.Mvc.CacheProfile
    {
        Duration = 120,
        Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any
    });
});
```

### Step 9: Use the profile

Update the first method to use the profile:

```csharp
[ResponseCache(CacheProfileName = "Cache2Minutes")]
```

### Step 10: Test

Restart the API and test in incognito. Behavior is identical, but the cache settings are now reusable via the profile name.

---

## Verification

When you have completed the lab:

- Response includes `Cache-Control: public, max-age=120` header
- Browser caching works (same response on repeated clicks with cache enabled)
- `VaryByQueryKeys` produces separate cached responses per `id`
- Cache profiles provide reusable cache settings
