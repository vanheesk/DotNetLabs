# Lab 01: Building a Blazor Frontend for the Pie Shop API

## Lab Overview

In Module 2.3, you built a **Pie Shop API** using Minimal APIs with full CRUD endpoints, validation, and OpenAPI support. Now it is time to build a **modern web frontend** for that API using **Blazor**.

You will create a **Blazor Web App** with Interactive Server rendering that consumes the Pie Shop API over HTTP. By the end of this lab, you will have a fully functional web application where users can browse, view, create, edit, and delete pies.

## Learning Objectives

By the end of this lab, you will be able to:

- Create a Blazor Web App project and understand its structure
- Use `HttpClient` and typed services to call a REST API
- Build interactive pages with Razor components
- Use data binding, event handling, and component parameters
- Implement forms with `EditForm` and data annotation validation
- Navigate between pages using `NavigationManager` and route parameters

## Prerequisites

- .NET 10 SDK installed
- Visual Studio 2022+ or VS Code with C# Dev Kit
- **Completed Module 2.3** (conceptual understanding of the Pie Shop API)

> **Starter projects available.** The `starter/` folder contains:
> - `PieShopApi/` — The Pie Shop API from Module 2.3 (simplified, with seed data and CORS)
> - `PieShopBlazor/` — A scaffolded Blazor Web App ready for you to extend
>
> If you prefer to start fresh, the `solution/` folder contains the complete working solution.

---

## Lab Setup

### Step 0 — Start the API

The Pie Shop API must be running for the Blazor app to work. Open a terminal:

```shell
cd starter/PieShopApi
dotnet run
```

Verify the API is running at `http://localhost:5100` by browsing to:

```
http://localhost:5100/pies
```

You should see a JSON array of pies.

> **Keep this terminal running** throughout the entire lab.

Open a **second terminal** for running the Blazor app.

---

## Step 1 — Explore the Blazor Starter Project

Open the `starter/PieShopBlazor/` project.

```shell
cd starter/PieShopBlazor
dotnet run
```

Browse to `http://localhost:5200`. You should see the default Blazor template with a Home page.

### Understand the Project Structure

| File / Folder                     | Purpose                              |
| --------------------------------- | ------------------------------------ |
| `Program.cs`                      | Host builder, DI, middleware         |
| `Components/App.razor`            | Root HTML document                   |
| `Components/Routes.razor`         | Client-side router                   |
| `Components/Layout/MainLayout.razor` | Shared page layout                |
| `Components/Layout/NavMenu.razor` | Navigation sidebar                   |
| `Components/Pages/Home.razor`     | Landing page                         |
| `Models/Pie.cs`                   | Pie model (shared with API)          |

### Task

Remove the sample `Counter.razor` and `Weather.razor` pages from `Components/Pages/` — we will not need them.

Also update `NavMenu.razor` to remove the Counter and Weather nav items, and add a Pies link:

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="pies">
        <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Pies
    </NavLink>
</div>
```

---

## Step 2 — Create the PieApiService

Create a new file `Services/PieApiService.cs`:

```csharp
using System.Net.Http.Json;
using PieShopBlazor.Models;

namespace PieShopBlazor.Services;

public class PieApiService(HttpClient http)
{
    public async Task<List<Pie>> GetAllAsync()
        => await http.GetFromJsonAsync<List<Pie>>("pies") ?? [];

    public async Task<Pie?> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<Pie>($"pies/{id}");

    public async Task<Pie?> CreateAsync(Pie pie)
    {
        var response = await http.PostAsJsonAsync("pies", pie);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Pie>();
    }

    public async Task<Pie?> UpdateAsync(int id, Pie pie)
    {
        var response = await http.PutAsJsonAsync($"pies/{id}", pie);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Pie>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"pies/{id}");
        return response.IsSuccessStatusCode;
    }
}
```

### Register the Service

Open `Program.cs` and add the `HttpClient` registration **before** `var app = builder.Build();`:

```csharp
builder.Services.AddHttpClient<PieApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5100");
});
```

You will also need to add the using directive at the top:

```csharp
using PieShopBlazor.Services;
```

> **Checkpoint:** The project should build without errors. The service is registered but not used yet.

---

## Step 3 — Build the Pie List Page

Create a new file `Components/Pages/PieList.razor`:

```razor
@page "/pies"
@using PieShopBlazor.Models
@using PieShopBlazor.Services
@inject PieApiService PieService
@inject NavigationManager Navigation

<PageTitle>Our Pies</PageTitle>

<h1>Our Pies</h1>

<div class="mb-3">
    <a href="/pies/add" class="btn btn-success">+ Add New Pie</a>
</div>

