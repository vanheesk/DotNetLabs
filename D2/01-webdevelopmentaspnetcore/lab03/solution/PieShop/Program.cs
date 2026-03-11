using Microsoft.EntityFrameworkCore;
using PieShop.Data;
using PieShop.Models;
using PieShop.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register EF Core DbContext
builder.Services.AddDbContext<PieShopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PieShopDb")));

// Register dependencies
builder.Services.AddScoped<IPieRepository, PieRepository>();

// Register services for URL generation
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<InviteLinkService>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PieShopDbContext>();
    DbInitializer.Seed(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Custom route with integer constraint
app.MapControllerRoute(
    name: "pieDetails",
    pattern: "pies/{id:int}",
    defaults: new { controller = "Pie", action = "Details" });

// Default conventional route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Attribute-routed API controllers
app.MapControllers();

// Minimal API route group
var api = app.MapGroup("/api");

api.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

var categoriesApi = api.MapGroup("/categories");

categoriesApi.MapGet("/", (PieShopDbContext db) =>
    Results.Ok(db.Categories.ToList()));

categoriesApi.MapGet("/{id:int}", (int id, PieShopDbContext db) =>
{
    var category = db.Categories.Find(id);
    return category is not null ? Results.Ok(category) : Results.NotFound();
});

app.Run();
