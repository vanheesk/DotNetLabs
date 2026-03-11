# Lab 02 — Add EF Core Persistence to the Pie Shop

## Overview

In [Lab 01](../lab01/lab01.md) you built a Pie Shop MVC application using an in-memory mock repository. In this lab you will **replace the mock with Entity Framework Core**, persist data to a relational database, and learn to query, shape, and manipulate data using LINQ.

### Learning objectives

After completing this lab you will be able to:

- Explain what EF Core is and why an ORM reduces hand-written data-access code.
- Add EF Core NuGet packages and register a `DbContext` with dependency injection.
- Build an EF Core model with entities, keys, relationships, constraints, and indexes.
- Create and apply migrations and seed initial data.
- Perform CRUD operations and understand change tracking.
- Write LINQ queries with eager/explicit loading of related data.
- Apply data shaping: filtering, sorting, pagination, projections, and aggregations.

### Prerequisites

| Requirement | Version |
|---|---|
| .NET SDK | 10.0 or later |
| Completed Lab 01 | Pie Shop MVC app with mock repository |
| SQL Server / SQLite | LocalDB, Docker SQL Server, or SQLite |

### Starter project

Open the solution in the `starter/` folder. It is the completed Lab 01 solution, ready for you to add EF Core support.

---

## Module A — Understand EF Core Fundamentals

Before writing code, you should be able to answer these questions:

1. What is an **ORM** and what problem does EF Core solve?
2. What is the difference between **code-first** and **database-first** (scaffolding / reverse engineering)?
3. When would you use `dotnet ef dbcontext scaffold`?

> **Checkpoint A:** You can explain the role of EF Core in this application.

---

## Module B — Add EF Core Packages & Register DbContext

### B1 — Install NuGet packages

Run from the project directory:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

> If you prefer **SQLite** for a simpler local setup, use `Microsoft.EntityFrameworkCore.Sqlite` instead of the SqlServer package.

Install the EF Core CLI tool:

```bash
dotnet tool install --global dotnet-ef
```

### B2 — Create the DbContext

Create `Data/PieShopDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using PieShop.Models;

namespace PieShop.Data;

public class PieShopDbContext : DbContext
{
    public PieShopDbContext(DbContextOptions<PieShopDbContext> options)
        : base(options) { }

    public DbSet<Pie> Pies => Set<Pie>();
    public DbSet<Category> Categories => Set<Category>();
}
```

### B3 — Register DbContext in DI

Add the following to `Program.cs` **before** `builder.Build()`:

```csharp
builder.Services.AddDbContext<PieShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PieShopDb")));
```

> For connection pooling (production scenario), you can use `AddDbContextPool<PieShopDbContext>(...)` instead. Pooling reuses context instances for higher throughput — but do not store per-request state on the context.

### B4 — Add a connection string

In `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PieShopDb": "Server=(localdb)\\mssqllocaldb;Database=PieShop;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

> For **SQLite**, use: `"Data Source=PieShop.db"` and change `UseSqlServer` to `UseSqlite`.

> **Checkpoint B:** The app starts successfully with the configured connection string and DbContext registration.

---

## Module C — Build the EF Core Model

### C1 — Update entity classes with data annotations

Update `Models/Pie.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PieShop.Models;

public class Pie
{
    public int PieId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ShortDescription { get; set; }

