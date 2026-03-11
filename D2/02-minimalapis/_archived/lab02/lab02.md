# Lab 02: Building a Production-Ready Pie Shop API

## Context â€” Building on Lab 01

In [Lab 01](../lab01/lab01.md), you:

- Created your first Minimal API project (PieShopApi)
- Defined basic endpoints (`/ping`, `/time`, `/pies`) in `Program.cs`
- Used dependency injection per endpoint
- Enabled .NET 10 built-in validation with `AddValidation()`
- Organized endpoints using `MapGroup`

In this lab, you will take those same techniques and apply them to the **Pie Shop domain**. You will build a full CRUD API for managing pies â€” the same pies you worked with in the MVC Pie Shop from Module 2.1, but now exposed as a clean REST API.

You will introduce:

- Full CRUD endpoints with proper HTTP verbs
- Route parameters and constraints
- Query strings for filtering and paging
- Typed responses with `TypedResults`
- Dedicated DTOs with validation
- Dependency injection with repository services

> **Goal:** By the end of this lab, you will have a clean, structured Pie Shop API that is ready to be extended with middleware, filters, OpenAPI, and security in the next lab.

---

## Lab Setup

### Prerequisites

- .NET 10 SDK installed
- Basic familiarity with C#
- **Completed [Lab 01 â€” Introduction to Minimal APIs](../lab01/lab01.md)**

### Starting Point

Continue with the project you created in Lab 01, or create a fresh one:

```shell
dotnet new webapi -n PieShopApi
cd PieShopApi
dotnet run
```

> If you are reusing your Lab 01 project, you can remove the `/ping`, `/time`, and initial `/pies` endpoints â€” we will replace them with full CRUD.

> **Starter solutions available.** Each step has a matching starter project in the `lab02/` folder.
> If you get stuck, copy the relevant folder and continue from there.
> For example, to start Step 3 from a known-good state:
> ```shell
> cp -r lab02/step02-complete/PieShopApi .
> cd PieShopApi && dotnet run
> ```

---

## Domain Scenario

You are building a **Pie Shop API** that manages the same domain from Module 2.1:

- **Pies** â€” the main catalog resource (name, description, price, category)
- **Categories** â€” grouping pies (Fruit pies, Cheese cakes, etc.)

This API will be expanded in the next lab with:

- Middleware and endpoint filters
- OpenAPI documentation
- Authentication & authorization

---

## Lab Structure Overview

| Step | Focus Area                     |
| ---- | ------------------------------ |
| 1    | Endpoint design & HTTP verbs   |
| 2    | Route parameters & constraints |
| 3    | Query strings & filtering      |
| 4    | Grouping endpoints             |
| 5    | Typed responses                |
| 6    | Model binding & validation     |
| 7    | Dependency Injection           |

---

## Step 1 â€” Define Core CRUD Endpoints

> **Falling behind?** Start from `lab02/start/PieShopApi/`.

### Task

Create the following endpoints for the **Pies** resource:

| Method   | Route           | Description      |
| -------- | --------------- | ---------------- |
| `GET`    | `/pies`         | List all pies    |
| `GET`    | `/pies/{id}`    | Get pie by ID    |
| `POST`   | `/pies`         | Create a pie     |
| `PUT`    | `/pies/{id}`    | Update a pie     |
| `DELETE` | `/pies/{id}`    | Delete a pie     |

### Guidelines

- Use Minimal API mapping methods (`MapGet`, `MapPost`, `MapPut`, `MapDelete`)
- Start with **in-memory storage** (e.g. a `List<Pie>`)
- Define the `Pie` record inline for now:

```csharp
public record Pie(int PieId, string Name, string? ShortDescription, decimal Price, bool IsPieOfTheWeek, int CategoryId);
```

> In Module 2.1 you used a class-based `Pie` entity. Here we use a record for immutability â€” a natural fit for API data shapes.

> **Checkpoint:** You can successfully call all five endpoints using `curl`, Postman, or `.http` files.

---

## Step 2 â€” Route Parameters & Constraints

> **Falling behind?** Start from `lab02/step01-complete/PieShopApi/`.

### Task

Enhance your routes with **constraints**:

- Pie ID must be a **positive integer**

### Example

```csharp
app.MapGet("/pies/{id:int:min(1)}", (int id) =>
{
    // ...
});
```

### Test

- `/pies/1` â†’ matches
- `/pies/abc` â†’ does **not** match (404)
- `/pies/0` â†’ does **not** match (constraint rejects it)

> **Checkpoint:** Invalid routes (e.g. `/pies/abc`) no longer match.

---

## Step 3 â€” Query Strings for Filtering & Paging

> **Falling behind?** Start from `lab02/step02-complete/PieShopApi/`.

### Task

Extend `GET /pies` to support:

- **Filtering** by name
- **Sorting** by a field
- **Paging** with page number and page size

### Example Request

