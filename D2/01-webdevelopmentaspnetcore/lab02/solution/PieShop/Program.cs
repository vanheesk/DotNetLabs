using Microsoft.EntityFrameworkCore;
using PieShop.Data;
using PieShop.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register EF Core DbContext
builder.Services.AddDbContext<PieShopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PieShopDb")));

// Register dependencies
builder.Services.AddScoped<IPieRepository, PieRepository>();

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

app.MapDefaultControllerRoute();

app.Run();
