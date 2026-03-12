using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);

// TODO: Add CORS services with three policies:
// 1. "AllowAll" - allows any origin, method, and header
// 2. "AllowLocalhost8080" - allows only https://localhost:8080
// 3. "AllowLocalhost7282" - allows only https://localhost:7282

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPieRepository, InMemoryPieRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// TODO: Add CORS middleware here with the appropriate policy
// Start with "AllowLocalhost8080" to observe the failure,
// then switch to "AllowLocalhost7282" to fix it.

app.MapControllers();

app.Run();