```
GET /pies?filter=apple&orderBy=price&page=1&pageSize=5
```

### Implementation Requirements

- Use query string binding
- Provide default values (e.g. `page = 1`, `pageSize = 10`)

### Bonus

Refactor query parameters into a single request object using `[AsParameters]`:

```csharp
public record PieQuery(string? Filter, string? OrderBy, int Page = 1, int PageSize = 10);

app.MapGet("/pies", ([AsParameters] PieQuery query) =>
{
    // ...
});
```

> **Checkpoint:** Query parameters correctly influence the response.

---

## Step 4 â€” Grouping Endpoints

> **Falling behind?** Start from `lab02/step03-complete/PieShopApi/`.

### Task

Group related endpoints using `MapGroup`:

```csharp
var pieGroup = app.MapGroup("/pies");

pieGroup.MapGet("/", handler);
pieGroup.MapGet("/{id:int:min(1)}", handler);
pieGroup.MapPost("/", handler);
pieGroup.MapPut("/{id:int:min(1)}", handler);
pieGroup.MapDelete("/{id:int:min(1)}", handler);
```

### Why This Matters

- Cleaner structure â€” no duplicated route prefixes
- Enables group-level filters and authorization later

> **Checkpoint:** All pie endpoints are organized under a single group.

---

## Step 5 â€” Return Better Responses

> **Falling behind?** Start from `lab02/step04-complete/PieShopApi/`.

### Task

Replace primitive responses with **typed results**:

- `TypedResults.Ok(pie)` â€” 200
- `TypedResults.NotFound()` â€” 404
- `TypedResults.NoContent()` â€” 204
- `TypedResults.CreatedAtRoute(...)` â€” 201

### Example

```csharp
pieGroup.MapGet("/{id:int:min(1)}", Results<Ok<Pie>, NotFound> (int id) =>
{
    var pie = pies.FirstOrDefault(p => p.PieId == id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(pie);
})
.WithName("Pies_GetById");
```

### Requirements

- Use `Results<T1, T2>` as the return type when multiple response types are possible
- Return `201 Created` for POST endpoints using `CreatedAtRoute`

> **Checkpoint:** HTTP status codes match API behavior (404 for missing pies, 201 for creation, etc.).

---

## Step 6 â€” Model Binding & Validation

> **Falling behind?** Start from `lab02/step05-complete/PieShopApi/`.

### Task

Create dedicated DTOs for API input:

- `CreatePieRequest`
- `UpdatePieRequest`

Extract all models to a `Models/` folder.

Add validation using data annotations and .NET 10 validation services (as you first saw in Lab 01):

```csharp
builder.Services.AddValidation();
```

### Example DTO

```csharp
public sealed record CreatePieRequest(
    [Required, StringLength(100, MinimumLength = 3)] string Name,
    [StringLength(200)] string? ShortDescription,
    [Range(0.01, 1000)] decimal Price,
    bool IsPieOfTheWeek = false,
    int CategoryId = 1);
```

### Expected Behavior

- Invalid requests return **400** with `ProblemDetails`
- Validation happens **before** endpoint logic runs

> **Checkpoint:** Invalid input (e.g. empty name, negative price) never reaches your business logic.

---

## Step 7 â€” Dependency Injection

> **Falling behind?** Start from `lab02/step06-complete/PieShopApi/`.

### Task

Introduce proper service abstractions:

- `IPieRepository` â€” interface for pie data access
- `InMemoryPieRepository` â€” in-memory implementation

Place these in a `Services/` folder.

### Registration

```csharp
builder.Services.AddSingleton<IPieRepository, InMemoryPieRepository>();
```

### Usage

```csharp
pieGroup.MapGet("/{id:int:min(1)}", Results<Ok<Pie>, NotFound> (int id, IPieRepository repo) =>
{
    var pie = repo.GetById(id);
    return pie is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(pie);
});
```

> Notice the symmetry with Module 2.1: in the MVC Pie Shop you also used `IPieRepository` with dependency injection. The same pattern applies in Minimal APIs.

> **Checkpoint:** No static data access in endpoints. All data flows through the injected repository.

---

## Reflection Questions

Answer these before moving on:

1. Why are `TypedResults` preferred over returning `IResult` directly?
2. How does the `IPieRepository` here compare to the one in Module 2.1?
3. Why is grouping endpoints useful for security?

---

## Preparing for the Next Lab

Your Pie Shop API now has:

- Endpoint groups with full CRUD
- Typed responses with proper HTTP status codes
- Validated DTOs
- Dependency injection via repository

The next lab will build on this by adding:

- Middleware & endpoint filters
- OpenAPI metadata
- Error handling with ProblemDetails
- Authentication & authorization

---

## Lab Complete

You have completed the **Building a Production-Ready Pie Shop API** lab.

> **Reference solution:** `lab02/step07-complete/PieShopApi/` contains the final state of this lab.