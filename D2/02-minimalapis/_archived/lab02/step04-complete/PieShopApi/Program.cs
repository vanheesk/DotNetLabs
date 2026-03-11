var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
var app = builder.Build();

var pies = new List<Pie>();
var nextId = 1;

var pieGroup = app.MapGroup("/pies");

pieGroup.MapGet("/", ([AsParameters] PieQuery query) =>
{
    var result = pies.AsEnumerable();

    if (!string.IsNullOrEmpty(query.Filter))
        result = result.Where(p => p.Name.Contains(query.Filter, StringComparison.OrdinalIgnoreCase));

    if (!string.IsNullOrEmpty(query.OrderBy))
    {
        result = query.OrderBy.ToLower() switch
        {
            "name" => result.OrderBy(p => p.Name),
            "price" => result.OrderBy(p => p.Price),
            "price,desc" => result.OrderByDescending(p => p.Price),
            _ => result
        };
    }

    return result
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize);
});

pieGroup.MapGet("/{id:int:min(1)}", (int id) =>
{
    var pie = pies.FirstOrDefault(p => p.PieId == id);
    return pie is null ? Results.NotFound() : Results.Ok(pie);
});

pieGroup.MapPost("/", (Pie pie) =>
{
    pie = pie with { PieId = nextId++ };
    pies.Add(pie);
    return Results.Created($"/pies/{pie.PieId}", pie);
});

pieGroup.MapPut("/{id:int:min(1)}", (int id, Pie updated) =>
{
    var index = pies.FindIndex(p => p.PieId == id);
    if (index == -1) return Results.NotFound();
    pies[index] = updated with { PieId = id };
    return Results.Ok(pies[index]);
});

pieGroup.MapDelete("/{id:int:min(1)}", (int id) =>
{
    var index = pies.FindIndex(p => p.PieId == id);
    if (index == -1) return Results.NotFound();
    pies.RemoveAt(index);
    return Results.NoContent();
});

app.Run();

public record Pie(int PieId, string Name, string? ShortDescription, decimal Price, bool IsPieOfTheWeek, int CategoryId);
public record PieQuery(string? Filter, string? OrderBy, int Page = 1, int PageSize = 10);