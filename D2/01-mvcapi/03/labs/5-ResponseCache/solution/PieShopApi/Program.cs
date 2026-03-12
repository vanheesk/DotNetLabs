var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Cache2Minutes", new Microsoft.AspNetCore.Mvc.CacheProfile()
    {
        Duration = 120,
        Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any
    });
});

builder.Services.AddResponseCaching();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();
