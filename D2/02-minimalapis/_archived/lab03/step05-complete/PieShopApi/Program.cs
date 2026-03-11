using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using PieShopApi;
using PieShopApi.Filters;
using PieShopApi.Middleware;
using PieShopApi.Models;
using PieShopApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPieRepository, InMemoryPieRepository>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.Use(async (context, next) =>
{
    await next(context);
    context.Response.Headers.Append("X-PieShop-Api", "v1");
});

app.MapGet("/boom", () =>
{
    throw new InvalidOperationException("Something went wrong!");
});

var pieGroup = app.MapGroup("/pies")
    .AddEndpointFilter<LoggingEndpointFilter>()
    .WithTags("Pies");

pieGroup.MapGet("/", ([AsParameters] PieQuery query, IPieRepository repo) =>
{
    var result = repo.GetAll();

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
})
.WithName("Pies_GetAll")
.WithSummary("List pies with optional filtering and paging");

pieGroup.MapGet("/{id:int:min(1)}", Results<Ok<Pie>, NotFound> (int id, IPieRepository repo) =>
{
    var pie = repo.GetById(id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(pie);
})
.WithName("Pies_GetById")
.WithSummary("Get a pie by ID")
.WithDescription("Returns a single pie or 404 if not found.")
.Produces<Pie>(200)
.ProducesProblem(404);

pieGroup.MapGet("/export", (IPieRepository repo) =>
{
    var csv = new StringBuilder("PieId,Name,ShortDescription,Price,IsPieOfTheWeek,CategoryId\n");
    foreach (var pie in repo.GetAll())
    {
        csv.AppendLine($"{pie.PieId},{pie.Name},{pie.ShortDescription},{pie.Price},{pie.IsPieOfTheWeek},{pie.CategoryId}");
    }
    var bytes = Encoding.UTF8.GetBytes(csv.ToString());
    return Results.File(bytes, "text/csv", "pies.csv");
})
.WithName("Pies_Export")
.WithSummary("Export all pies as CSV");

pieGroup.MapGet("/{id:int:min(1)}/xml", (int id, IPieRepository repo) =>
{
    var pie = repo.GetById(id);
    return pie is null
        ? Results.NotFound()
        : new XmlResult<Pie>(pie);
})
.WithName("Pies_GetByIdXml")
.WithSummary("Get a pie by ID as XML");

pieGroup.MapPost("/", (CreatePieRequest request, IPieRepository repo) =>
{
    var pie = new Pie(0, request.Name, request.ShortDescription, request.Price, request.IsPieOfTheWeek, request.CategoryId);
    pie = repo.Add(pie);
    return TypedResults.CreatedAtRoute(pie, "Pies_GetById", new { id = pie.PieId });
})
.WithName("Pies_Create")
.WithSummary("Create a new pie")
.Produces<Pie>(201)
.ProducesValidationProblem(400);

pieGroup.MapPut("/{id:int:min(1)}", Results<Ok<Pie>, NotFound> (int id, UpdatePieRequest request, IPieRepository repo) =>
{
    var updated = new Pie(id, request.Name, request.ShortDescription, request.Price, request.IsPieOfTheWeek, request.CategoryId);
    var result = repo.Update(id, updated);
    return result is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(result);
})
.WithName("Pies_Update")
.WithSummary("Update an existing pie");

pieGroup.MapDelete("/{id:int:min(1)}", Results<NoContent, NotFound> (int id, IPieRepository repo) =>
{
    return repo.Delete(id)
        ? TypedResults.NoContent()
        : TypedResults.NotFound();
})
.WithName("Pies_Delete")
.WithSummary("Delete a pie");

app.Run();