@if (pies is null)
{
    <p><em>Loading pies...</em></p>
}
else if (pies.Count == 0)
{
    <p>No pies found.</p>
}
else
{
    <div class="row">
        @foreach (var pie in pies)
        {
            <div class="col-md-4 mb-4">
                <div class="card h-100">
                    <div class="card-body">
                        <h5 class="card-title">@pie.Name</h5>
                        <p class="card-text">@pie.ShortDescription</p>
                        <p class="card-text">
                            <strong>@pie.Price.ToString("C")</strong>
                            @if (pie.IsPieOfTheWeek)
                            {
                                <span class="badge bg-warning ms-2">Pie of the Week!</span>
                            }
                        </p>
                    </div>
                    <div class="card-footer">
                        <a href="@($"/pies/{pie.PieId}")" class="btn btn-primary btn-sm">Details</a>
                        <a href="@($"/pies/{pie.PieId}/edit")" class="btn btn-outline-secondary btn-sm">Edit</a>
                    </div>
                </div>
            </div>
        }
    </div>
}

@code {
    private List<Pie>? pies;

    protected override async Task OnInitializedAsync()
    {
        pies = await PieService.GetAllAsync();
    }
}
```

Run the app and navigate to `/pies`. You should see Bootstrap cards for each pie from the API.

> **Checkpoint:** The Pie List page loads and displays pies from the API.

---

## Step 4 — Build the Pie Detail Page

Create `Components/Pages/PieDetail.razor`:

```razor
@page "/pies/{Id:int}"
@using PieShopBlazor.Models
@using PieShopBlazor.Services
@inject PieApiService PieService
@inject NavigationManager Navigation

<PageTitle>@(pie?.Name ?? "Pie Details")</PageTitle>

@if (pie is null)
{
    <p><em>Loading...</em></p>
}
else
{
    <h1>@pie.Name</h1>

    <div class="card" style="max-width: 500px;">
        <div class="card-body">
            <p>@pie.ShortDescription</p>
            <hr />
            <p><strong>Price:</strong> @pie.Price.ToString("C")</p>
            <p><strong>Category:</strong> @pie.CategoryId</p>
            <p>
                <strong>Pie of the Week:</strong>
                @(pie.IsPieOfTheWeek ? "Yes ⭐" : "No")
            </p>
        </div>
        <div class="card-footer">
            <a href="@($"/pies/{pie.PieId}/edit")" class="btn btn-outline-secondary">Edit</a>
            <button class="btn btn-danger" @onclick="DeletePie">Delete</button>
            <a href="/pies" class="btn btn-link">Back to list</a>
        </div>
    </div>
}

@code {
    [Parameter] public int Id { get; set; }

    private Pie? pie;

    protected override async Task OnInitializedAsync()
    {
        pie = await PieService.GetByIdAsync(Id);
    }

    private async Task DeletePie()
    {
        var deleted = await PieService.DeleteAsync(Id);
        if (deleted)
        {
            Navigation.NavigateTo("/pies");
        }
    }
}
```

> **Checkpoint:** Click a pie card on the list page. You should see the detail view. The Delete button should remove the pie and navigate back to the list.

---

## Step 5 — Build the Add Pie Page

Create `Components/Pages/PieAdd.razor`:

```razor
@page "/pies/add"
@using PieShopBlazor.Models
@using PieShopBlazor.Services
@inject PieApiService PieService
@inject NavigationManager Navigation

<PageTitle>Add a Pie</PageTitle>

<h1>Add a New Pie</h1>

<div class="card" style="max-width: 500px;">
    <div class="card-body">
        <EditForm Model="@pie" OnValidSubmit="HandleSubmit" FormName="AddPie">
            <DataAnnotationsValidator />
            <ValidationSummary class="text-danger" />

            <div class="mb-3">
                <label class="form-label">Name</label>
                <InputText @bind-Value="pie.Name" class="form-control" />
                <ValidationMessage For="() => pie.Name" />
            </div>

            <div class="mb-3">
                <label class="form-label">Description</label>
                <InputText @bind-Value="pie.ShortDescription" class="form-control" />
            </div>

            <div class="mb-3">
                <label class="form-label">Price</label>
                <InputNumber @bind-Value="pie.Price" class="form-control" />
                <ValidationMessage For="() => pie.Price" />
            </div>

            <div class="mb-3 form-check">
                <InputCheckbox @bind-Value="pie.IsPieOfTheWeek" class="form-check-input" />
                <label class="form-check-label">Pie of the Week</label>
            </div>

            <div class="mb-3">
                <label class="form-label">Category ID</label>
                <InputNumber @bind-Value="pie.CategoryId" class="form-control" />
            </div>

            <button type="submit" class="btn btn-success">Create</button>
            <a href="/pies" class="btn btn-link">Cancel</a>
        </EditForm>
    </div>
</div>

@code {
    [SupplyParameterFromForm]
    private PieFormModel pie { get; set; } = new();

    private async Task HandleSubmit()
    {
        var newPie = new Pie
        {
            Name = pie.Name,
            ShortDescription = pie.ShortDescription,
            Price = pie.Price,
            IsPieOfTheWeek = pie.IsPieOfTheWeek,
            CategoryId = pie.CategoryId
        };
        await PieService.CreateAsync(newPie);
        Navigation.NavigateTo("/pies");
    }
}
```

You'll need a `PieFormModel` class for the form. Create `Models/PieFormModel.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace PieShopBlazor.Models;

