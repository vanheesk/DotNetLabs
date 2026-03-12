# Lab 3: CORS (Cross-Origin Resource Sharing)

## Objective

Learn how to configure CORS policies to control which origins can access your API.

## Prerequisites

- .NET 10 SDK
- Browser with developer tools (F12)

## Project Structure

- **PieShopApi** — ASP.NET Core Web API on `https://localhost:7043`
- **PieShopBlazorClient** — Blazor Web App on `https://localhost:7282`

## Getting Started

Open **two terminals** side by side.

Terminal 1 (API):
```bash
cd starter/PieShopApi
dotnet run --launch-profile https
```

Terminal 2 (Blazor client):
```bash
cd starter/PieShopBlazorClient
dotnet run --launch-profile https
```

---

## Part 1 — Observing a CORS Failure

### Step 1: Open the Blazor app

Navigate to `https://localhost:7282` in the browser. Open **Dev Tools** (F12) → **Console** tab.

Notice the pie list is **empty** and you see a CORS error in the console:

> Access to fetch at 'https://localhost:7043/pies' from origin 'https://localhost:7282' has been blocked by CORS policy.

### Step 2: Understand the problem

The Blazor `Home.razor` page uses `fetch` to call the API at `https://localhost:7043/pies`. Since the Blazor app runs on `https://localhost:7282` (a different origin), the browser blocks the request.

---

## Part 2 — Adding CORS Policies

### Step 3: Add CORS services in `PieShopApi/Program.cs`

Add three CORS policies before `AddControllers()`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    options.AddPolicy("AllowLocalhost8080", builder =>
        builder.WithOrigins("https://localhost:8080").AllowAnyMethod().AllowAnyHeader());

    options.AddPolicy("AllowLocalhost7282", builder =>
        builder.WithOrigins("https://localhost:7282").AllowAnyMethod().AllowAnyHeader());
});
```

### Step 4: Apply the wrong policy first

Add CORS middleware before `MapControllers()` using the `AllowLocalhost8080` policy (wrong port):

```csharp
app.UseCors("AllowLocalhost8080");
```

### Step 5: Test — CORS still fails

Restart the API and refresh the Blazor app. The pie list is still empty because port 8080 doesn't match the Blazor client's origin (port 7282).

---

## Part 3 — Fixing with the Correct Origin

### Step 6: Switch to the correct policy

Change the CORS policy to `AllowLocalhost7282`:

```csharp
app.UseCors("AllowLocalhost7282");
```

### Step 7: Test — CORS works

Restart the API and refresh the browser. The pie list now loads correctly!

---

## Part 4 — Using AllowAll

### Step 8: Switch to `AllowAll`

Change the CORS policy to `AllowAll`:

```csharp
app.UseCors("AllowAll");
```

This allows any origin. Useful for development but **should not be used in production**.

---

## Verification

When you have completed the lab:

- With `AllowLocalhost8080` → CORS error (pie list empty)
- With `AllowLocalhost7282` → pies load correctly
- With `AllowAll` → pies load correctly (any origin allowed)
