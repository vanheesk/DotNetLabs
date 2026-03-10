using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Exercise 3: Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ----- Exercise 2: Hello endpoint -----
app.MapGet("/hello/{name}", (string name) => $"Hello, {name}!");

// ----- Exercise 4: More endpoints -----
app.MapGet("/time", () => Results.Ok(new { utc = DateTime.UtcNow }));

app.MapGet("/random/{min:int}/{max:int}", (int min, int max) =>
{
    var value = Random.Shared.Next(min, max + 1);
    return Results.Ok(new { min, max, value });
});

app.MapPost("/echo", (JsonElement body) => Results.Ok(body));

app.Run();
