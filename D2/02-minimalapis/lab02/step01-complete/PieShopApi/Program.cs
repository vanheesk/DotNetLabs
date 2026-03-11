var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
var app = builder.Build();

var pies = new List<Pie>();
var nextId = 1;

app.MapGet("/pies", () => pies);

app.MapGet("/pies/{id}", (int id) =>
{
    var pie = pies.FirstOrDefault(p => p.PieId == id);
    return pie is null ? Results.NotFound() : Results.Ok(pie);
});

app.MapPost("/pies", (Pie pie) =>
{
    pie = pie with { PieId = nextId++ };
    pies.Add(pie);
    return Results.Created($"/pies/{pie.PieId}", pie);
});

app.MapPut("/pies/{id}", (int id, Pie updated) =>
{
    var index = pies.FindIndex(p => p.PieId == id);
    if (index == -1) return Results.NotFound();
    pies[index] = updated with { PieId = id };
    return Results.Ok(pies[index]);
});

app.MapDelete("/pies/{id}", (int id) =>
{
    var index = pies.FindIndex(p => p.PieId == id);
    if (index == -1) return Results.NotFound();
    pies.RemoveAt(index);
    return Results.NoContent();
});

app.Run();

public record Pie(int PieId, string Name, string? ShortDescription, decimal Price, bool IsPieOfTheWeek, int CategoryId);