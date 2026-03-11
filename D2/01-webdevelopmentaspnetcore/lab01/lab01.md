# Lab 01 — Build a "Pie Shop" with ASP.NET Core MVC

## Overview

In this lab you will build a small **Pie Shop catalog** from scratch using ASP.NET Core MVC (.NET 10). You will learn how the **Model–View–Controller** pattern separates concerns, how to register services with **Dependency Injection**, and how to compose a polished UI with **Razor views**, **Layouts**, and **Bootstrap**.

### Learning objectives

After completing this lab you will be able to:

- Explain MVC separation of concerns and why it improves testability and maintainability.
- Implement a domain model (`Pie`, `Category`) and expose it through a repository interface.
- Register dependencies using `AddTransient`, `AddScoped`, and `AddSingleton`.
- Build controllers with actions that return different `ActionResult` types.
- Use model binding to bind request data to action parameters.
- Build strongly-typed Razor views and use a **ViewModel**.
- Set up Layouts (`_Layout`), `_ViewStart.cshtml`, and `_ViewImports.cshtml`.
- Add Bootstrap styling via **LibMan**.

### Prerequisites

| Requirement | Version |
|---|---|
| .NET SDK | 10.0 or later |
| IDE | Visual Studio 2022 / VS Code with C# Dev Kit / JetBrains Rider |

### Starter project

Open the solution in the `starter/` folder. It contains an empty ASP.NET Core project with the MVC framework wired up and static-files middleware enabled.

---

## Module A — Verify the MVC Project Skeleton

The starter project already contains:

- `Program.cs` with `AddControllersWithViews()` and `MapDefaultControllerRoute()`.
- The conventional folder structure: `Controllers/`, `Models/`, `Views/`.
- Static-files middleware enabled via `UseStaticFiles()`.

### Steps

1. Open the `starter/PieShop.sln` solution.
2. Run the application (`dotnet run` or press **F5**).
3. Browse to `https://localhost:<port>/` and verify you see the default welcome page.

> **Checkpoint A:** The app runs and the default route serves a response.

---

## Module B — Model + Repository (Domain & Abstraction)

### B1 — Create the domain models

Create the following classes in the `Models/` folder:

**`Models/Category.cs`**

```csharp
namespace PieShop.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Pie> Pies { get; set; } = new();
}
```

**`Models/Pie.cs`**

```csharp
namespace PieShop.Models;

public class Pie
{
    public int PieId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public bool IsPieOfTheWeek { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
```

### B2 — Create the repository interface

**`Models/IPieRepository.cs`**

```csharp
namespace PieShop.Models;

public interface IPieRepository
{
    IEnumerable<Pie> AllPies { get; }
    IEnumerable<Pie> PiesOfTheWeek { get; }
    Pie? GetPieById(int pieId);
}
```

### B3 — Create the mock repository

**`Models/MockPieRepository.cs`**

Create a class that implements `IPieRepository` and returns hard-coded data. Include at least **three pies** across **two categories**. Example:

```csharp
namespace PieShop.Models;

public class MockCategoryRepository
{
    public static Category FruitPies { get; } = new()
    {
        CategoryId = 1, Name = "Fruit pies", Description = "All-fruity pies"
    };

    public static Category CheeseCakes { get; } = new()
    {
        CategoryId = 2, Name = "Cheese cakes", Description = "Cheesy all the way"
    };
}

public class MockPieRepository : IPieRepository
{
    private readonly List<Pie> _pies;

    public MockPieRepository()
    {
        var fruitCategory = MockCategoryRepository.FruitPies;
        var cheeseCategory = MockCategoryRepository.CheeseCakes;

        _pies = new List<Pie>
        {
            new() { PieId = 1, Name = "Apple Pie", ShortDescription = "Our famous apple pie", Price = 12.95m, IsPieOfTheWeek = true, CategoryId = 1, Category = fruitCategory },
            new() { PieId = 2, Name = "Blueberry Cheese Cake", ShortDescription = "Delicious blueberry cheese cake", Price = 18.95m, IsPieOfTheWeek = false, CategoryId = 2, Category = cheeseCategory },
            new() { PieId = 3, Name = "Strawberry Pie", ShortDescription = "Fresh strawberry pie", Price = 15.95m, IsPieOfTheWeek = true, CategoryId = 1, Category = fruitCategory },
        };
    }

    public IEnumerable<Pie> AllPies => _pies;
    public IEnumerable<Pie> PiesOfTheWeek => _pies.Where(p => p.IsPieOfTheWeek);
    public Pie? GetPieById(int pieId) => _pies.FirstOrDefault(p => p.PieId == pieId);
}
```

