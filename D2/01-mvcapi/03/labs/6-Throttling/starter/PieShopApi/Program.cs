using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// TODO: Add a rate limiter with a fixed window policy named "myWindowLimiter"
// Configure: PermitLimit = 4, Window = 60 seconds,
//            QueueProcessingOrder = OldestFirst, QueueLimit = 2
// Hint: use builder.Services.AddRateLimiter(...) with .AddFixedWindowLimiter(...)
// Don't forget the using statements: System.Threading.RateLimiting and Microsoft.AspNetCore.RateLimiting

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPieRepository, InMemoryPieRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// TODO: Add the rate limiter middleware here
// Hint: app.UseRateLimiter();

app.MapControllers();

app.Run();
