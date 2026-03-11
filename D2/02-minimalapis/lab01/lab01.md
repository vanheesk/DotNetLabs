# Lab 01: Pie Shop API Kickstart

## Lab Overview

In Day 1, you built Minimal API projects from scratch, learned routing, middleware, filters, OpenAPI, and JWT auth. You already know the fundamentals.

This lab **skips the basics** and fast-tracks you to a working **Pie Shop API** that will serve as the foundation for the remaining labs in this module. You will focus on concepts that are new or go deeper than Day 1:

- `TypedResults` and `Results<T1, T2>` union return types
- `[AsParameters]` for clean query binding
- .NET 10 built-in validation with `AddValidation()`
- Response customization (CSV export, custom XML result)

> **Estimated time:** 45 minutes

## Learning Objectives

By the end of this lab, you will be able to:

- Scaffold a full CRUD Minimal API using the Pie Shop domain
- Return semantically correct HTTP responses using `TypedResults`
- Bind complex query parameters with `[AsParameters]`
- Enable automatic model validation with .NET 10's `AddValidation()`
- Return custom content types (CSV file downloads, XML responses)

## Prerequisites

- .NET 10 SDK installed
- Completed Day 1 web development labs (Minimal API fundamentals, routing, DI)
- Familiarity with the Pie Shop domain from Module 2.1 (MVC labs)

> **Starter and solution projects are available.** If you get stuck, use the reference solution:
> ```shell
> cp -r lab01/lab01-complete/PieShopApi .
> cd PieShopApi && dotnet run
> ```

---

## Part 1 — Project Setup & Domain Model

> **Starting point:** `lab01/start/PieShopApi/`

The starter project contains an empty `Program.cs` with `WebApplication` scaffolding, plus pre-defined model files.

### 1a — Review the Domain Models

Open the `Models/` folder. You will find:

- `Pie.cs` — the core entity
- `Category.cs` — grouping for pies
- `CreatePieRequest.cs` — DTO for creating a pie (with validation attributes)
- `UpdatePieRequest.cs` — DTO for updating a pie
- `PieQuery.cs` — query parameters for filtering/paging

Review each file. Notice the validation attributes on the request DTOs — these will be used in Part 4.

### 1b — Review the Repository

Open `Services/IPieRepository.cs` and `Services/InMemoryPieRepository.cs`.

The in-memory repository is pre-wired with seed data so you have pies to query immediately.

### 1c — Wire Up DI and Validation

In `Program.cs`, register the repository and enable .NET 10 validation:

```csharp
builder.Services.AddValidation();
builder.Services.AddSingleton<IPieRepository, InMemoryPieRepository>();
```

> You registered services and used DI in Day 1. Here we just need it wired up to move forward.

---

## Part 2 — Full CRUD with TypedResults

In Day 1, you used `Results.Ok()` and `Results.NotFound()`. Now you will use **`TypedResults`** — the strongly-typed equivalent that enables better OpenAPI metadata and compile-time safety.

### Key Difference

| Day 1 | Today |
|-------|-------|
| `Results.Ok(pie)` returns `IResult` | `TypedResults.Ok(pie)` returns `Ok<Pie>` |
| Return type: `IResult` | Return type: `Results<Ok<Pie>, NotFound>` |

The `Results<T1, T2>` union type tells the framework (and OpenAPI) exactly which responses are possible.

### 2a — GET All Pies

```csharp
var pieGroup = app.MapGroup("/pies");

pieGroup.MapGet("/", (IPieRepository repo) =>
{
    return TypedResults.Ok(repo.GetAll());
})
.WithName("Pies_GetAll");
```

### 2b — GET Pie by ID

Implement this yourself. Requirements:

- Route: `/{id:int:min(1)}`
- Return type: `Results<Ok<Pie>, NotFound>`
- Return `TypedResults.NotFound()` when the pie doesn't exist
- Add `.WithName("Pies_GetById")`

### 2c — POST Create Pie

Implement this yourself. Requirements:

- Accept a `CreatePieRequest` body
- Map it to a `Pie` and add via `repo.Add(pie)`
- Return `TypedResults.CreatedAtRoute(pie, "Pies_GetById", new { id = pie.PieId })`

### 2d — PUT Update Pie

Implement this yourself. Requirements:

- Route: `/{id:int:min(1)}`
- Accept an `UpdatePieRequest` body
- Return the updated pie or `NotFound`

### 2e — DELETE Pie

Implement this yourself. Requirements:

- Route: `/{id:int:min(1)}`
- Return `TypedResults.NoContent()` on success, `TypedResults.NotFound()` on failure
- Return type: `Results<NoContent, NotFound>`

