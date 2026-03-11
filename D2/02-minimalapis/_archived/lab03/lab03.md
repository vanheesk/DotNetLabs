# Lab 03: Advanced Pie Shop API Features

## Context â€” Building on Lab 02

In [Lab 02](../lab02/lab02.md), you built a **Pie Shop API** with:

- Full CRUD endpoints for `/pies` with proper HTTP verbs
- Route parameters and constraints (`{id:int:min(1)}`)
- Query string filtering and paging
- Endpoint groups via `MapGroup`
- Typed responses with `TypedResults` (including `CreatedAtRoute`)
- Validated DTOs using `AddValidation()` and data annotations
- Dependency injection via `IPieRepository`

In this lab, you will extend that same API with **production-grade features**:

- Custom middleware and pipeline ordering
- Endpoint filters (inline and reusable)
- Response customization (streams, files, custom results)
- Standardized error handling with `ProblemDetails`
- OpenAPI generation and endpoint metadata
- JWT-based authentication and authorization

> **Goal:** By the end of this lab, your Pie Shop API will be a properly secured, documented, and observable production-ready API.

---

## Lab Setup

### Prerequisites

- .NET 10 SDK installed
- **Completed [Lab 02 â€” Building a Production-Ready Pie Shop API](../lab02/lab02.md)**
- Your PieShopApi project with working `/pies` CRUD endpoints

### Starting Point

Open the PieShopApi project from Lab 02. Verify it still runs:

```shell
dotnet run
```

> All steps in this lab add to the existing project. Do not create a new project.

> **Starter solutions available.** Each step has a matching starter project in the `lab03/` folder.
> If you get stuck, copy the relevant folder and continue from there.
> For example, to start Step 3 from a known-good state:
> ```shell
> cp -r lab03/step02-complete/PieShopApi .
> cd PieShopApi && dotnet run
> ```

---

## Lab Structure Overview

| Step | Focus Area                          |
| ---- | ----------------------------------- |
| 1    | Middleware & pipeline ordering       |
| 2    | Endpoint filters (inline + reusable) |
| 3    | Response customization              |
| 4    | Error handling & ProblemDetails     |
| 5    | OpenAPI generation & metadata       |
| 6    | Authentication & authorization      |

---

## Step 1 â€” Middleware Fundamentals & Pipeline Ordering

> **Falling behind?** Start from `lab03/start/PieShopApi/`.

Middleware are pipeline building blocks that run on **every request**. They handle cross-cutting concerns like logging, error handling, and authentication. The order in which you register middleware determines when each piece runs.

### Task 1a â€” Add Request Logging Middleware

Add inline middleware that logs the HTTP method and path at the start of a request, and the status code at the end:

```csharp
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("-> {Method} {Path}", context.Request.Method, context.Request.Path);

    await next(context);

    logger.LogInformation("<- {StatusCode}", context.Response.StatusCode);
});
```

### Task 1b â€” Add a Response Mutation

Append a custom response header to every response:

```csharp
app.Use(async (context, next) =>
{
    await next(context);
    context.Response.Headers.Append("X-PieShop-Api", "v1");
});
```

### Task 1c â€” Understand Middleware Ordering

The recommended ordering for API middleware is:

1. Exception handling (earliest)
2. CORS
3. Authentication
4. Authorization
5. Your custom middleware
6. Endpoint mapping

> Experiment: move your logging middleware before and after `UseAuthorization()`. Observe how the behavior changes.

### Stretch Goal

Refactor your inline logging middleware into a reusable middleware class:

```csharp
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("-> {Method} {Path}", context.Request.Method, context.Request.Path);
        await _next(context);
        _logger.LogInformation("<- {StatusCode}", context.Response.StatusCode);
    }
}

// Register it:
app.UseMiddleware<RequestLoggingMiddleware>();
```

> **Checkpoint:** Every request/response is logged. You can explain why middleware ordering matters.

---

## Step 2 â€” Endpoint Filters

> **Falling behind?** Start from `lab03/step01-complete/PieShopApi/`.

Endpoint filters are like middleware, but they are **attached to specific endpoints or groups** and run **after routing**. Unlike middleware, filters can access handler arguments directly.

### Task 2a â€” Add an Inline Filter

Add an inline endpoint filter to the `GET /pies` endpoint that logs paging parameters:

```csharp
pieGroup.MapGet("/", ([AsParameters] PieQuery query, IPieRepository repo) =>
{
    // ... your existing handler
})
.AddEndpointFilter(async (context, next) =>
{
    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
    var query = context.GetArgument<PieQuery>(0);
    logger.LogInformation("Paging: page={Page}, size={Size}", query.Page, query.PageSize);

    return await next(context);
});
```

### Task 2b â€” Create a Reusable Filter

Convert the inline filter into a class that implements `IEndpointFilter`:

```csharp
public class LoggingEndpointFilter(ILogger<LoggingEndpointFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        logger.LogInformation("Filter executing for {Method} {Path}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path);

        var result = await next(context);

        logger.LogInformation("Filter completed");
        return result;
    }
}
```

### Task 2c â€” Apply Filter to a Route Group

Apply your reusable filter to the entire `/pies` group so it runs for all pie endpoints:

```csharp
var pieGroup = app.MapGroup("/pies")
    .AddEndpointFilter<LoggingEndpointFilter>();
```

### Key Concept: FIFO / FILO

When stacking multiple filters:

- **Before** `next()` â€” runs in registration order (FIFO)
- **After** `next()` â€” runs in reverse order (FILO)

> Experiment: add two filters with distinct log markers and verify the ordering.

> **Checkpoint:** Filters see handler parameters. You can demonstrate FIFO/FILO ordering with stacked filters.

---

## Step 3 â€” Response Customization

> **Falling behind?** Start from `lab03/step02-complete/PieShopApi/`.

So far your API only returns JSON. Minimal APIs can also return streams, files, and custom content types.

### Task 3a â€” Return a File/Stream

Add an endpoint that exports all pies as CSV:

```csharp
pieGroup.MapGet("/export", (IPieRepository repo) =>
{
    var csv = new StringBuilder("PieId,Name,ShortDescription,Price,IsPieOfTheWeek,CategoryId\n");
    foreach (var pie in repo.GetAll())
    {
        csv.AppendLine($"{pie.PieId},{pie.Name},{pie.ShortDescription},{pie.Price},{pie.IsPieOfTheWeek},{pie.CategoryId}");
    }
    var bytes = Encoding.UTF8.GetBytes(csv.ToString());
    return Results.File(bytes, "text/csv", "pies.csv");
});
```

### Task 3b â€” Create a Custom `IResult`

Build a custom result that returns XML:

```csharp
using System.Runtime.Serialization;

public class XmlResult<T>(T value, int statusCode = 200) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/xml";

        var serializer = new DataContractSerializer(typeof(T));
        await using var ms = new MemoryStream();
        serializer.WriteObject(ms, value);
        ms.Position = 0;
        await ms.CopyToAsync(httpContext.Response.Body);
    }
}
```

Use it in an endpoint:

```csharp
pieGroup.MapGet("/{id:int:min(1)}/xml", (int id, IPieRepository repo) =>
{
    var pie = repo.GetById(id);
    return pie is null
        ? Results.NotFound()
        : new XmlResult<Pie>(pie);
});
```

> **Checkpoint:** Your API can return CSV downloads and XML responses alongside JSON.

---

## Step 4 â€” Error Handling & ProblemDetails

> **Falling behind?** Start from `lab03/step03-complete/PieShopApi/`.

Unhandled exceptions should return standardized error responses using the [RFC 9457](https://www.rfc-editor.org/rfc/rfc9457) `application/problem+json` format.

### Task 4a â€” Register ProblemDetails Services

```csharp
builder.Services.AddProblemDetails();
```

### Task 4b â€” Enable Exception Handling Middleware

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}
```

### Task 4c â€” Create a Failing Endpoint

Add a test endpoint to verify error handling:

```csharp
app.MapGet("/boom", () =>
{
    throw new InvalidOperationException("Something went wrong!");
});
```

Test the endpoint:

- In **Development**: you will see the developer exception page with full stack trace
- In **Production** (`DOTNET_ENVIRONMENT=Production`): you will see a `ProblemDetails` JSON response

### Using `Results.Problem` Explicitly

You can also return problem details explicitly from your handlers:

```csharp
return Results.Problem(
    title: "Pie not found",
    statusCode: 404,
    detail: $"No pie with ID {id} exists.");
