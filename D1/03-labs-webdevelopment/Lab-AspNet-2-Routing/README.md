# Lab 2: Routing & Endpoint Groups

## Objective

Implement routing using **Minimal APIs**, organise endpoints with **route groups** for versioning, apply authorisation metadata, and use **route constraints**.

---

## Prerequisites

- .NET 10 SDK installed
- Completed Lab 1 or familiarity with creating ASP.NET Core projects

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Basic Minimal API Routing

### Tasks

1. Create several endpoints under different HTTP verbs:
   - `GET /products` — returns a list of product names
   - `GET /products/{id:int}` — returns a single product by ID
   - `POST /products` — accepts a product and returns `201 Created`
2. Use route constraints (`:int`, `:alpha`, `:minlength`) to validate parameters.

---

## Exercise 2 – Route Groups for API Versioning

**Route groups** let you organise and prefix related endpoints. They are ideal for API versioning.

```csharp
var v1 = app.MapGroup("/api/v1");
v1.MapGet("/items", () => ...);
```

### Tasks

1. Create a route group `/api/v1/products` with basic CRUD endpoints.
2. Create a route group `/api/v2/products` with an enhanced version (e.g., supports filtering with query params).
3. Add `.WithTags("V1")` and `.WithTags("V2")` for Swagger grouping.

---

## Exercise 3 – Route Constraints

Route constraints restrict which values a route parameter will accept.

| Constraint | Example | Matches |
|-----------|---------|---------|
| `int` | `{id:int}` | `123` |
| `alpha` | `{name:alpha}` | `abc` |
| `minlength(n)` | `{code:minlength(3)}` | `ABC` |
| `range(a,b)` | `{age:range(1,120)}` | `25` |
| `guid` | `{id:guid}` | GUID strings |

### Tasks

1. Add a `GET /users/{username:alpha:minlength(3)}` endpoint.
2. Add a `GET /orders/{id:guid}` endpoint.
3. Test with both valid and invalid values — observe the 404 for non-matching constraints.

---

## Exercise 4 – Applying Metadata to Groups

You can apply filters, tags, and authorisation to an entire group.

### Tasks

1. Create a group `/api/admin` and add `.RequireAuthorization()` to the group (this will return 401 for now, which is expected).
2. Add `.WithTags("Admin")` to the group.
3. Add a couple of GET endpoints under the admin group.
4. Verify in Swagger that the admin endpoints are tagged and that calling them returns 401.

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed. Notice how route groups keep your code organised and allow you to apply cross-cutting concerns to sets of endpoints.
