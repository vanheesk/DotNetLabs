using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PieShopApi;
using PieShopApi.Data;
using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();

builder.Services.AddSingleton(sp =>
    new SlowQueryInterceptor(
        sp.GetRequiredService<ILogger<SlowQueryInterceptor>>(),
        TimeSpan.FromMilliseconds(100)));

builder.Services.AddDbContext<PieShopDbContext>((sp, options) =>
    options.UseSqlite("Data Source=pieshop.db")
           .AddInterceptors(sp.GetRequiredService<SlowQueryInterceptor>()));

var app = builder.Build();

var pieGroup = app.MapGroup("/pies");

pieGroup.MapGet("/", async ([AsParameters] PieQuery query, PieShopDbContext db) =>
{
    IQueryable<PieEntity> pies = db.Pies.Include(p => p.Category);

    if (!string.IsNullOrEmpty(query.Filter))
        pies = pies.Where(p => p.Name.Contains(query.Filter));

    pies = query.OrderBy?.ToLower() switch
    {
        "name" => pies.OrderBy(p => p.Name),
        "price" => pies.OrderBy(p => p.Price),
        "price,desc" => pies.OrderByDescending(p => p.Price),
        _ => pies.OrderBy(p => p.PieId)
    };

    if (query.AfterId.HasValue)
        pies = pies.Where(p => p.PieId > query.AfterId.Value);

    var results = await pies
        .Take(query.PageSize)
        .Select(p => new PieDto(p.PieId, p.Name, p.ShortDescription, p.Price, p.IsPieOfTheWeek, p.Category.Name))
        .ToListAsync();

    return TypedResults.Ok(results);
})
.WithName("Pies_GetAll");

pieGroup.MapGet("/{id:int:min(1)}", async Task<Results<Ok<PieDto>, NotFound>> (int id, PieShopDbContext db) =>
{
    var pie = await PieQueries.GetById(db, id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(new PieDto(pie.PieId, pie.Name, pie.ShortDescription, pie.Price, pie.IsPieOfTheWeek, pie.Category.Name));
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
