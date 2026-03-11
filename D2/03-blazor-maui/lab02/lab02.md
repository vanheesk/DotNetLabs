# Lab 02: Building a .NET MAUI Frontend for the Pie Shop API

## Lab Overview

In Module 2.3, you built a **Pie Shop API** using Minimal APIs. In Lab 01, you built a **Blazor web frontend** for that same API. Now you will build a **native mobile/desktop application** using **.NET MAUI** that consumes the exact same API.

This demonstrates a key architectural benefit: **one API, multiple frontends**. The same Pie Shop API serves both a web app (Blazor) and a native app (MAUI).

## Learning Objectives

By the end of this lab, you will be able to:

- Create a .NET MAUI project and understand its structure
- Implement the MVVM pattern using `CommunityToolkit.Mvvm`
- Build XAML-based user interfaces with data binding
- Use Shell navigation with route parameters
- Call a REST API from a MAUI application using `HttpClient`
- Register services and view models with dependency injection

## Prerequisites

- .NET 10 SDK installed with the **MAUI workload** (`dotnet workload install maui`)
- Visual Studio 2022+ or VS Code with .NET MAUI extension
- **Completed Module 2.3** (conceptual understanding of the Pie Shop API)

> **Starter projects available.** The `starter/` folder contains:
> - `PieShopApi/` — The Pie Shop API from Module 2.3 (simplified, with seed data and CORS)
> - `PieShopMaui/` — A scaffolded .NET MAUI App ready for you to extend
>
> If you prefer to start fresh, the `solution/` folder contains the complete working solution.

---

## Lab Setup

### Step 0a — Verify MAUI Workload

Ensure the MAUI workload is installed:

```shell
dotnet workload list
```

If `maui` is not listed, install it:

```shell
dotnet workload install maui
```

### Step 0b — Start the API

The Pie Shop API must be running for the MAUI app to work. Open a terminal:

```shell
cd starter/PieShopApi
dotnet run
```

Verify the API is running at `http://localhost:5100/pies`.

> **Keep this terminal running** throughout the entire lab.

Open a **second terminal** for running the MAUI app.

---

## Step 1 — Explore the MAUI Starter Project

Open the `starter/PieShopMaui/` project.

### Build and Run (Windows)

```shell
cd starter/PieShopMaui
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

You should see the default MAUI template app with a "Hello, World!" message and a click counter.

### Understand the Project Structure

| File / Folder                | Purpose                              |
| ---------------------------- | ------------------------------------ |
| `MauiProgram.cs`             | App builder, DI, service registration|
| `App.xaml / .cs`             | Application root + resources         |
| `AppShell.xaml / .cs`        | Navigation structure (Shell)         |
| `MainPage.xaml / .cs`        | Default landing page                 |
| `Platforms/`                 | Platform-specific code               |
| `Resources/`                 | Icons, fonts, images, styles         |

### Add Required NuGet Package

Add the `CommunityToolkit.Mvvm` package for MVVM support:

```shell
dotnet add package CommunityToolkit.Mvvm
```

Also add `Microsoft.Extensions.Http` for `HttpClient` factory support:

```shell
dotnet add package Microsoft.Extensions.Http
```

---

## Step 2 — Create the Models and API Service

### Create the Pie Model

Create a new file `Models/Pie.cs`:

```csharp
namespace PieShopMaui.Models;

public class Pie
{
    public int PieId { get; set; }
    public string Name { get; set; } = "";
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public bool IsPieOfTheWeek { get; set; }
    public int CategoryId { get; set; }
}
```

### Create the PieApiService

Create `Services/PieApiService.cs`:

```csharp
using System.Net.Http.Json;
using PieShopMaui.Models;

namespace PieShopMaui.Services;

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

### Register Services in MauiProgram.cs

Open `MauiProgram.cs` and add the following **before** `return builder.Build();`:

```csharp
using PieShopMaui.Services;

// ... inside CreateMauiApp():

builder.Services.AddHttpClient<PieApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5100");
});
```

> **Note for Android emulator:** If running on Android, use `http://10.0.2.2:5100` instead. The address `10.0.2.2` maps to the host machine's localhost from the emulator.

> **Checkpoint:** The project should build successfully with the service and model in place.

---

## Step 3 — Build the Pie List Page with MVVM

### Create the ViewModel

Create `ViewModels/PieListViewModel.cs`:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PieShopMaui.Models;
using PieShopMaui.Services;

namespace PieShopMaui.ViewModels;

