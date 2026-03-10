var builder = WebApplication.CreateBuilder(args);

// ----- Exercise 4: Add Aspire service defaults -----
builder.AddServiceDefaults();

var app = builder.Build();

// ----- Exercise 4: Map default health-check endpoints -----
app.MapDefaultEndpoints();

app.MapGet("/products", () => new[]
{
    new { Id = 1, Name = "Widget", Price = 9.99 },
    new { Id = 2, Name = "Gizmo", Price = 14.99 },
    new { Id = 3, Name = "Thingamajig", Price = 24.99 }
});

app.MapGet("/", () => "Lab 10: CatalogService");

app.Run();