```

> **Checkpoint:** Unhandled exceptions produce standardized `ProblemDetails` responses. No raw stack traces leak in production.

---

## Step 5 â€” OpenAPI Generation & Endpoint Metadata

> **Falling behind?** Start from `lab03/step04-complete/PieShopApi/`.

OpenAPI provides a machine-readable specification of your API. .NET 10 ships with built-in OpenAPI document generation.

### Task 5a â€” Expose the OpenAPI Document

```csharp
builder.Services.AddOpenApi();

// After building the app:
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
```

Run the app and browse to:

```
GET /openapi/v1.json
```

### Task 5b â€” Add Endpoint Metadata

Decorate your endpoints with descriptive metadata:

```csharp
pieGroup.MapGet("/{id:int:min(1)}", (int id, IPieRepository repo) =>
{
    // ...
})
.WithName("Pies_GetById")
.WithTags("Pies")
.WithSummary("Get a pie by ID")
.WithDescription("Returns a single pie or 404 if not found.");
```

### Task 5c â€” Describe Response Types

Annotate endpoints with their possible response types:

```csharp
pieGroup.MapGet("/{id:int:min(1)}", ...)
    .Produces<Pie>(200)
    .ProducesProblem(404);

pieGroup.MapPost("/", ...)
    .Produces<Pie>(201)
    .ProducesValidationProblem(400);
```

> **Checkpoint:** `/openapi/v1.json` includes tags, operation IDs, summaries, and response type metadata.

---

## Step 6 â€” Authentication & Authorization

> **Falling behind?** Start from `lab03/step05-complete/PieShopApi/`.

Secure your API using **JWT Bearer tokens** and **policy-based authorization**.

### Task 6a â€” Add JWT Bearer Authentication

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();
```

Enable the middleware (**order matters** â€” authentication before authorization):

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

### Task 6b â€” Define an Authorization Policy

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("pies.write", policy =>
        policy.RequireClaim("scope", "pies.write"));
```

### Task 6c â€” Protect Endpoints

Apply authorization to write operations:

```csharp
pieGroup.MapPost("/", handler).RequireAuthorization("pies.write");
pieGroup.MapPut("/{id:int:min(1)}", handler).RequireAuthorization("pies.write");
pieGroup.MapDelete("/{id:int:min(1)}", handler).RequireAuthorization("pies.write");
```

Keep read endpoints publicly accessible:

```csharp
pieGroup.MapGet("/", handler).AllowAnonymous();
pieGroup.MapGet("/{id:int:min(1)}", handler).AllowAnonymous();
```

### Task 6d â€” Test Locally with `dotnet user-jwts`

Generate a test token:

```shell
dotnet user-jwts create --role Admin --scope pies.write
```

Use the generated token in your requests:

```shell
curl -H "Authorization: Bearer <token>" https://localhost:<port>/pies -X POST -d '...'
```

Verify:

- **Without token** â€” 401 Unauthorized
- **With valid token but missing scope** â€” 403 Forbidden
- **With correct token and scope** â€” request succeeds

> **Checkpoint:** Write endpoints are protected. Read endpoints are publicly accessible. Tokens are testable locally.

---

## Reflection Questions

1. What is the difference between middleware and endpoint filters? When would you use each?
2. How does middleware ordering affect authentication and error handling?
3. When would you return a custom `IResult` instead of JSON?
4. Why is `ProblemDetails` important for API consumers?
5. What happens if you apply `RequireAuthorization` to a group but forget `UseAuthentication()`?

---

## Stretch Goals

These are optional challenges that tie together multiple concepts from this lab:

1. **Validation filter** â€” Create an endpoint filter that returns `Results.Problem(...)` when input is invalid (combines filters + ProblemDetails).
2. **FIFO/FILO demo** â€” Stack multiple filters with distinct log markers and document the observed ordering.
3. **OpenAPI for error responses** â€” Add `ProducesProblem()` and `ProducesValidationProblem()` annotations to all relevant endpoints.

---

## Lab Complete

You have completed the **Advanced Pie Shop API Features** lab.

> **Reference solution:** `lab03/step06-complete/PieShopApi/` contains the final state of this lab.

By now your Pie Shop API should have:

- Custom middleware with correct pipeline ordering
- Inline and reusable endpoint filters applied to route groups
- CSV export and custom XML result support
- Standardized error handling with `ProblemDetails`
- OpenAPI documentation with endpoint metadata and response declarations
- JWT-based authentication with policy authorization, testable via `dotnet user-jwts`

In Module 2.5, you will use security labs to identify and fix OWASP vulnerabilities in the Pie Shop API.