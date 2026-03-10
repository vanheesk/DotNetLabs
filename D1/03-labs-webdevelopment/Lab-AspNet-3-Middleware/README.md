# Lab 3: Middleware Pipeline

## Objective

Build a **middleware pipeline** with exception handling, HTTPS redirection, static files, routing, and custom middleware. Learn how middleware ordering affects request processing and how to add cross-cutting concerns like request timing.

---

## Prerequisites

- .NET 10 SDK installed
- Completed Labs 1–2 or familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Understand the Middleware Pipeline Order

The order middleware is added determines how requests are processed. The standard order is:

1. Exception handling
2. HTTPS redirection
3. Static files
4. Routing
5. Authentication & Authorisation
6. Endpoint execution

### Tasks

1. Examine the starter `Program.cs` — note the comments indicating where each middleware should go.
2. Add the standard middleware in the correct order:
   ```csharp
   app.UseExceptionHandler("/error");
   app.UseHttpsRedirection();
   app.UseStaticFiles();
   app.UseRouting();
   ```
3. Add a test endpoint and verify the pipeline works.

---

## Exercise 2 – Custom Timing Middleware

Build custom middleware that measures how long each request takes and adds an `X-Elapsed-Ms` response header.

### Tasks

1. Create a class `TimingMiddleware` implementing `IMiddleware`:
   - Start a `Stopwatch` before calling `next(context)`
   - After `next` completes, add `X-Elapsed-Ms` header with the elapsed time
2. Register the middleware: `builder.Services.AddTransient<TimingMiddleware>();`
3. Use it: `app.UseMiddleware<TimingMiddleware>();`
4. Call any endpoint and inspect the response headers — you should see `X-Elapsed-Ms`.

---

## Exercise 3 – Short-Circuiting Middleware

Middleware can **short-circuit** the pipeline by not calling `next()`. This is useful for health checks, maintenance mode, or blocking certain paths.

### Tasks

1. Add inline middleware that returns `503 Service Unavailable` for requests to `/maintenance`:
   ```csharp
   app.Use(async (context, next) =>
   {
       if (context.Request.Path.StartsWithSegments("/maintenance"))
       {
           context.Response.StatusCode = 503;
           await context.Response.WriteAsync("Under maintenance");
           return; // short-circuit
       }
       await next(context);
   });
   ```
2. Test that `/maintenance` returns 503 while other paths still work normally.

---

## Exercise 4 – Request Logging Middleware

### Tasks

1. Create a `RequestLoggingMiddleware` that logs:
   - The HTTP method and path on entry
   - The status code and elapsed time on exit
2. Use `ILogger<RequestLoggingMiddleware>` for structured logging.
3. Register and add the middleware early in the pipeline.
4. Make several requests and observe the log output in the console.

---

## Exercise 5 – Error Handling

### Tasks

1. Add a `GET /error-test` endpoint that throws an exception on purpose.
2. Configure `app.UseExceptionHandler` to handle unhandled exceptions and return a `ProblemDetails` JSON response.
3. Verify that navigating to `/error-test` returns a proper error JSON (not a stack trace).

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed. Pay attention to middleware ordering — changing the order can cause unexpected behaviour.