public partial class PieListViewModel(PieApiService pieService) : ObservableObject
{
    [ObservableProperty]
    private List<Pie> pies = [];

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand]
    private async Task LoadPiesAsync()
    {
        IsLoading = true;
        Pies = await pieService.GetAllAsync();
        IsLoading = false;
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Pie pie)
    {
        await Shell.Current.GoToAsync($"piedetail?id={pie.PieId}");
    }

    [RelayCommand]
    private async Task GoToAddAsync()
    {
        await Shell.Current.GoToAsync("pieedit");
    }
}
```

### Create the Page (XAML)

Create `Views/PieListPage.xaml`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:PieShopMaui.ViewModels"
             x:Class="PieShopMaui.Views.PieListPage"
             x:DataType="vm:PieListViewModel"
             Title="Our Pies">

    <Grid RowDefinitions="Auto,*" Padding="10">
        <Button Text="+ Add New Pie"
                Command="{Binding GoToAddCommand}"
                BackgroundColor="#28a745"
                TextColor="White"
                Margin="0,0,0,10" />

        <ActivityIndicator Grid.Row="1"
                           IsRunning="{Binding IsLoading}"
                           IsVisible="{Binding IsLoading}"
                           HorizontalOptions="Center" />

        <CollectionView Grid.Row="1"
                        ItemsSource="{Binding Pies}"
                        SelectionMode="None">
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="models:Pie"
                              xmlns:models="clr-namespace:PieShopMaui.Models">
                    <Border Padding="15" Margin="0,5"
                            Stroke="LightGray"
                            StrokeShape="RoundRectangle 8">
                        <Border.GestureRecognizers>
                            <TapGestureRecognizer
                                Command="{Binding Source={RelativeSource AncestorType={x:Type vm:PieListViewModel}}, Path=GoToDetailCommand}"
                                CommandParameter="{Binding .}" />
                        </Border.GestureRecognizers>
                        <Grid ColumnDefinitions="*,Auto">
                            <StackLayout>
                                <Label Text="{Binding Name}"
                                       FontSize="18" FontAttributes="Bold" />
                                <Label Text="{Binding ShortDescription}"
                                       TextColor="Gray" FontSize="14" />
                            </StackLayout>
                            <StackLayout Grid.Column="1" VerticalOptions="Center">
                                <Label Text="{Binding Price, StringFormat='{0:C}'}"
                                       FontSize="16" FontAttributes="Bold"
                                       HorizontalTextAlignment="End" />
                                <Label Text="⭐ Pie of the Week"
                                       IsVisible="{Binding IsPieOfTheWeek}"
                                       FontSize="12" TextColor="Orange"
                                       HorizontalTextAlignment="End" />
                            </StackLayout>
                        </Grid>
                    </Border>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
    </Grid>
</ContentPage>
```

### Create the Code-Behind

Create `Views/PieListPage.xaml.cs`:

```csharp
using PieShopMaui.ViewModels;

namespace PieShopMaui.Views;

public partial class PieListPage : ContentPage
{
    private readonly PieListViewModel _viewModel;

    public PieListPage(PieListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPiesCommand.ExecuteAsync(null);
    }
}
```

### Register in MauiProgram.cs

Add these registrations:

```csharp
using PieShopMaui.ViewModels;
using PieShopMaui.Views;

builder.Services.AddTransient<PieListViewModel>();
builder.Services.AddTransient<PieListPage>();
```

### Update AppShell.xaml

Replace the existing `ShellContent` with:

```xml
<ShellContent Title="Pies"
              Route="PieList"
              ContentTemplate="{DataTemplate views:PieListPage}" />
```

Add the namespace at the top of AppShell.xaml:

```xml
xmlns:views="clr-namespace:PieShopMaui.Views"
```

> **Checkpoint:** Run the app. You should see a list of pies loaded from the API.

---

## Step 4 — Build the Pie Detail Page

### Create the ViewModel

Create `ViewModels/PieDetailViewModel.cs`:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PieShopMaui.Models;
using PieShopMaui.Services;

namespace PieShopMaui.ViewModels;

[QueryProperty(nameof(PieId), "id")]
public partial class PieDetailViewModel(PieApiService pieService) : ObservableObject
{
    [ObservableProperty]
    private int pieId;

    [ObservableProperty]
    private Pie? pie;

    partial void OnPieIdChanged(int value)
    {
        Task.Run(() => LoadPieAsync(value));
    }

    private async Task LoadPieAsync(int id)
    {
        Pie = await pieService.GetByIdAsync(id);
    }

