var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO (Exercise 4): Register ProblemDetails
// builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// TODO (Exercise 4): Add exception handler and status code pages
// app.UseExceptionHandler();
// app.UseStatusCodePages();

// ----- Exercise 1: OpenAPI metadata -----
// TODO: Add a GET "/products" endpoint with OpenAPI metadata:
//       .WithName("GetProducts")
//       .WithSummary("Get all products")
//       .WithDescription("Returns the full list of products")
//       .Produces<Product[]>()

// ----- Exercise 2: Endpoint Filter for Validation -----
// TODO: Add a POST "/products" endpoint with an inline validation filter
//       Validate that Name is not empty and Price > 0
//       Return Results.ValidationProblem(...) on failure

// ----- Exercise 3: Reusable Filter -----
// TODO: Create a ValidationFilter<Product> class and apply it

// ----- Exercise 4: ProblemDetails -----
// TODO: Add a GET "/problem" endpoint that returns Results.Problem(...)

app.MapGet("/", () => "Lab 5: OpenAPI & Endpoint Filters");

app.Run();

// ----- Types -----
// public record Product(string Name, decimal Price, string? Category);
