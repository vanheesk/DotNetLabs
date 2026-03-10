using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// TODO: Add endpoints that demonstrate the running app

app.MapGet("/", () => "Lab 9: Publishing & Containers");

// TODO: Add /info endpoint returning runtime information
// app.MapGet("/info", () => new
// {
//     dotnetVersion = Environment.Version.ToString(),
//     os = RuntimeInformation.OSDescription,
//     arch = RuntimeInformation.OSArchitecture.ToString(),
//     processArch = RuntimeInformation.ProcessArchitecture.ToString(),
//     framework = RuntimeInformation.FrameworkDescription,
//     pid = Environment.ProcessId,
//     machineName = Environment.MachineName
// });

// TODO: Add /health endpoint
// app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
