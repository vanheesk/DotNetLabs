using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----- Exercise 2 & 3: Configure OpenTelemetry -----
builder.Services.AddOpenTelemetry()
    .ConfigureResource(res => res.AddService("Lab8-Observability"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("Lab8")
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("Lab8")
            .AddConsoleExporter();
    });

// ----- Exercise 4: Configure OpenTelemetry Logging -----
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.AddConsoleExporter();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ----- Exercise 2: Create an ActivitySource -----
var activitySource = new ActivitySource("Lab8");

// ----- Exercise 3: Create a Meter and Counter -----
var meter = new Meter("Lab8");
var requestCounter = meter.CreateCounter<int>("weather.requests", "requests", "Number of weather requests");

string[] summaries = ["Freezing", "Cool", "Mild", "Warm", "Hot", "Scorching"];

app.MapGet("/weather", (ILogger<Program> logger) =>
{
    // Exercise 2: Start a custom span
    using var activity = activitySource.StartActivity("GenerateForecast");
    activity?.SetTag("forecast.count", 5);

    // Exercise 3: Increment the counter
    requestCounter.Add(1);

    // Exercise 4: Log a structured message
    logger.LogInformation("Weather forecast requested at {Time}", DateTime.UtcNow);

    var forecast = Enumerable.Range(1, 5).Select(i => new WeatherForecast(
        DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)]
    )).ToArray();

    activity?.SetTag("forecast.min_temp", forecast.Min(f => f.TemperatureC));
    activity?.SetTag("forecast.max_temp", forecast.Max(f => f.TemperatureC));

    return Results.Ok(forecast);
}).WithTags("Weather")
  .WithName("GetWeather")
  .WithSummary("Get a 5-day weather forecast");

app.MapGet("/", () => "Lab 8: Observability with OpenTelemetry");

app.Run();

// ----- Types -----
public record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
