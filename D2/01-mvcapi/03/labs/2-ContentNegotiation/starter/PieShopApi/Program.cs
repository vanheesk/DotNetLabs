using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);

// TODO: Configure content negotiation here
// 1. Enable RespectBrowserAcceptHeader
// 2. Add XML formatters with .AddXmlDataContractSerializerFormatters()
// 3. Enable ReturnHttpNotAcceptable
// 4. Add the custom PieCsvFormatter
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPieRepository, InMemoryPieRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
