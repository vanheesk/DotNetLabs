using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Lab 9: Publishing & Containers");

// ----- Exercise 1-3: Info endpoint showing runtime details -----
app.MapGet("/info", () => new
{
    dotnetVersion = Environment.Version.ToString(),
    os = RuntimeInformation.OSDescription,
    arch = RuntimeInformation.OSArchitecture.ToString(),
    processArch = RuntimeInformation.ProcessArchitecture.ToString(),
    framework = RuntimeInformation.FrameworkDescription,
    pid = Environment.ProcessId,
    machineName = Environment.MachineName,
    startTime = DateTime.UtcNow
}).WithTags("Info")
  .WithName("GetRuntimeInfo")
  .WithSummary("Returns runtime and environment information");

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }))
    .WithTags("Health")
    .WithName("HealthCheck");

// Endpoint to help compare publish sizes
app.MapGet("/publish-notes", () => new
{
    frameworkDependent = "dotnet publish -c Release -o out/fdd",
    selfContained = "dotnet publish -c Release --self-contained -o out/sc",
    trimmed = "dotnet publish -c Release --self-contained -p:PublishTrimmed=true -o out/sc-trimmed",
    container = "dotnet publish -c Release /t:PublishContainer",
    nativeAot = "dotnet publish -c Release -o out/aot  (requires PublishAot=true in csproj)"
}).WithTags("Info")
  .WithName("PublishNotes")
  .WithSummary("Shows the various publish commands for reference");

app.Run();