> **Checkpoint B:** The project compiles and the models + repository are ready for DI registration.

---

## Module C — Dependency Injection Registration

### Steps

1. Open `Program.cs`.
2. Register the mock repository with a **Scoped** lifetime:

```csharp
builder.Services.AddScoped<IPieRepository, MockPieRepository>();
```

3. **Think about it:** What would change if you used `AddSingleton` instead of `AddScoped`? What about `AddTransient`?
   - `AddTransient` — a new instance for every injection point.
   - `AddScoped` — one instance per HTTP request.
   - `AddSingleton` — one instance for the entire application lifetime.

> **Checkpoint C:** The app starts without DI resolution errors.

---

## Module D — Controllers & Actions

### D1 — Create `HomeController`

If it doesn't exist, create `Controllers/HomeController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace PieShop.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

Create `Views/Home/Index.cshtml`:

```html
<h1>Welcome to Pie Shop!</h1>
<p>Your one-stop shop for delicious pies.</p>
```

### D2 — Create `PieController`

Create `Controllers/PieController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using PieShop.Models;

namespace PieShop.Controllers;

public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;

    public PieController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;
    }

    public IActionResult List()
    {
        var pies = _pieRepository.AllPies;
        return View(pies);
    }
}
```

### D3 — Add an alternative `ActionResult`

Add an action to `PieController` that returns a **JSON** result:

```csharp
public IActionResult ListJson()
{
    return Json(_pieRepository.AllPies);
}
```

