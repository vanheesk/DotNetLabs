using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
var app = builder.Build();

app.MapGet("/ping", () => "pong");

app.MapGet("/time", (ILogger<Program> logger) =>
{
    logger.LogInformation("Time endpoint called");
    return DateTime.UtcNow;
});

app.MapPost("/pies", (CreatePieRequest request) =>
{
    return Results.Created($"/pies/{Guid.NewGuid()}", request);
});

app.Run();

public sealed record CreatePieRequest(
    [Required, StringLength(100, MinimumLength = 3)] string Name,
    [StringLength(200)] string? ShortDescription,
    [Range(0.01, 1000)] decimal Price);