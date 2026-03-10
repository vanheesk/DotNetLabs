using System.Diagnostics;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO Exercise 2: Configure OpenTelemetry Tracing
// builder.Services.AddOpenTelemetry()
//     .WithTracing(tracing =>
//     {
//         tracing
//             .AddAspNetCoreInstrumentation()
//             .AddSource("Lab8")
//             .AddConsoleExporter();
//     });

// TODO Exercise 3: Configure OpenTelemetry Metrics
//     .WithMetrics(metrics =>
//     {
//         metrics
//             .AddAspNetCoreInstrumentation()
//             .AddRuntimeInstrumentation()
//             .AddMeter("Lab8")
//             .AddConsoleExporter();
//     });

// TODO Exercise 4: Configure OpenTelemetry Logging
// builder.Logging.AddOpenTelemetry(logging =>
// {
//     logging.AddConsoleExporter();
// });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

string[] summaries = ["Freezing", "Cool", "Mild", "Warm", "Hot", "Scorching"];

// TODO Exercise 2: Create an ActivitySource
// var activitySource = new ActivitySource("Lab8");

// TODO Exercise 3: Create a Meter and Counter
// var meter = new Meter("Lab8");
// var requestCounter = meter.CreateCounter<int>("weather.requests", "requests", "Number of weather requests");

app.MapGet("/weather", (ILogger<Program> logger) =>
{
    // TODO Exercise 2: Start a custom span
    // using var activity = activitySource.StartActivity("GenerateForecast");

    // TODO Exercise 3: Increment the counter
    // requestCounter.Add(1);

    // TODO Exercise 4: Log a structured message
    // logger.LogInformation("Weather forecast requested at {Time}", DateTime.UtcNow);

    var forecast = Enumerable.Range(1, 5).Select(i => new WeatherForecast(
        DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)]
    )).ToArray();

    return Results.Ok(forecast);
}).WithTags("Weather");

app.MapGet("/", () => "Lab 8: Observability with OpenTelemetry");

app.Run();

// ----- Types -----
public record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