> **Checkpoint:** Run the app. Test all five CRUD endpoints using `curl`, `.http` files, or Postman. Verify correct HTTP status codes (200, 201, 204, 404).

---

## Part 3 — Query Binding with [AsParameters]

Enhance `GET /pies` to support filtering, sorting, and paging using the `PieQuery` record.

### 3a — Apply [AsParameters]

Replace the simple `GET /` handler with:

```csharp
pieGroup.MapGet("/", ([AsParameters] PieQuery query, IPieRepository repo) =>
{
    var result = repo.GetAll();

    // TODO: Apply filtering, sorting, and paging
    // Use query.Filter, query.OrderBy, query.Page, query.PageSize

    return TypedResults.Ok(result);
})
.WithName("Pies_GetAll");
```

### 3b — Implement the Logic

Add the following behavior (try it yourself before looking at the hints):

1. **Filter** — if `query.Filter` is not empty, filter pies whose `Name` contains the value (case-insensitive)
2. **Sort** — support `name`, `price`, and `price,desc` as `OrderBy` values
3. **Page** — skip `(Page - 1) * PageSize` items and take `PageSize` items

<details>
<summary>Hint: Filtering</summary>

```csharp
if (!string.IsNullOrEmpty(query.Filter))
    result = result.Where(p => p.Name.Contains(query.Filter, StringComparison.OrdinalIgnoreCase));
```
</details>

<details>
<summary>Hint: Sorting</summary>

```csharp
result = query.OrderBy?.ToLower() switch
{
    "name" => result.OrderBy(p => p.Name),
    "price" => result.OrderBy(p => p.Price),
    "price,desc" => result.OrderByDescending(p => p.Price),
    _ => result
};
```
</details>

### Test

```
GET /pies?filter=apple&orderBy=price&page=1&pageSize=5
```

> **Checkpoint:** Filtering, sorting, and paging all work correctly. Default values apply when parameters are omitted.

---

## Part 4 — Validation in Action

You already added `builder.Services.AddValidation()` in Part 1. Now test it.

### 4a — Send Invalid Input

POST to `/pies` with invalid data:

```json
{
  "name": "",
  "price": -5
}
```

Expected: a `400 Bad Request` with `ProblemDetails` — the request never reaches your handler.

### 4b — Understand What Happened

- `AddValidation()` automatically validates parameters annotated with `[Required]`, `[Range]`, `[StringLength]`, etc.
- No manual validation code needed in your endpoints
- This is new in .NET 10 — in Day 1 you may have validated manually

### 4c — Experiment

1. Comment out `builder.Services.AddValidation()`
2. Send the same invalid request
3. Observe: the request now reaches your handler with invalid data
4. Re-enable validation

> **Checkpoint:** You can explain how `AddValidation()` short-circuits invalid requests automatically.

---

## Part 5 — Response Customization

Your API only returns JSON so far. Add two alternative response formats.

### 5a — CSV Export

Add an endpoint that exports all pies as a downloadable CSV file:

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
})
.WithName("Pies_Export");
```

### 5b — Custom XML Result

Create a `XmlResult<T>` class that implements `IResult` and serializes the response as XML.

Try implementing it yourself first. Requirements:
- Set `Content-Type` to `application/xml`
- Use `DataContractSerializer` to serialize the value
- Accept a configurable status code (default 200)

<details>
<summary>Hint: XmlResult implementation</summary>

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
</details>

### 5c — Add an XML Endpoint

```csharp
pieGroup.MapGet("/{id:int:min(1)}/xml", (int id, IPieRepository repo) =>
{
    var pie = repo.GetById(id);
    return pie is null
        ? Results.NotFound()
        : new XmlResult<Pie>(pie);
})
.WithName("Pies_GetByIdXml");
```

> **Checkpoint:** `/pies/export` downloads a CSV file. `/pies/1/xml` returns XML. Both alongside standard JSON endpoints.

---

## Lab Checkpoint

Your Pie Shop API now has:

- ✅ Full CRUD with `TypedResults` and union return types
- ✅ Query binding with `[AsParameters]` for filtering, sorting, paging
- ✅ Automatic validation via `AddValidation()`
- ✅ CSV export and XML response support
- ✅ Clean `Program.cs` with route groups and DI

This is the baseline for **Lab 02** (EF Core, output caching, resilience) and **Lab 03** (gRPC inter-service communication).

---

## Lab Complete

> **Reference solution:** `lab01/lab01-complete/PieShopApi/` contains the final state of this lab.

In the next lab, you will replace the in-memory repository with EF Core and add output caching, resilience, and rate limiting to make the API production-ready.
