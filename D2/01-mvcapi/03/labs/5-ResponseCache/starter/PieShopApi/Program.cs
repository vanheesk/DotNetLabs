var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// TODO: Add response caching services (Part 2)
// builder.Services.AddResponseCaching();

// TODO: For Part 3, replace this with AddControllers that includes a CacheProfile
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// TODO: Add response caching middleware (Part 2)
// app.UseResponseCaching();

app.MapControllers();

app.Run();
