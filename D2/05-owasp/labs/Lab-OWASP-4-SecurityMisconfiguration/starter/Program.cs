var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO (Exercise 1): Register ProblemDetails
// builder.Services.AddProblemDetails();

// ⚠️ VULNERABLE (Exercise 3): Overly permissive CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Swagger available in ALL environments (Exercise 4: restrict to dev only)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

// TODO (Exercise 1): Add exception handler and status code pages
// app.UseExceptionHandler();
// app.UseStatusCodePages();

// TODO (Exercise 2): Add security headers middleware
// app.Use(async (context, next) => { ... });

// =====================================================
// EXERCISE 1: Information Leakage
// =====================================================

// This endpoint throws an exception — observe the raw stack trace in the response
app.MapGet("/error-test", () =>
{
    throw new InvalidOperationException(
        "Database connection string: Server=prod-db;User=admin;Password=s3cret!");
})
.WithName("ErrorTest")
.WithSummary("Throws an exception (VULNERABLE — leaks details)")
.WithTags("Misconfiguration");

// =====================================================
// EXERCISE 3: CORS
// =====================================================

app.MapGet("/api/data", () => new { message = "This is sensitive data", secret = "abc123" })
    .WithName("GetData")
    .WithSummary("Returns data (check CORS headers)")
    .WithTags("API");

// =====================================================
// EXERCISE 4: Debug endpoint exposed in production
// =====================================================

// ⚠️ VULNERABLE: This should only be available in Development
app.MapGet("/debug/config", (IConfiguration config) =>
    config.AsEnumerable()
          .Where(kvp => kvp.Value is not null)
          .Select(kvp => new { kvp.Key, kvp.Value }))
    .WithName("DebugConfig")
    .WithSummary("Show app configuration (VULNERABLE — exposed in production)")
    .WithTags("Debug");

app.MapGet("/", () => "Lab OWASP-4: Security Misconfiguration (A05:2021)")
    .ExcludeFromDescription();

app.Run();