> **Checkpoint D:** Browsing to `/Pie/List` (you'll create the view next) and `/Pie/ListJson` both work.

---

## Module E — Model Binding (Details Page)

### Steps

1. Add a `Details` action to `PieController` that accepts an `id` from the route:

```csharp
public IActionResult Details(int id)
{
    var pie = _pieRepository.GetPieById(id);
    if (pie == null)
        return NotFound();
    return View(pie);
}
```

2. Create `Views/Pie/Details.cshtml` (you will improve it in Module F).

> **Checkpoint E:** Navigating to `/Pie/Details/1` shows pie information.

---

## Module F — Razor Views, Strongly-Typed Views & ViewModels

### F1 — Create a strongly-typed List view

Create `Views/Pie/List.cshtml`:

```html
@model IEnumerable<PieShop.Models.Pie>

<h1>All Pies</h1>

<div class="row">
    @foreach (var pie in Model)
    {
        <div class="col-sm-4 mb-3">
            <div class="card">
                <div class="card-body">
                    <h5 class="card-title">@pie.Name</h5>
                    <p class="card-text">@pie.ShortDescription</p>
                    <p class="card-text"><strong>@pie.Price.ToString("c")</strong></p>
                    <a asp-action="Details" asp-route-id="@pie.PieId"
                       class="btn btn-primary">Details</a>
                </div>
            </div>
        </div>
    }
</div>
```

### F2 — Create a ViewModel

Create `ViewModels/PiesListViewModel.cs`:

```csharp
using PieShop.Models;

namespace PieShop.ViewModels;

public class PiesListViewModel
{
    public IEnumerable<Pie> Pies { get; set; } = Enumerable.Empty<Pie>();
    public string? CurrentCategory { get; set; }
}
```

### F3 — Update the controller to use the ViewModel

```csharp
public IActionResult List()
{
    var viewModel = new PiesListViewModel
    {
        Pies = _pieRepository.AllPies,
        CurrentCategory = "All Pies"
    };
    return View(viewModel);
}
```

### F4 — Update the List view to use the ViewModel

Change the `@model` directive at the top of `List.cshtml`:

```html
@model PieShop.ViewModels.PiesListViewModel

<h1>@Model.CurrentCategory</h1>

<div class="row">
    @foreach (var pie in Model.Pies)
    {
        <!-- cards remain the same -->
    }
</div>
```

### F5 — Create the Details view

Create `Views/Pie/Details.cshtml`:

```html
@model PieShop.Models.Pie

<h1>@Model.Name</h1>
<p>@Model.LongDescription</p>
<p><strong>Price:</strong> @Model.Price.ToString("c")</p>
<p><strong>Category:</strong> @Model.Category?.Name</p>
<a asp-action="List" class="btn btn-secondary">Back to list</a>
```

> **Checkpoint F:** `/Pie/List` renders cards from the ViewModel; `/Pie/Details/1` shows pie details.

---

## Module G — Layout, Sections & Imports

### G1 — Create `_Layout.cshtml`

Create `Views/Shared/_Layout.cshtml`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Pie Shop</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" />
</head>
<body>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
        <div class="container">
            <a class="navbar-brand" asp-controller="Home" asp-action="Index">Pie Shop</a>
            <ul class="navbar-nav">
                <li class="nav-item">
                    <a class="nav-link" asp-controller="Pie" asp-action="List">Pies</a>
                </li>
            </ul>
        </div>
    </nav>
    <div class="container">
        @RenderBody()
    </div>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

### G2 — Create `_ViewStart.cshtml`

Create `Views/_ViewStart.cshtml`:

```cshtml
@{
    Layout = "_Layout";
}
```

### G3 — Create `_ViewImports.cshtml`

Create `Views/_ViewImports.cshtml`:

```cshtml
@using PieShop
@using PieShop.Models
@using PieShop.ViewModels
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

> **Checkpoint G:** All views share the same navigation bar and layout without repeating layout assignments.

---

## Module H — Bootstrap Styling with LibMan

### Steps

1. Install the **LibMan CLI** (if not already):

```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
```

2. Initialize LibMan in the project root:

```bash
libman init --default-provider cdnjs
```

3. Add Bootstrap:

```bash
libman install twitter-bootstrap@5.3.3 --destination wwwroot/lib/bootstrap
```

4. Verify the files appeared under `wwwroot/lib/bootstrap/`.
5. Your layout already references Bootstrap CSS/JS from Module G.
6. Optionally add a `wwwroot/css/site.css` for custom overrides.

> **Checkpoint H:** The UI uses Bootstrap classes and looks polished.

---

## Deliverables

Before moving on to **Lab 02** make sure you have:

- [x] A running MVC app showing `/Pie/List` with data from the mock repository.
- [x] A details page at `/Pie/Details/{id}` using route-based model binding.
- [x] `IPieRepository` interface + `MockPieRepository` implementation.
- [x] DI registration with `AddScoped`.
- [x] Strongly-typed Razor views using a `PiesListViewModel`.
- [x] `_Layout`, `_ViewStart`, and `_ViewImports` configured.
- [x] Bootstrap via LibMan with updated styling.

---

## Optional Extensions (Fast Finishers)

1. **Add a `CategoryController`** with a `List` action to reinforce routing conventions and view discovery.
2. **Add a "Search pies" form** to `PieController` to deepen model-binding practice.
3. **Add an action that returns a `FileResult`** (e.g., download a pie image) to practice file-based action results.
4. **Add a `ICategoryRepository`** with its own mock implementation and register it in DI.

---

> **Next:** In [Lab 02](../lab02/lab02.md) you will replace the mock repository with **Entity Framework Core** and persist your Pie Shop data to a real database.
