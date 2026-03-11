var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/ping", () => "pong");

app.MapGet("/time", (ILogger<Program> logger) =>
{
    logger.LogInformation("Time endpoint called");
    return DateTime.UtcNow;
});

app.Run();