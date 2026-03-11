var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// TODO (Exercise 2): Add rate limiter services
// builder.Services.AddRateLimiter(options => { ... });

// TODO (Exercise 5): Configure request body size limits
// builder.WebHost.ConfigureKestrel(options => { ... });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// TODO (Exercise 2): Add rate limiter middleware
// app.UseRateLimiter();

// ----- In-memory product store -----
var products = Enumerable.Range(1, 1000)
    .Select(i => new Product(i, $"Product {i}", Math.Round(Random.Shared.NextDouble() * 100, 2), $"Category {i % 10}"))
    .ToList();

// =====================================================
// EXERCISE 1: No rate limiting, no pagination
// =====================================================

// ⚠️ VULNERABLE: Returns ALL products with no limits
app.MapGet("/api/products", () => products)
    .WithName("GetProducts")
    .WithSummary("Get products (NO rate limiting, NO pagination)")
    .WithTags("Products");

app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProduct")
.WithSummary("Get a product by ID")
.WithTags("Products");

// TODO (Exercise 2): Apply .RequireRateLimiting("fixed") to endpoints

// TODO (Exercise 4): Add paginated endpoint
// GET /api/products/paged?page=1&pageSize=10
// Enforce max pageSize of 50
// Return pagination metadata

app.MapGet("/", () => "Lab OWASP-API-4: Unrestricted Resource Consumption (API4:2023)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record Product(int Id, string Name, double Price, string Category);
