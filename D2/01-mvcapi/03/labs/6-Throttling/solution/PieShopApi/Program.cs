using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "myWindowLimiter", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(60);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

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
app.UseRateLimiter();
app.MapControllers();

app.Run();
