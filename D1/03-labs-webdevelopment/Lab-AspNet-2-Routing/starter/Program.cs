var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ----- Exercise 1: Basic Minimal API Routing -----

// TODO: Map GET "/products" that returns a list of product names
// TODO: Map GET "/products/{id:int}" that returns a product by id
// TODO: Map POST "/products" that accepts a Product record and returns Created

// ----- Exercise 2: Route Groups for API Versioning -----

// TODO: Create a route group for "/api/v1/products" with basic CRUD
//       var v1 = app.MapGroup("/api/v1/products").WithTags("V1");
//       v1.MapGet("/", () => ...);

// TODO: Create a route group for "/api/v2/products" with filtering support
//       var v2 = app.MapGroup("/api/v2/products").WithTags("V2");
//       v2.MapGet("/", (string? category) => ...);

// ----- Exercise 3: Route Constraints -----

// TODO: Map GET "/users/{username:alpha:minlength(3)}"
// TODO: Map GET "/orders/{id:guid}"

// ----- Exercise 4: Applying Metadata to Groups -----

// TODO: Create a group "/api/admin" with .RequireAuthorization() and .WithTags("Admin")
//       Add GET endpoints under it

app.Run();

// ----- Types -----
// TODO: Define a Product record
// public record Product(int Id, string Name, string Category, decimal Price);
