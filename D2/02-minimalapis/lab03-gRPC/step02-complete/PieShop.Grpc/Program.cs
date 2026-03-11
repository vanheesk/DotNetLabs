using Microsoft.EntityFrameworkCore;
using PieShop.Grpc.Services;
using PieShopApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddDbContext<PieShopDbContext>(options =>
    options.UseSqlite("Data Source=../PieShopApi/pieshop.db"));

var app = builder.Build();

app.MapGrpcService<PieCatalogServiceImpl>();
app.MapGrpcReflectionService();

app.Run();
