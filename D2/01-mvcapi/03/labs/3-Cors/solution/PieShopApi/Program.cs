using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    options.AddPolicy("AllowLocalhost8080", builder =>
        builder.WithOrigins("https://localhost:8080").AllowAnyMethod().AllowAnyHeader());

    options.AddPolicy("AllowLocalhost7282", builder =>
        builder.WithOrigins("https://localhost:7282").AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPieRepository, InMemoryPieRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost7282");
app.MapControllers();

app.Run();
