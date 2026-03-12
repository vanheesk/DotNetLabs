using PieShopApi.Formatters;
using PieShopApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
    options.OutputFormatters.Add(new PieCsvFormatter());
}).AddXmlDataContractSerializerFormatters();
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
