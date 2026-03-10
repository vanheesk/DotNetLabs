using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// TODO (Exercise 2): Register TimingMiddleware
// builder.Services.AddTransient<TimingMiddleware>();

var app = builder.Build();

// ----- Exercise 1: Standard middleware pipeline -----
// TODO: Add middleware in the correct order:
// app.UseExceptionHandler(...)
// app.UseHttpsRedirection();
// app.UseStaticFiles();  // needs wwwroot folder to exist
// app.UseRouting();

// ----- Exercise 2: Custom Timing Middleware -----
// TODO: Use the timing middleware
// app.UseMiddleware<TimingMiddleware>();

// ----- Exercise 3: Short-Circuiting Middleware -----
// TODO: Add inline middleware that returns 503 for "/maintenance"

// ----- Exercise 4: Request Logging Middleware -----
// TODO: Register and use RequestLoggingMiddleware

// ----- Endpoints -----
app.MapGet("/", () => "Hello from Lab 3!");

// ----- Exercise 5: Error Handling -----
// TODO: Add a GET "/error-test" endpoint that throws an exception

app.Run();

// ----- Exercise 2: TimingMiddleware class -----
// TODO: Create a TimingMiddleware class that implements IMiddleware
// public class TimingMiddleware : IMiddleware
// {
//     public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//     {
//         var sw = Stopwatch.StartNew();
//         await next(context);
//         sw.Stop();
//         context.Response.Headers["X-Elapsed-Ms"] = sw.ElapsedMilliseconds.ToString();
//     }
// }

// ----- Exercise 4: RequestLoggingMiddleware class -----
// TODO: Create a RequestLoggingMiddleware class that logs method, path, and status code