    [RelayCommand]
    private async Task EditAsync()
    {
        await Shell.Current.GoToAsync($"pieedit?id={PieId}");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Delete Pie", $"Are you sure you want to delete {Pie?.Name}?", "Yes", "No");

        if (confirm)
        {
            await pieService.DeleteAsync(PieId);
            await Shell.Current.GoToAsync("..");
        }
    }
}
```

### Create the Page

Create `Views/PieDetailPage.xaml`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:PieShopMaui.ViewModels"
             x:Class="PieShopMaui.Views.PieDetailPage"
             x:DataType="vm:PieDetailViewModel"
             Title="{Binding Pie.Name}">

    <ScrollView Padding="20">
        <StackLayout Spacing="15">
            <Border Padding="20" Stroke="LightGray"
                    StrokeShape="RoundRectangle 8">
                <StackLayout Spacing="10">
                    <Label Text="{Binding Pie.Name}"
                           FontSize="24" FontAttributes="Bold" />
                    <Label Text="{Binding Pie.ShortDescription}"
                           FontSize="16" TextColor="Gray" />
                    <BoxView HeightRequest="1" Color="LightGray" />
                    <Label Text="{Binding Pie.Price, StringFormat='Price: {0:C}'}"
                           FontSize="18" />
                    <Label Text="{Binding Pie.CategoryId, StringFormat='Category: {0}'}"
                           FontSize="16" />
                    <Label Text="⭐ Pie of the Week!"
                           IsVisible="{Binding Pie.IsPieOfTheWeek}"
                           FontSize="16" TextColor="Orange"
                           FontAttributes="Bold" />
                </StackLayout>
            </Border>

            <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                <Button Text="Edit"
                        Command="{Binding EditCommand}"
                        BackgroundColor="#0d6efd"
                        TextColor="White" />
                <Button Grid.Column="1"
                        Text="Delete"
                        Command="{Binding DeleteCommand}"
                        BackgroundColor="#dc3545"
                        TextColor="White" />
            </Grid>
        </StackLayout>
    </ScrollView>
</ContentPage>
```

Create `Views/PieDetailPage.xaml.cs`:

```csharp
using PieShopMaui.ViewModels;

namespace PieShopMaui.Views;

public partial class PieDetailPage : ContentPage
{
    public PieDetailPage(PieDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
```

### Register and Configure Route

In `MauiProgram.cs`:

```csharp
builder.Services.AddTransient<PieDetailViewModel>();
builder.Services.AddTransient<PieDetailPage>();
```

In `AppShell.xaml.cs` constructor (after `InitializeComponent()`):

```csharp
Routing.RegisterRoute("piedetail", typeof(PieDetailPage));
```

> **Checkpoint:** Tap a pie in the list to see its details. Delete should navigate back.

---

## Step 5 — Build the Add/Edit Pie Page

### Create the ViewModel

Create `ViewModels/PieEditViewModel.cs`:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PieShopMaui.Models;
using PieShopMaui.Services;

namespace PieShopMaui.ViewModels;

[QueryProperty(nameof(PieId), "id")]
public partial class PieEditViewModel(PieApiService pieService) : ObservableObject
{
    [ObservableProperty]
    private int pieId;

    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private string shortDescription = "";

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private bool isPieOfTheWeek;

    [ObservableProperty]
    private int categoryId = 1;

    [ObservableProperty]
    private string pageTitle = "Add a New Pie";

    public bool IsEditing => PieId > 0;

    partial void OnPieIdChanged(int value)
    {
        if (value > 0)
        {
            PageTitle = "Edit Pie";
            Task.Run(() => LoadPieAsync(value));
        }
    }

    private async Task LoadPieAsync(int id)
    {
        var pie = await pieService.GetByIdAsync(id);
        if (pie is null) return;

        Name = pie.Name;
        ShortDescription = pie.ShortDescription ?? "";
        Price = pie.Price;
        IsPieOfTheWeek = pie.IsPieOfTheWeek;
        CategoryId = pie.CategoryId;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Validation", "Name is required.", "OK");
            return;
        }

        var pie = new Pie
        {
            PieId = PieId,
            Name = Name,
            ShortDescription = ShortDescription,
            Price = Price,
            IsPieOfTheWeek = IsPieOfTheWeek,
            CategoryId = CategoryId
        };