    public string? LongDescription { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? ImageThumbnailUrl { get; set; }

    public bool IsPieOfTheWeek { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
```

Update `Models/Category.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace PieShop.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public List<Pie> Pies { get; set; } = new();
}
```

### C2 — Add Fluent API configuration (optional overrides)

In `PieShopDbContext`, add an `OnModelCreating` override:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Pie>()
        .HasIndex(p => p.Name);

    modelBuilder.Entity<Pie>()
        .HasOne(p => p.Category)
        .WithMany(c => c.Pies)
        .HasForeignKey(p => p.CategoryId);
}
```

### C3 — Understand key conventions

Note how EF Core uses the convention `<TypeName>Id` (e.g., `PieId`, `CategoryId`) as the primary key. If you used just `Id`, that would also work by convention.

> **Checkpoint C:** The project compiles and the model is configured with keys, constraints, relationships, and an index.

---

## Module D — Migrations & Seed Data

### D1 — Create the initial migration

```bash
dotnet ef migrations add InitialCreate
```

### D2 — Apply the migration

```bash
dotnet ef database update
```

### D3 — Add a database seeder

Create `Data/DbInitializer.cs`:

```csharp
using PieShop.Models;

namespace PieShop.Data;

public static class DbInitializer
{
    public static void Seed(PieShopDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Pies.Any())
            return; // Already seeded

        var fruitCategory = new Category
        {
            Name = "Fruit pies",
            Description = "All-fruity pies"
        };

        var cheeseCategory = new Category
        {
            Name = "Cheese cakes",
            Description = "Cheesy all the way"
        };

        context.Categories.AddRange(fruitCategory, cheeseCategory);

        context.Pies.AddRange(
            new Pie { Name = "Apple Pie", ShortDescription = "Our famous apple pie", LongDescription = "A classic apple pie made with fresh Granny Smith apples.", Price = 12.95m, IsPieOfTheWeek = true, Category = fruitCategory },
            new Pie { Name = "Blueberry Cheese Cake", ShortDescription = "Delicious blueberry cheese cake", LongDescription = "Rich cheese cake topped with fresh blueberries.", Price = 18.95m, IsPieOfTheWeek = false, Category = cheeseCategory },
            new Pie { Name = "Strawberry Pie", ShortDescription = "Fresh strawberry pie", LongDescription = "Sweet pie packed with juicy strawberries.", Price = 15.95m, IsPieOfTheWeek = true, Category = fruitCategory },
            new Pie { Name = "Cherry Pie", ShortDescription = "Classic cherry pie", LongDescription = "Traditional cherry pie with a flaky crust.", Price = 13.95m, IsPieOfTheWeek = false, Category = fruitCategory },
            new Pie { Name = "Pumpkin Cheese Cake", ShortDescription = "Seasonal pumpkin cheese cake", LongDescription = "Creamy pumpkin cheese cake with a graham cracker crust.", Price = 16.95m, IsPieOfTheWeek = true, Category = cheeseCategory }
        );

        context.SaveChanges();
    }
}
```

### D4 — Call the seeder at startup

In `Program.cs`, after `var app = builder.Build();`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PieShopDbContext>();
    DbInitializer.Seed(context);
}
```

> **Checkpoint D:** Run the app, verify the database is created and seeded data appears (e.g., via `/Pie/List`).

---

## Module E — Replace the Mock Repository with EF Core

### E1 — Create `PieRepository` backed by EF Core

Create `Data/PieRepository.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using PieShop.Models;

namespace PieShop.Data;

public class PieRepository : IPieRepository
{
    private readonly PieShopDbContext _context;

