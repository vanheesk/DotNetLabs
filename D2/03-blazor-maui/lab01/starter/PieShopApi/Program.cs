using Microsoft.AspNetCore.Http.HttpResults;
using PieShopApi.Models;
using PieShopApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPieRepository, InMemoryPieRepository>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

var pieGroup = app.MapGroup("/pies").WithTags("Pies");

pieGroup.MapGet("/", ([AsParameters] PieQuery query, IPieRepository repo) =>
{
    var result = repo.GetAll();

    if (!string.IsNullOrEmpty(query.Filter))
        result = result.Where(p => p.Name.Contains(query.Filter, StringComparison.OrdinalIgnoreCase));

    return TypedResults.Ok(result
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize));
}).WithName("GetAllPies");

pieGroup.MapGet("/{id:int:min(1)}", Results<Ok<Pie>, NotFound> (int id, IPieRepository repo) =>
{
    var pie = repo.GetById(id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(pie);
}).WithName("GetPieById");

pieGroup.MapPost("/", (CreatePieRequest request, IPieRepository repo) =>
{
    var pie = new Pie(0, request.Name, request.ShortDescription, request.Price, request.IsPieOfTheWeek, request.CategoryId);
    pie = repo.Add(pie);
    return TypedResults.CreatedAtRoute(pie, "GetPieById", new { id = pie.PieId });
}).WithName("CreatePie");

pieGroup.MapPut("/{id:int:min(1)}", Results<Ok<Pie>, NotFound> (int id, UpdatePieRequest request, IPieRepository repo) =>
{
    var updated = new Pie(id, request.Name, request.ShortDescription, request.Price, request.IsPieOfTheWeek, request.CategoryId);
    var result = repo.Update(id, updated);
    return result is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(result);
}).WithName("UpdatePie");

pieGroup.MapDelete("/{id:int:min(1)}", Results<NoContent, NotFound> (int id, IPieRepository repo) =>
{
    return repo.Delete(id)
        ? TypedResults.NoContent()
        : TypedResults.NotFound();
}).WithName("DeletePie");

app.Run();
