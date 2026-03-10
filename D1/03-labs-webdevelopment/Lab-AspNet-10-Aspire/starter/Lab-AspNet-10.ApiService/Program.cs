var builder = WebApplication.CreateBuilder(args);

// TODO Exercise 2: Add Aspire service defaults (OpenTelemetry, health checks, resilience, service discovery)
// builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// TODO Exercise 2: Map default health-check endpoints (/health and /alive)
// app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

string[] summaries = ["Freezing", "Cool", "Mild", "Warm", "Hot", "Scorching"];

app.MapGet("/weather", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(i => new WeatherForecast(
        DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)]
    )).ToArray();

    return Results.Ok(forecast);
}).WithTags("Weather")
  .WithName("GetWeather")
  .WithSummary("Get a 5-day weather forecast");

// TODO Exercise 5: Add /catalog endpoint that calls the CatalogService via service discovery
// builder.Services.AddHttpClient();  // <-- move this above var app = builder.Build();
// app.MapGet("/catalog", async (IHttpClientFactory httpClientFactory) =>
// {
//     var client = httpClientFactory.CreateClient();
//     var response = await client.GetStringAsync("http://catalogservice/products");
//     return Results.Content(response, "application/json");
// });

app.MapGet("/", () => "Lab 10: .NET Aspire");

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
