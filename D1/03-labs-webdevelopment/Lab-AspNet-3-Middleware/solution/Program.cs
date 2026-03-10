using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<TimingMiddleware>();
builder.Services.AddTransient<RequestLoggingMiddleware>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// ----- Exercise 1 & 5: Standard middleware pipeline -----
app.UseExceptionHandler(exApp =>
{
    exApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = 500;
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var problem = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            title = "An unexpected error occurred",
            status = 500,
            detail = feature?.Error.Message ?? "Unknown error"
        };
        await context.Response.WriteAsJsonAsync(problem);
    });
});

// ----- Exercise 4: Request Logging -----
app.UseMiddleware<RequestLoggingMiddleware>();

// ----- Exercise 2: Timing Middleware -----
app.UseMiddleware<TimingMiddleware>();

// ----- Exercise 3: Short-Circuiting Middleware -----
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/maintenance"))
    {
        context.Response.StatusCode = 503;
        await context.Response.WriteAsync("Under maintenance — please try again later.");
        return; // short-circuit
    }
    await next(context);
});

app.UseRouting();

// ----- Endpoints -----
app.MapGet("/", () => "Hello from Lab 3!");

app.MapGet("/slow", async () =>
{
    await Task.Delay(500);
    return "This endpoint took a while...";
});

// ----- Exercise 5: Error endpoint -----
app.MapGet("/error-test", () =>
{
    throw new InvalidOperationException("This is a test exception!");
});

app.Run();

// ----- Exercise 2: Timing Middleware -----
public class TimingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var sw = Stopwatch.StartNew();
        await next(context);
        sw.Stop();
        context.Response.Headers["X-Elapsed-Ms"] = sw.ElapsedMilliseconds.ToString();
    }
}

// ----- Exercise 4: Request Logging Middleware -----
public class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        logger.LogInformation("→ {Method} {Path}", method, path);

        var sw = Stopwatch.StartNew();
        await next(context);
        sw.Stop();

        logger.LogInformation("← {Method} {Path} responded {StatusCode} in {Elapsed}ms",
            method, path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