    public PieRepository(PieShopDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Pie> AllPies =>
        _context.Pies.Include(p => p.Category).ToList();

    public IEnumerable<Pie> PiesOfTheWeek =>
        _context.Pies.Include(p => p.Category)
            .Where(p => p.IsPieOfTheWeek).ToList();

    public Pie? GetPieById(int pieId) =>
        _context.Pies.Include(p => p.Category)
            .FirstOrDefault(p => p.PieId == pieId);
}
```

### E2 — Swap the DI registration

In `Program.cs`, replace:

```csharp
builder.Services.AddScoped<IPieRepository, MockPieRepository>();
```

with:

```csharp
builder.Services.AddScoped<IPieRepository, PieRepository>();
```

> **Checkpoint E:** `/Pie/List` now shows data from the database instead of the mock.

---

## Module F — CRUD Operations & Change Tracking

### F1 — Add a create action

Add to `PieController`:

```csharp
[HttpGet]
public IActionResult Create()
{
    return View();
}

[HttpPost]
public IActionResult Create(Pie pie)
{
    if (ModelState.IsValid)
    {
        _context.Pies.Add(pie);
        _context.SaveChanges();
        return RedirectToAction("List");
    }
    return View(pie);
}
```

> You will need to inject `PieShopDbContext` into the controller (or extend the repository interface) to demonstrate CRUD.

### F2 — Add update and delete

Add actions for:

- **Edit** — load the entity, update a property, call `SaveChanges()`.
- **Delete** — load the entity, call `Remove()`, then `SaveChanges()`.

### F3 — Unit of Work exercise

Create a method or endpoint that performs **multiple adds/updates/deletes** before calling `SaveChanges()` once. This demonstrates EF Core's unit-of-work pattern.

> **Checkpoint F:** You can create, update, and delete pies through the UI or endpoints.

---

## Module G — Querying & Loading Related Data

### G1 — LINQ querying

Write the following queries (e.g., in a new `SearchController` or extend `PieController`):

```csharp
// Find a single pie by name
var pie = _context.Pies.Single(p => p.Name == "Apple Pie");

// Find all pies under a price threshold
var affordable = _context.Pies.Where(p => p.Price < 15.00m).ToList();
```

### G2 — Eager loading

You already used `Include()` in the repository. Now add a query with `ThenInclude()`:

```csharp
var categories = _context.Categories
    .Include(c => c.Pies)
    .ToList();
```

### G3 — Explicit loading

Try loading related data explicitly:

```csharp
var pie = _context.Pies.First();
_context.Entry(pie).Reference(p => p.Category).Load();
```

### G4 — Discuss lazy loading

> **Discussion:** EF Core supports lazy loading via proxies, but it is opt-in. Why might you avoid it? (N+1 queries, performance cost, serialization issues.)

> **Checkpoint G:** You can query pies with their categories and explain the loading strategy used.

---

## Module H — Data Shaping: Filter, Sort, Page, Project, Aggregate

Build a "search/list" endpoint that supports query parameters:

### H1 — Filtering

```csharp
public IActionResult Search(string? category, decimal? maxPrice)
{
    var query = _context.Pies.Include(p => p.Category).AsQueryable();

    if (!string.IsNullOrEmpty(category))
        query = query.Where(p => p.Category!.Name == category);

    if (maxPrice.HasValue)
        query = query.Where(p => p.Price <= maxPrice.Value);

    return View(query.ToList());
}
```

### H2 — Sorting

```csharp
query = query.OrderBy(p => p.Price).ThenBy(p => p.Name);
```

### H3 — Pagination

```csharp
int page = 1, pageSize = 5;
query = query.Skip((page - 1) * pageSize).Take(pageSize);
```

### H4 — Projections

```csharp
var dtos = _context.Pies.Select(p => new { p.Name, p.Price, Category = p.Category!.Name }).ToList();
```

### H5 — Aggregations

```csharp
var count = _context.Pies.Count();
var avgPrice = _context.Pies.Average(p => p.Price);
var countByCategory = _context.Pies
    .GroupBy(p => p.Category!.Name)
    .Select(g => new { Category = g.Key, Count = g.Count() })
    .ToList();
```

> **Checkpoint H:** Your search endpoint supports filtering, sorting, and pagination and returns shaped data.

---

## Deliverables

Before moving on to **Lab 03** make sure you have:

- [x] EF Core packages installed and `PieShopDbContext` registered.
- [x] Entity classes with data annotations and/or Fluent API configuration.
- [x] At least one migration applied and seed data visible.
- [x] `PieRepository` using EF Core replacing the mock repository.
- [x] CRUD operations working (create, update, delete).
- [x] Queries using `Include` for eager loading of related data.
- [x] Data shaping with filtering, sorting, pagination, projections, and aggregations.

---

## Optional Extensions (Fast Finishers)

1. **Bulk updates/deletes** — Use EF Core's `ExecuteUpdate` / `ExecuteDelete` for set-based operations without loading entities.
2. **JSON column mapping** — Add a property mapped to a JSON column (e.g., `NutritionInfo` as a complex/owned type stored as JSON).
3. **Switch to SQLite** — Swap the database provider and verify everything still works.
4. **Add an `Order` entity** — Create an `Order` with a many-to-many relationship to `Pie` (via `OrderDetail`) and practice more complex queries.

---

> **Next:** In [Lab 03](../lab03/lab03.md) you will enhance the Pie Shop with **custom routing**, **Tag Helper navigation**, **Minimal API endpoints**, and **programmatic URL generation**.
