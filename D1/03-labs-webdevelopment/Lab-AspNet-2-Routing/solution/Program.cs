var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// In-memory store
var products = new List<Product>
{
    new(1, "Laptop", "Electronics", 999.99m),
    new(2, "Desk Chair", "Furniture", 349.00m),
    new(3, "Keyboard", "Electronics", 79.99m),
    new(4, "Monitor", "Electronics", 449.00m),
    new(5, "Standing Desk", "Furniture", 599.00m)
};

// ----- Exercise 1: Basic Minimal API Routing -----

app.MapGet("/products", () => products.Select(p => p.Name))
   .WithTags("Products");

app.MapGet("/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
}).WithTags("Products");

app.MapPost("/products", (Product product) =>
{
    products.Add(product);
    return Results.Created($"/products/{product.Id}", product);
}).WithTags("Products");

// ----- Exercise 2: Route Groups for API Versioning -----

var v1 = app.MapGroup("/api/v1/products").WithTags("V1");
v1.MapGet("/", () => products);
v1.MapGet("/{id:int}", (int id) =>
    products.FirstOrDefault(p => p.Id == id) is { } p
        ? Results.Ok(p)
        : Results.NotFound());

var v2 = app.MapGroup("/api/v2/products").WithTags("V2");
v2.MapGet("/", (string? category, decimal? minPrice) =>
{
    var filtered = products.AsEnumerable();
    if (category is not null)
        filtered = filtered.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    if (minPrice is not null)
        filtered = filtered.Where(p => p.Price >= minPrice);
    return filtered;
});
v2.MapGet("/{id:int}", (int id) =>
    products.FirstOrDefault(p => p.Id == id) is { } p
        ? Results.Ok(p)
        : Results.NotFound());

// ----- Exercise 3: Route Constraints -----

app.MapGet("/users/{username:alpha:minlength(3)}", (string username) =>
    Results.Ok(new { username, message = $"User profile for {username}" }))
    .WithTags("Users");

app.MapGet("/orders/{id:guid}", (Guid id) =>
    Results.Ok(new { orderId = id, status = "Processing" }))
    .WithTags("Orders");

// ----- Exercise 4: Applying Metadata to Groups -----

var admin = app.MapGroup("/api/admin")
    .RequireAuthorization()
    .WithTags("Admin");

admin.MapGet("/stats", () => new { totalProducts = products.Count, totalValue = products.Sum(p => p.Price) });
admin.MapGet("/users", () => new[] { "admin", "moderator" });

app.Run();

// ----- Types -----
public record Product(int Id, string Name, string Category, decimal Price);
