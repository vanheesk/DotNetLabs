using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// Exercise 2 & 3: Rate limiting configuration
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Exercise 2: Fixed window limiter
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Exercise 3: Sliding window limiter
    options.AddSlidingWindowLimiter("sliding", opt =>
    {
        opt.PermitLimit = 20;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 4;
        opt.QueueLimit = 0;
    });

    // Exercise 3: Token bucket limiter
    options.AddTokenBucketLimiter("token-bucket", opt =>
    {
        opt.TokenLimit = 10;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        opt.TokensPerPeriod = 2;
        opt.QueueLimit = 5;
    });

    // Custom rejection response with Retry-After header
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "60";
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests. Please try again later.",
            retryAfterSeconds = 60
        }, cancellationToken);
    };
});

// Exercise 5: Request body size limits
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1_048_576; // 1 MB
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Apply rate limiter middleware
app.UseRateLimiter();

// ----- In-memory product store -----
var products = Enumerable.Range(1, 1000)
    .Select(i => new Product(i, $"Product {i}", Math.Round(Random.Shared.NextDouble() * 100, 2), $"Category {i % 10}"))
    .ToList();

// =====================================================
// Exercise 2: Rate-limited endpoint (fixed window)
// =====================================================

app.MapGet("/api/products/all", () => products)
    .RequireRateLimiting("fixed")
    .WithName("GetAllProducts")
    .WithSummary("Get all products (rate limited — fixed window: 10/min)")
    .WithTags("Products");

// =====================================================
// Exercise 3: Different rate limiting policies
// =====================================================

app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.RequireRateLimiting("sliding")
.WithName("GetProduct")
.WithSummary("Get a product by ID (sliding window: 20/min)")
.WithTags("Products");

app.MapGet("/api/products/search", (string? name, string? category) =>
{
    var query = products.AsEnumerable();
    if (!string.IsNullOrEmpty(name))
        query = query.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrEmpty(category))
        query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    return query.Take(50).ToList();
})
.RequireRateLimiting("token-bucket")
.WithName("SearchProducts")
.WithSummary("Search products (token bucket: burst-friendly)")
.WithTags("Products");

// =====================================================
// Exercise 4: Paginated endpoint
// =====================================================

app.MapGet("/api/products", (int page = 1, int pageSize = 10) =>
{
    if (page < 1)
        return Results.BadRequest(new { error = "Page must be >= 1" });
    if (pageSize < 1 || pageSize > 50)
        return Results.BadRequest(new { error = "Page size must be between 1 and 50" });

    var totalItems = products.Count;
    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    var items = products
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return Results.Ok(new
    {
        items,
        pagination = new { page, pageSize, totalItems, totalPages }
    });
})
.RequireRateLimiting("sliding")
.WithName("GetProductsPaged")
.WithSummary("Get products with pagination (max 50 per page)")
.WithTags("Products");

// =====================================================
// Exercise 5: Upload endpoint with size limits
// =====================================================

app.MapPost("/api/upload", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest("Expected form content type");

    var form = await request.ReadFormAsync();
    var file = form.Files.FirstOrDefault();

    if (file is null)
        return Results.BadRequest("No file uploaded");

    return Results.Ok(new
    {
        fileName = file.FileName,
        size = file.Length,
        contentType = file.ContentType
    });
})
.RequireRateLimiting("fixed")
.WithName("UploadFile")
.WithSummary("Upload a file (max 1 MB)")
.WithTags("Upload")
.DisableAntiforgery();

app.MapGet("/", () => "Lab OWASP-API-4: Unrestricted Resource Consumption (API4:2023)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record Product(int Id, string Name, double Price, string Category);