        if (IsEditing)
        {
            await pieService.UpdateAsync(PieId, pie);
        }
        else
        {
            await pieService.CreateAsync(pie);
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
```

### Create the Page

Create `Views/PieEditPage.xaml`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:PieShopMaui.ViewModels"
             x:Class="PieShopMaui.Views.PieEditPage"
             x:DataType="vm:PieEditViewModel"
             Title="{Binding PageTitle}">

    <ScrollView Padding="20">
        <StackLayout Spacing="15">
            <Border Padding="20" Stroke="LightGray"
                    StrokeShape="RoundRectangle 8">
                <StackLayout Spacing="15">
                    <StackLayout>
                        <Label Text="Name" FontAttributes="Bold" />
                        <Entry Text="{Binding Name}" Placeholder="Enter pie name" />
                    </StackLayout>

                    <StackLayout>
                        <Label Text="Description" FontAttributes="Bold" />
                        <Entry Text="{Binding ShortDescription}"
                               Placeholder="Short description" />
                    </StackLayout>

                    <StackLayout>
                        <Label Text="Price" FontAttributes="Bold" />
                        <Entry Text="{Binding Price}" Keyboard="Numeric"
                               Placeholder="0.00" />
                    </StackLayout>

                    <Grid ColumnDefinitions="*,Auto" Padding="0">
                        <Label Text="Pie of the Week"
                               VerticalTextAlignment="Center"
                               FontAttributes="Bold" />
                        <Switch Grid.Column="1"
                                IsToggled="{Binding IsPieOfTheWeek}" />
                    </Grid>

                    <StackLayout>
                        <Label Text="Category ID" FontAttributes="Bold" />
                        <Entry Text="{Binding CategoryId}" Keyboard="Numeric"
                               Placeholder="1" />
                    </StackLayout>
                </StackLayout>
            </Border>

            <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                <Button Text="Save"
                        Command="{Binding SaveCommand}"
                        BackgroundColor="#28a745"
                        TextColor="White" />
                <Button Grid.Column="1"
                        Text="Cancel"
                        Command="{Binding CancelCommand}"
                        BackgroundColor="#6c757d"
                        TextColor="White" />
            </Grid>
        </StackLayout>
    </ScrollView>
</ContentPage>
```

Create `Views/PieEditPage.xaml.cs`:

```csharp
using PieShopMaui.ViewModels;

namespace PieShopMaui.Views;

public partial class PieEditPage : ContentPage
{
    public PieEditPage(PieEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
```

### Register and Configure Route

In `MauiProgram.cs`:

```csharp
builder.Services.AddTransient<PieEditViewModel>();
builder.Services.AddTransient<PieEditPage>();
```

In `AppShell.xaml.cs` constructor:

```csharp
Routing.RegisterRoute("pieedit", typeof(PieEditPage));
```

> **Checkpoint:** You can now:
> - Tap "Add New Pie" from the list to create a pie
> - Tap "Edit" from the detail page to modify a pie
> - Both navigate back to the previous page on save

---

## Step 6 — Final Registration and Cleanup

### Complete MauiProgram.cs

Make sure `MauiProgram.cs` has all registrations:

```csharp
using Microsoft.Extensions.Logging;
using PieShopMaui.Services;
using PieShopMaui.ViewModels;
using PieShopMaui.Views;

namespace PieShopMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // API Service
        builder.Services.AddHttpClient<PieApiService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5100");
        });

        // ViewModels
        builder.Services.AddTransient<PieListViewModel>();
        builder.Services.AddTransient<PieDetailViewModel>();
        builder.Services.AddTransient<PieEditViewModel>();

        // Pages
        builder.Services.AddTransient<PieListPage>();
        builder.Services.AddTransient<PieDetailPage>();
        builder.Services.AddTransient<PieEditPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
```

### Remove Default MainPage

Once you have the Pie List page working as the shell content, you can remove the original `MainPage.xaml` and `MainPage.xaml.cs` files, as they are replaced by your new pages.

> **Checkpoint:** Run the complete app. You should be able to browse, view, create, edit, and delete pies — all from the MAUI app.

---

## Summary

Congratulations! You have built a **.NET MAUI application** that:

- ✅ Connects to the Pie Shop API via a typed `HttpClient` service
- ✅ Lists pies using `CollectionView` with data binding
- ✅ Shows pie details using Shell navigation with query parameters
- ✅ Creates new pies with form entry controls
- ✅ Edits existing pies with pre-populated forms
- ✅ Deletes pies with confirmation dialogs
- ✅ Uses MVVM with `CommunityToolkit.Mvvm` for clean architecture

### Architecture Recap

```
.NET MAUI App                           Pie Shop API (port 5100)
┌────────────────────────┐              ┌──────────────────────┐
│ PieListPage            │              │ GET    /pies          │
│ PieDetailPage          │    HTTP      │ GET    /pies/{id}     │
│ PieEditPage            │  ────────►   │ POST   /pies          │
│                        │              │ PUT    /pies/{id}     │
│ PieApiService          │              │ DELETE /pies/{id}     │
│ ViewModels (MVVM)      │              │                      │
└────────────────────────┘              └──────────────────────┘
```

### Key Comparison: Blazor vs MAUI

| Aspect            | Blazor (Lab 01)            | MAUI (Lab 02)              |
| ----------------- | -------------------------- | -------------------------- |
| UI Language       | Razor (HTML + C#)          | XAML                       |
| Pattern           | Code-behind / inject       | MVVM                       |
| Rendering         | Browser (via SignalR)      | Native platform controls   |
| Navigation        | URL-based routing          | Shell URI navigation       |
| Forms             | EditForm + validation      | Entry + ViewModel logic    |
| Target            | Web browsers               | Mobile + Desktop           |
| API Integration   | Same `HttpClient` patterns | Same `HttpClient` patterns |

Both frontends consume the **same API** — proving that a well-designed API backend enables multiple frontend experiences.
