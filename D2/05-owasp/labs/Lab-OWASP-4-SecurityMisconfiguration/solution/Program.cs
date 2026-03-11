var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Exercise 1: ProblemDetails for standardised error responses
builder.Services.AddProblemDetails();

// Exercise 3: Restrictive CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://trusted-app.example.com")
              .WithMethods("GET", "POST")
              .WithHeaders("Content-Type", "Authorization");
    });

    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Exercise 4: Only expose Swagger and debug endpoints in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("Development");
}
else
{
    app.UseCors();
}

// Exercise 1: Global exception handler — returns ProblemDetails without stack traces
app.UseExceptionHandler();
app.UseStatusCodePages();

// Exercise 2: Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "0");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    await next();
});

// =====================================================
// Error test endpoint — now returns ProblemDetails
// =====================================================

app.MapGet("/error-test", () =>
{
    throw new InvalidOperationException("Internal error occurred");
})
.WithName("ErrorTest")
.WithSummary("Throws an exception — returns ProblemDetails (SAFE)")
.WithTags("Misconfiguration");

// =====================================================
// API endpoint with proper CORS
// =====================================================

app.MapGet("/api/data", () => new { message = "This is data" })
    .WithName("GetData")
    .WithSummary("Returns data with proper CORS and security headers")
    .WithTags("API");

// =====================================================
// Exercise 4: Debug endpoint gated by environment
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug/config", (IConfiguration config) =>
        config.AsEnumerable()
              .Where(kvp => kvp.Value is not null)
              .Select(kvp => new { kvp.Key, kvp.Value }))
        .WithName("DebugConfig")
        .WithSummary("Show app configuration (dev only)")
        .WithTags("Debug");
}

app.MapGet("/", () => "Lab OWASP-4: Security Misconfiguration (A05:2021)")
    .ExcludeFromDescription();

app.Run();
