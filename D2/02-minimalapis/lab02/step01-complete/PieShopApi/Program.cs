using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PieShopApi.Data;
using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddDbContext<PieShopDbContext>(options =>
    options.UseSqlite("Data Source=pieshop.db"));
var app = builder.Build();

var pieGroup = app.MapGroup("/pies");

pieGroup.MapGet("/", async ([AsParameters] PieQuery query, PieShopDbContext db) =>
{
    IQueryable<PieEntity> pies = db.Pies;

    if (!string.IsNullOrEmpty(query.Filter))
        pies = pies.Where(p => p.Name.Contains(query.Filter));

    if (!string.IsNullOrEmpty(query.OrderBy))
    {
        pies = query.OrderBy.ToLower() switch
        {
            "name" => pies.OrderBy(p => p.Name),
            "price" => pies.OrderBy(p => p.Price),
            "price,desc" => pies.OrderByDescending(p => p.Price),
            _ => pies
        };
    }

    var results = await pies
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync();

    return TypedResults.Ok(results);
})
.WithName("Pies_GetAll");

pieGroup.MapGet("/{id:int:min(1)}", async Task<Results<Ok<PieEntity>, NotFound>> (int id, PieShopDbContext db) =>
{
    var pie = await db.Pies.FindAsync(id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(pie);
})
.WithName("Pies_GetById");

pieGroup.MapPost("/", async (CreatePieRequest request, PieShopDbContext db) =>
{
    var entity = new PieEntity
    {
        Name = request.Name,
        ShortDescription = request.ShortDescription,
        Price = request.Price,
        IsPieOfTheWeek = request.IsPieOfTheWeek,
        CategoryId = request.CategoryId
    };
    db.Pies.Add(entity);
    await db.SaveChangesAsync();
    return TypedResults.CreatedAtRoute(entity, "Pies_GetById", new { id = entity.PieId });
})
.WithName("Pies_Create");

pieGroup.MapPut("/{id:int:min(1)}", async Task<Results<Ok<PieEntity>, NotFound>> (int id, UpdatePieRequest request, PieShopDbContext db) =>
{
    var entity = await db.Pies.FindAsync(id);
    if (entity is null) return TypedResults.NotFound();

    entity.Name = request.Name;
    entity.ShortDescription = request.ShortDescription;
    entity.Price = request.Price;
    entity.IsPieOfTheWeek = request.IsPieOfTheWeek;
    entity.CategoryId = request.CategoryId;
    await db.SaveChangesAsync();

    return TypedResults.Ok(entity);
})
.WithName("Pies_Update");

pieGroup.MapDelete("/{id:int:min(1)}", async Task<Results<NoContent, NotFound>> (int id, PieShopDbContext db) =>
{
    var entity = await db.Pies.FindAsync(id);
    if (entity is null) return TypedResults.NotFound();

    db.Pies.Remove(entity);
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
})
.WithName("Pies_Delete");

app.Run();