public class PieFormModel
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = "";

    [StringLength(200)]
    public string? ShortDescription { get; set; }

    [Range(0.01, 1000)]
    public decimal Price { get; set; }

    public bool IsPieOfTheWeek { get; set; }

    public int CategoryId { get; set; } = 1;
}
```

> **Checkpoint:** Navigate to `/pies/add`, fill in the form, and submit. A new pie should appear in the list.

---

## Step 6 — Build the Edit Pie Page

Create `Components/Pages/PieEdit.razor`:

```razor
@page "/pies/{Id:int}/edit"
@using PieShopBlazor.Models
@using PieShopBlazor.Services
@inject PieApiService PieService
@inject NavigationManager Navigation

<PageTitle>Edit Pie</PageTitle>

<h1>Edit Pie</h1>

@if (pie is null)
{
    <p><em>Loading...</em></p>
}
else
{
    <div class="card" style="max-width: 500px;">
        <div class="card-body">
            <EditForm Model="@pie" OnValidSubmit="HandleSubmit" FormName="EditPie">
                <DataAnnotationsValidator />
                <ValidationSummary class="text-danger" />

                <div class="mb-3">
                    <label class="form-label">Name</label>
                    <InputText @bind-Value="pie.Name" class="form-control" />
                    <ValidationMessage For="() => pie.Name" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Description</label>
                    <InputText @bind-Value="pie.ShortDescription" class="form-control" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Price</label>
                    <InputNumber @bind-Value="pie.Price" class="form-control" />
                    <ValidationMessage For="() => pie.Price" />
                </div>

                <div class="mb-3 form-check">
                    <InputCheckbox @bind-Value="pie.IsPieOfTheWeek" class="form-check-input" />
                    <label class="form-check-label">Pie of the Week</label>
                </div>

                <div class="mb-3">
                    <label class="form-label">Category ID</label>
                    <InputNumber @bind-Value="pie.CategoryId" class="form-control" />
                </div>

                <button type="submit" class="btn btn-primary">Save</button>
                <a href="@($"/pies/{Id}")" class="btn btn-link">Cancel</a>
            </EditForm>
        </div>
    </div>
}

@code {
    [Parameter] public int Id { get; set; }

    [SupplyParameterFromForm]
    private PieFormModel? pie { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (pie is not null) return;

        var existing = await PieService.GetByIdAsync(Id);
        if (existing is null)
        {
            Navigation.NavigateTo("/pies");
            return;
        }

        pie = new PieFormModel
        {
            Name = existing.Name,
            ShortDescription = existing.ShortDescription,
            Price = existing.Price,
            IsPieOfTheWeek = existing.IsPieOfTheWeek,
            CategoryId = existing.CategoryId
        };
    }

    private async Task HandleSubmit()
    {
        if (pie is null) return;

        var updatedPie = new Pie
        {
            PieId = Id,
            Name = pie.Name,
            ShortDescription = pie.ShortDescription,
            Price = pie.Price,
            IsPieOfTheWeek = pie.IsPieOfTheWeek,
            CategoryId = pie.CategoryId
        };
        await PieService.UpdateAsync(Id, updatedPie);
        Navigation.NavigateTo($"/pies/{Id}");
    }
}
```

> **Checkpoint:** Navigate to a pie's detail page and click Edit. The form should be pre-filled. Changes should be saved and visible on the detail page.

---

## Step 7 — Polish the App

### Update the Home Page

Update `Components/Pages/Home.razor` to welcome users:

```razor
@page "/"

<PageTitle>Pie Shop</PageTitle>

<div class="text-center">
    <h1 class="display-4">Welcome to the Pie Shop!</h1>
    <p class="lead">Browse our delicious selection of pies, add your own, and manage the catalog.</p>
    <a href="/pies" class="btn btn-primary btn-lg mt-3">Browse Pies</a>
</div>
```

### Update the Navbar Brand

In `NavMenu.razor`, update the brand name:

```razor
<a class="navbar-brand" href="">🥧 Pie Shop</a>
```

> **Checkpoint:** The app has a polished home page and clear navigation.

---

## Summary

Congratulations! You have built a **Blazor Web App** that:

- ✅ Connects to the Pie Shop API via a typed `HttpClient` service
- ✅ Lists pies with Bootstrap card layouts
- ✅ Shows pie details with route parameters
- ✅ Creates new pies with validated forms
- ✅ Edits existing pies with pre-populated forms
- ✅ Deletes pies from the detail view
- ✅ Uses Interactive Server rendering for real-time interactivity

### Architecture Recap

```
Blazor Web App (port 5200)          Pie Shop API (port 5100)
┌────────────────────────┐          ┌──────────────────────┐
│ PieList.razor          │          │ GET    /pies          │
│ PieDetail.razor        │  HTTP    │ GET    /pies/{id}     │
│ PieAdd.razor           │ ──────►  │ POST   /pies          │
│ PieEdit.razor          │          │ PUT    /pies/{id}     │
│         │              │          │ DELETE /pies/{id}     │
│ PieApiService.cs       │          │                      │
└────────────────────────┘          └──────────────────────┘
```

### What's Next?

In **Lab 02**, you will build the same Pie Shop frontend as a **.NET MAUI mobile/desktop app**, using the same API — demonstrating how the same backend can serve multiple frontend technologies.
