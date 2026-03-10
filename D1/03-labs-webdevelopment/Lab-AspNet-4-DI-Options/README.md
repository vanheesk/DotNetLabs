# Lab 4: Dependency Injection & Options Pattern

## Objective

Register and inject services using ASP.NET Core's built-in **Dependency Injection (DI)** container. Bind configuration settings using the **Options pattern** and override values per environment.

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs from Labs 1–3

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Register and Inject a Service

### Concepts

ASP.NET Core has three DI lifetimes:

| Lifetime | Registration | Behaviour |
|----------|-------------|-----------|
| **Transient** | `AddTransient<T>()` | New instance every time |
| **Scoped** | `AddScoped<T>()` | One instance per request |
| **Singleton** | `AddSingleton<T>()` | One instance for the app |

### Tasks

1. Define an `IClock` interface with a `DateTime Now { get; }` property.
2. Implement `SystemClock : IClock` that returns `DateTime.UtcNow`.
3. Register it as a singleton: `builder.Services.AddSingleton<IClock, SystemClock>();`
4. Create a `GET /time` endpoint that injects `IClock` and returns the current time.

---

## Exercise 2 – Understand Lifetimes

### Tasks

1. Create a `RequestCounter` service that has a GUID set in its constructor.
2. Register it as both **Transient** and **Scoped** (in separate calls, under different interfaces or names).
3. Create an endpoint that resolves the service twice in the same request and prints both GUIDs.
4. Observe that:
   - Transient gives different GUIDs each time
   - Scoped gives the same GUID within one request

---

## Exercise 3 – Options Pattern

The Options pattern binds configuration sections to strongly-typed classes.

```csharp
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));
```

### Tasks

1. Add an `"App"` section to `appsettings.json`:
   ```json
   {
     "App": {
       "Title": "Lab 4 API",
       "MaxPageSize": 50,
       "EnableFeatureX": false
     }
   }
   ```
2. Create an `AppSettings` class with matching properties.
3. Bind the configuration: `builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));`
4. Create a `GET /settings` endpoint that injects `IOptions<AppSettings>` and returns the settings.

---

## Exercise 4 – Environment Overrides

### Tasks

1. Create `appsettings.Development.json` that overrides `EnableFeatureX` to `true`.
2. Run the app in Development mode and verify the override at `/settings`.
3. Run in Production mode and verify the original value.

> **Tip:** Set the environment with `ASPNETCORE_ENVIRONMENT=Development dotnet run`

---

## Exercise 5 – IOptionsMonitor for Live Reload

`IOptionsMonitor<T>` detects changes to configuration files at runtime without restarting.

### Tasks

1. Create a `GET /settings/live` endpoint that uses `IOptionsMonitor<AppSettings>`.
2. While the app is running, change a value in `appsettings.json`.
3. Call the endpoint again — the value should reflect the change without restart.

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed.
