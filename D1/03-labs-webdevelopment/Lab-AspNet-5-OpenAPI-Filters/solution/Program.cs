using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ----- Exercise 5: Scalar UI -----
app.MapScalarApiReference(options =>
{
    options.Title = "Lab 5 API";
    options.Theme = ScalarTheme.BluePlanet;
});

app.UseExceptionHandler();
app.UseStatusCodePages();

// In-memory store
var products = new List<Product>
{
    new("Laptop", 999.99m, "Electronics"),
    new("Desk Chair", 349.00m, "Furniture")
};

// ----- Exercise 1: OpenAPI metadata -----
app.MapGet("/products", () => products)
    .WithName("GetProducts")
    .WithSummary("Get all products")
    .WithDescription("Returns the full list of products")
    .Produces<List<Product>>();

app.MapGet("/products/{index:int}", (int index) =>
        index >= 0 && index < products.Count
            ? Results.Ok(products[index])
            : Results.NotFound())
    .WithName("GetProductByIndex")
    .WithSummary("Get a product by index")
    .Produces<Product>()
    .ProducesProblem(404);

// ----- Exercise 2: Inline Endpoint Filter -----
app.MapPost("/products", (Product product) =>
{
    products.Add(product);
    return Results.Created($"/products/{products.Count - 1}", product);
})
.AddEndpointFilter(async (ctx, next) =>
{
    var product = ctx.GetArgument<Product>(0);
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(product.Name))
        errors["Name"] = ["Name is required"];
    if (product.Price <= 0)
        errors["Price"] = ["Price must be greater than 0"];

    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    return await next(ctx);
})
.WithName("CreateProduct")
.WithSummary("Create a new product")
.Produces<Product>(201)
.ProducesValidationProblem();

// ----- Exercise 3: Reusable Filter -----
app.MapPost("/products/v2", (Product product) =>
{
    products.Add(product);
    return Results.Created($"/products/{products.Count - 1}", product);
})
.AddEndpointFilter<ProductValidationFilter>()
.WithName("CreateProductV2")
.WithSummary("Create a product (reusable filter)")
.WithTags("V2");

// ----- Exercise 4: ProblemDetails -----
app.MapGet("/problem", () =>
    Results.Problem("Something went wrong on the server", statusCode: 500))
    .WithName("TriggerProblem")
    .WithSummary("Returns a ProblemDetails response")
    .ProducesProblem(500);

app.MapGet("/error-test", () =>
{
    throw new InvalidOperationException("Test exception for ProblemDetails");
})
.WithName("ErrorTest")
.WithSummary("Throws an exception to test error handling")
.ExcludeFromDescription();

app.MapGet("/", () => "Lab 5: OpenAPI & Endpoint Filters");

app.Run();

// ----- Types -----

public record Product(string Name, decimal Price, string? Category);

// ----- Reusable Filter (Exercise 3) -----
public class ProductValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var product = context.GetArgument<Product>(0);
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(product.Name))
            errors["Name"] = ["Name is required"];
        if (product.Price <= 0)
            errors["Price"] = ["Price must be greater than 0"];

        if (errors.Count > 0)
            return Results.ValidationProblem(errors);

        return await next(context);
    }
}
