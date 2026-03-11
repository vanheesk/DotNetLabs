# Lab 03 — Routing, Navigation & Minimal APIs

## Overview

In [Lab 02](../lab02/lab02.md) you added EF Core persistence to the Pie Shop. In this lab you will **enhance the application's routing**, practice **Tag Helper–based navigation**, add **Minimal API endpoints**, and learn **programmatic URL generation**.

### Learning objectives

After completing this lab you will be able to:

- Explain characteristics of good URLs (short, readable, persistent, "hackable").
- Configure conventional routing with defaults, optional segments, and constraints.
- Use attribute routing for API-style endpoints.
- Generate navigation links with **Anchor Tag Helpers** (`asp-controller`, `asp-action`, `asp-route-*`).
- Generate URLs programmatically using `LinkGenerator`.
- Create **Minimal API** endpoints with route groups.

### Prerequisites

| Requirement | Version |
|---|---|
| .NET SDK | 10.0 or later |
| Completed Lab 02 | Pie Shop with EF Core |

### Starter project

Open the solution in the `starter/` folder. It is the completed Lab 02 solution, ready for routing enhancements.

---

## Module A — Verify Conventional Routing Baseline

### Steps

1. Open `Program.cs` and confirm the default conventional route is configured:

```csharp
app.MapDefaultControllerRoute();
// This is equivalent to:
// app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
```

2. Start the app and test the following URLs. Predict which ones will match **before** you try them:

| URL | Expected match? | Controller | Action | id |
|---|---|---|---|---|
| `/` | Yes | Home | Index | — |
| `/Pie` | Yes | Pie | Index | — |
| `/Pie/List` | Yes | Pie | List | — |
| `/Pie/Details/3` | Yes | Pie | Details | 3 |
| `/Pie/Details` | Yes | Pie | Details | — (null) |
| `/Pie/Details/abc` | Yes | Pie | Details | — (won't bind to int) |
| `/Admin/Dashboard` | No | — | — | — |

3. **Discuss:** Why does `/Pie/Details/abc` still match the route even though the action expects an `int`?

> **Checkpoint A:** You can explain how conventional routing resolves each URL.

---

## Module B — Route Parameters, Optional Segments & Constraints

### B1 — Required parameter

Add a custom route that requires an `id`:

```csharp
app.MapControllerRoute(
    name: "pieDetails",
    pattern: "pies/{id}",
    defaults: new { controller = "Pie", action = "Details" });
```

Test: `/pies/1` should show pie details. `/pies` should return 404.

### B2 — Optional segment

Modify the pattern to make `id` optional:

```csharp
pattern: "pies/{id?}"
```

Test: `/pies` should now match (returns all pies / or falls through to the action's default behavior).

### B3 — Route constraint

Add an **integer constraint** so only numeric IDs match:

```csharp
pattern: "pies/{id:int}"
```

Test:
- `/pies/1` → matches (200 OK).
- `/pies/abc` → does **not** match (404).

> **Checkpoint B:** Numeric IDs work; non-numeric IDs are rejected by the route constraint.

---

## Module C — Attribute Routing (API-Style)

### C1 — Create an API controller

Create `Controllers/Api/PiesApiController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using PieShop.Models;

namespace PieShop.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PiesApiController : ControllerBase
{
    private readonly IPieRepository _pieRepository;

    public PiesApiController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_pieRepository.AllPies);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var pie = _pieRepository.GetPieById(id);
        if (pie == null)
            return NotFound();
        return Ok(pie);
    }
}
```

### C2 — Enable API controller routing

Make sure `Program.cs` includes:

```csharp
app.MapControllers();
```

### C3 — Test the API

- `GET /api/piesapi` → returns all pies as JSON.
- `GET /api/piesapi/1` → returns pie with id 1.
- `GET /api/piesapi/abc` → 404 (constraint rejects it).

### C4 — Discussion

> When would you prefer **attribute routing** vs **conventional routing**?
> - Attribute routing: fine-grained control, API endpoints, explicit route per action.
> - Conventional routing: consistent patterns across many controllers, MVC views.

> **Checkpoint C:** `/api/piesapi/{id:int}` responds correctly and rejects non-integer IDs.

---

## Module D — Tag Helper Navigation

### D1 — Replace HTML helpers with Anchor Tag Helpers

If you have any links using `@Html.ActionLink(...)`, replace them with Tag Helpers:

**Before:**
```html
@Html.ActionLink("View Pie List", "List", "Pie")
```

**After:**
```html
<a asp-controller="Pie" asp-action="List">View Pie List</a>
```

### D2 — Add route values to links

Update the Details link to pass a route value:

```html
<a asp-controller="Pie" asp-action="Details" asp-route-id="@pie.PieId">
    View Details
</a>
```

### D3 — Build a navigation partial

Create `Views/Shared/_NavPartial.cshtml`:

```html
<ul class="navbar-nav">
    <li class="nav-item">
        <a class="nav-link" asp-controller="Home" asp-action="Index">Home</a>
    </li>
    <li class="nav-item">
        <a class="nav-link" asp-controller="Pie" asp-action="List">Pies</a>
    </li>
</ul>
```

Reference it in `_Layout.cshtml`:

```html
<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
    <div class="container">
        <a class="navbar-brand" asp-controller="Home" asp-action="Index">Pie Shop</a>
        <partial name="_NavPartial" />
    </div>
</nav>
```

### D4 — Verify links survive route changes

Change a route pattern temporarily and confirm the Tag Helper–generated links still work correctly (they are based on routing metadata, not hard-coded paths).

> **Checkpoint D:** All navigation uses Tag Helpers and generates correct URLs.

---

## Module E — Programmatic URL Generation with `LinkGenerator`

### E1 — Create a URL generation service

Create `Services/InviteLinkService.cs`:

```csharp
using Microsoft.AspNetCore.Routing;

namespace PieShop.Services;

public class InviteLinkService
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InviteLinkService(LinkGenerator linkGenerator,
                              IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GeneratePieLink(int pieId)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        return _linkGenerator.GetUriByAction(
            httpContext,
            action: "Details",
            controller: "Pie",
            values: new { id = pieId });
    }
}
```

### E2 — Register the service

In `Program.cs`:

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<InviteLinkService>();
```

### E3 — Use it from a controller

Add to `PieController`:

```csharp
public IActionResult ShareLink(int id)
{
    var link = _inviteLinkService.GeneratePieLink(id);
    return Ok(new { shareUrl = link });
}
```

> **Checkpoint E:** `/Pie/ShareLink/1` returns a full URL that correctly points to the pie details page.

---

## Module F — Minimal API Endpoints with Route Groups

### F1 — Add a health check endpoint

In `Program.cs`, add after `var app = builder.Build();` and after the seeding block:

```csharp
var api = app.MapGroup("/api");

api.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
```

### F2 — Add Minimal API endpoints for categories

```csharp
var categoriesApi = api.MapGroup("/categories");

categoriesApi.MapGet("/", (PieShopDbContext db) =>
    Results.Ok(db.Categories.ToList()));

categoriesApi.MapGet("/{id:int}", (int id, PieShopDbContext db) =>
{
    var category = db.Categories.Find(id);
    return category is not null ? Results.Ok(category) : Results.NotFound();
});
```

### F3 — Test the Minimal API

- `GET /api/health` → returns status + timestamp.
- `GET /api/categories` → returns all categories.
- `GET /api/categories/1` → returns a single category.

> **Checkpoint F:** Minimal API endpoints work alongside MVC controllers.

---

## Deliverables

Make sure you have completed:

- [x] Conventional routing with defaults and optional segments working.
- [x] At least one route constraint (`{id:int}`) rejecting invalid types.
- [x] An API controller with attribute routing under `api/[controller]`.
- [x] All MVC navigation using Anchor Tag Helpers (`asp-controller`, `asp-action`).
- [x] A reusable navigation partial included in the layout.
- [x] `InviteLinkService` generating URLs via `LinkGenerator`.
- [x] Minimal API route group with health check and category endpoints.

---

## Optional Extensions (Fast Finishers)

1. **Razor Pages routing** — Add a Razor Page at `/Products/Details` using `asp-page` and `asp-route-id` for navigation.
2. **Blazor component routing** — Create a `.razor` component with `@page "/orders/{id:int}"` and `[Parameter]` binding (note: Tag Helpers do not work in Razor components).
3. **Minimal API validation** — Add request validation to a Minimal API `POST` endpoint.
4. **OpenAPI integration** — Add `builder.Services.AddOpenApi()` and expose API documentation via Swagger UI.

---

## Summary

Across the three labs you have built a complete **Pie Shop** application:

| Lab | What you built |
|---|---|
| [Lab 01](../lab01/lab01.md) | MVC project skeleton, models, mock repository, DI, controllers, Razor views, layout, Bootstrap |
| [Lab 02](../lab02/lab02.md) | EF Core persistence, migrations, seed data, CRUD, querying, data shaping |
| **Lab 03** | Custom routing, constraints, attribute routing, Tag Helper navigation, programmatic URL generation, Minimal APIs |

Congratulations — you now have hands-on experience with the core building blocks of modern ASP.NET Core web development!
