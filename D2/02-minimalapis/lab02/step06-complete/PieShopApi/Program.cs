using Microsoft.AspNetCore.Http.HttpResults;
using PieShopApi.Models;

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

    return TypedResults.Ok(result
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize));
});

pieGroup.MapGet("/{id:int:min(1)}", Results<Ok<Pie>, NotFound> (int id) =>
{
    var pie = pies.FirstOrDefault(p => p.PieId == id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(pie);
})
.WithName("Pies_GetById");

pieGroup.MapPost("/", (CreatePieRequest request) =>
{
    var pie = new Pie(nextId++, request.Name, request.ShortDescription, request.Price, request.IsPieOfTheWeek, request.CategoryId);
    pies.Add(pie);
    return TypedResults.CreatedAtRoute(pie, "Pies_GetById", new { id = pie.PieId });
});

pieGroup.MapPut("/{id:int:min(1)}", Results<Ok<Pie>, NotFound> (int id, UpdatePieRequest request) =>
{
    var index = pies.FindIndex(p => p.PieId == id);
    if (index == -1) return TypedResults.NotFound();
    pies[index] = new Pie(id, request.Name, request.ShortDescription, request.Price, request.IsPieOfTheWeek, request.CategoryId);
    return TypedResults.Ok(pies[index]);
});

pieGroup.MapDelete("/{id:int:min(1)}", Results<NoContent, NotFound> (int id) =>
{
    var index = pies.FindIndex(p => p.PieId == id);
    if (index == -1) return TypedResults.NotFound();
    pies.RemoveAt(index);
    return TypedResults.NoContent();
});

app.Run();