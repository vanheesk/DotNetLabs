# Lab 01: Introduction to Minimal APIs â€” Building a Pie Shop API

## Lab Overview

In Module 2.1, you built the **Pie Shop** as a server-rendered MVC application. You even added a few Minimal API endpoints alongside MVC in Lab 03. Now it is time to build a **dedicated Pie Shop API** from scratch using Minimal APIs â€” the modern, lightweight approach to building HTTP APIs in ASP.NET Core.

This lab is designed as a foundation. In later labs, we will extend the same project with:

- Full CRUD with typed responses
- Filters and middleware
- OpenAPI documentation
- Authentication & authorization

## Learning Objectives

By the end of this lab, you will be able to:

- Explain what Minimal APIs are and when to use them
- Create a Minimal API project using Visual Studio or the dotnet CLI
- Define HTTP endpoints directly in `Program.cs`
- Use dependency injection per endpoint
- Enable and observe built-in validation in .NET 10
- Organize endpoints using route groups

## Prerequisites

- .NET 10 SDK installed
- Visual Studio 2022+ or .NET CLI
- Completed Module 2.1 (conceptual understanding of ASP.NET Core and the Pie Shop domain)

> **Starter solutions available.** Each part has a matching starter project in the `lab01/` folder.
> If you get stuck, copy the relevant folder and continue from there.
> For example, to start Part 5 from a known-good state:
> ```shell
> cp -r lab01/part04-complete/PieShopApi .
> cd PieShopApi && dotnet run
> ```

---

## Part 1 â€” What Are Minimal APIs?

Minimal APIs are a lightweight HTTP stack introduced in .NET 6 that allow you to define API endpoints without controllers, attributes, or boilerplate.

Key characteristics:

- Explicit routing via `MapGet`, `MapPost`, etc.
- Full middleware and DI support
- Best path for Native AOT Web APIs

In Module 2.1 Lab 03 you added a few Minimal API endpoints to the MVC Pie Shop. In this lab, you will build a **standalone** Pie Shop API using only Minimal APIs.

---

## Part 2 â€” Create a Minimal API Project

### Option A: Visual Studio

1. Create a new project
2. Select **ASP.NET Core Web API**
3. Target **.NET 10**
4. Check **Do not use controllers**

### Option B: .NET CLI

```shell
dotnet new webapi --use-minimal-apis -n PieShopApi
cd PieShopApi
dotnet run
```

> Verify the app runs and returns a response on the default endpoint.

---

## Part 3 â€” Understanding Program.cs

Open `Program.cs`. You should see:

1. Application host creation
2. Dependency registration
3. Middleware setup
4. Endpoint mapping
5. `app.Run()`

---

## Part 4 â€” Your First Minimal API Endpoint

> **Falling behind?** Start from `lab01/start/PieShopApi/`.

Add a simple endpoint:

```csharp
app.MapGet("/ping", () => "pong");
```

Run the app and browse to:

```
GET /ping
```

> You have just defined an API endpoint without controllers or attributes.

---

## Part 5 â€” Dependency Injection per Endpoint

> **Falling behind?** Start from `lab01/part04-complete/PieShopApi/`.

Minimal APIs allow you to inject only what an endpoint needs.

```csharp
app.MapGet("/time", (ILogger<Program> logger) =>
{
    logger.LogInformation("Time endpoint called");
    return DateTime.UtcNow;
});
```

- No constructor injection required
- No unused dependencies
- Easier testing

This is one of the key differences compared to controller-based APIs.

---

## Part 6 â€” Adding a POST Endpoint with Validation (.NET 10)

> **Falling behind?** Start from `lab01/part05-complete/PieShopApi/`.

### Enable Validation

In `Program.cs`:

```csharp
builder.Services.AddValidation();
```

### Define a Request Model

```csharp
public sealed record CreatePieRequest(
    [Required, StringLength(100, MinimumLength = 3)] string Name,
    [StringLength(200)] string? ShortDescription,
    [Range(0.01, 1000)] decimal Price);
```

### Map the Endpoint

```csharp
app.MapPost("/pies", (CreatePieRequest request) =>
{
    return Results.Created($"/pies/{Guid.NewGuid()}", request);
});
```

### Try Invalid Input

Send invalid JSON (e.g. missing `Name`).

- The request fails before the handler runs
- HTTP 400 with `ProblemDetails` is returned automatically

### Experiment

1. Remove `AddValidation()`
2. Repeat the request
3. Observe the difference

---

## Part 7 â€” Organizing Endpoints with Route Groups

> **Falling behind?** Start from `lab01/part06-complete/PieShopApi/`.

As APIs grow, a single-file approach becomes harder to maintain.

Introduce a route group:

```csharp
var pies = app.MapGroup("/pies");

pies.MapGet("/", () => "List pies");
pies.MapPost("/", (CreatePieRequest request) => Results.Ok(request));
```

- Clear structure
- Shared metadata
- Easier extension later

---

## Part 8 â€” Native AOT Awareness (Conceptual)

Minimal APIs are the preferred Web API model for Native AOT in ASP.NET Core.

- Smaller executables
- Faster cold starts
- Some dynamic features are restricted

> We are not enabling AOT yet. This will be covered in a later lab.

---

## Lab Checkpoint

By now, you should have:

- A running Pie Shop Minimal API
- Multiple endpoints (GET + POST)
- Validation enabled
- Route groups in place
- A clean, understandable `Program.cs`

---

## Reflection Questions

1. How do Minimal APIs compare to the MVC controllers you built in Module 2.1?
2. When would you choose Minimal APIs over controllers?
3. How does per-endpoint DI affect testability?

---

## Lab Complete

You have completed the **Introduction to Minimal APIs** lab.

> **Reference solution:** `lab01/part07-complete/PieShopApi/` contains the final state of this lab.

In the next lab, we will build on this project to create a full CRUD Pie Shop API.