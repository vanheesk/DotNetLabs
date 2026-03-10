# Lab 10 – .NET Aspire

## Objectives
- Understand what **.NET Aspire** is and how it improves the inner-loop developer experience.
- See how Aspire replaces the manual **OpenTelemetry** setup from Lab 8 with a single line of code.
- Use the **Aspire Dashboard** to observe traces, metrics, and structured logs.
- Orchestrate **multiple services** with an AppHost.
- Experience **service discovery** for service-to-service HTTP calls.

## Prerequisites
- .NET 10 SDK
- Docker Desktop (the Aspire Dashboard runs as a container)

## Background

Throughout this training you have manually configured DI, middleware, OpenTelemetry, health checks, and HTTP resilience — each time adding packages and writing boilerplate. **.NET Aspire** bundles all of those cross-cutting concerns into reusable defaults and adds an **orchestrator** (AppHost) that starts your services, provisions resources, and launches a developer dashboard — all with a few lines of code.

The lab uses three projects:

| Project | Role |
|---------|------|
| **AppHost** | The orchestrator — defines which services and resources make up your app. |
| **ServiceDefaults** | A shared library that configures OpenTelemetry, health checks, resilience, and service discovery for every service that references it. |
| **ApiService** | A Minimal API (the weather API you've been building all day). |

In the solution you will also find a **CatalogService** that demonstrates service-to-service communication via Aspire's service discovery.

## Exercises

### Exercise 1 – Explore the Aspire Structure

1. Open the `starter/` folder and examine the solution:
   ```
   Lab-AspNet-10.AppHost/        → Orchestrator
   Lab-AspNet-10.ServiceDefaults/ → Shared defaults (OpenTelemetry, health checks, resilience)
   Lab-AspNet-10.ApiService/      → Your weather API
   ```
2. Open `Lab-AspNet-10.AppHost/Program.cs`. Notice the `DistributedApplication.CreateBuilder()` — this is the Aspire equivalent of `WebApplication.CreateBuilder()`, but for the entire distributed app.
3. Open `Lab-AspNet-10.ServiceDefaults/Extensions.cs`. Compare the OpenTelemetry setup here with the manual setup you did in Lab 8 — it's the same concepts (tracing, metrics, logging), but packaged as reusable extension methods.
4. Open `Lab-AspNet-10.ApiService/Program.cs`. Notice the TODOs.

### Exercise 2 – Wire Up ServiceDefaults

In `Lab-AspNet-10.ApiService/Program.cs`:

1. After `WebApplication.CreateBuilder(args)`, add the service defaults:
   ```csharp
   builder.AddServiceDefaults();
   ```
   This single line registers OpenTelemetry (tracing, metrics, logging), health checks, service discovery, and HTTP resilience — everything you configured manually in Labs 4 and 8.

2. After building the app, map the default health-check endpoints:
   ```csharp
   app.MapDefaultEndpoints();
   ```
   This adds `/health` and `/alive` endpoints automatically.

3. Build and verify:
   ```bash
   cd starter/Lab-AspNet-10.ApiService
   dotnet build
   ```

### Exercise 3 – Run with the Aspire Dashboard

1. Start the entire application from the AppHost:
   ```bash
   cd starter/Lab-AspNet-10.AppHost
   dotnet run
   ```
2. The console output will show a URL for the **Aspire Dashboard** (typically `https://localhost:15xxx`). Open it in your browser.
3. In the dashboard, observe:
   - **Resources** — your ApiService is listed with its status and endpoints.
   - **Structured Logs** — click through to see the structured log output (no more console noise!).
   - **Traces** — make a few requests to `http://localhost:5xxx/weather` and watch distributed traces appear.
   - **Metrics** — view ASP.NET Core and runtime metrics in real time.
4. Compare this experience with the console-based OpenTelemetry output from Lab 8.

### Exercise 4 – Add a Second Service

1. Create a new folder `Lab-AspNet-10.CatalogService/` next to the other projects.
2. Create a `Lab-AspNet-10.CatalogService.csproj`:
   ```xml
   <Project Sdk="Microsoft.NET.Sdk.Web">
     <PropertyGroup>
       <TargetFramework>net10.0</TargetFramework>
       <Nullable>enable</Nullable>
       <ImplicitUsings>enable</ImplicitUsings>
     </PropertyGroup>
     <ItemGroup>
       <ProjectReference Include="..\Lab-AspNet-10.ServiceDefaults\Lab-AspNet-10.ServiceDefaults.csproj" />
     </ItemGroup>
   </Project>
   ```
3. Create a `Program.cs` for the CatalogService:
   ```csharp
   var builder = WebApplication.CreateBuilder(args);
   builder.AddServiceDefaults();

   var app = builder.Build();
   app.MapDefaultEndpoints();

   app.MapGet("/products", () => new[]
   {
       new { Id = 1, Name = "Widget", Price = 9.99 },
       new { Id = 2, Name = "Gizmo", Price = 14.99 },
       new { Id = 3, Name = "Thingamajig", Price = 24.99 }
   });

   app.Run();
   ```
4. Register it in the AppHost (`Lab-AspNet-10.AppHost/Program.cs`):
   ```csharp
   var catalogService = builder.AddProject<Projects.Lab_AspNet_10_CatalogService>("catalogservice");
   ```
   Don't forget to also add a `<ProjectReference>` to the CatalogService in the AppHost `.csproj`.
5. Add the CatalogService to the solution file (`Lab-AspNet-10.slnx`):
   ```xml
   <Project Path="Lab-AspNet-10.CatalogService/Lab-AspNet-10.CatalogService.csproj" />
   ```
6. Run the AppHost again — both services now appear in the dashboard.

### Exercise 5 – Service Discovery

Instead of hard-coding URLs, Aspire's **service discovery** lets services find each other by name.

1. In the AppHost, tell Aspire that ApiService needs to talk to CatalogService:
   ```csharp
   var apiService = builder.AddProject<Projects.Lab_AspNet_10_ApiService>("apiservice")
       .WithReference(catalogService);
   ```
2. In ApiService `Program.cs`, register `IHttpClientFactory`:
   ```csharp
   builder.Services.AddHttpClient();
   ```
3. Add an endpoint that calls the CatalogService by its **service name** (not a URL):
   ```csharp
   app.MapGet("/catalog", async (IHttpClientFactory httpClientFactory) =>
   {
       var client = httpClientFactory.CreateClient();
       var response = await client.GetStringAsync("http://catalogservice/products");
       return Results.Content(response, "application/json");
   });
   ```
   Notice the URL `http://catalogservice/products` — Aspire resolves `catalogservice` to the actual address at runtime.
4. Run the AppHost and call `/catalog` on the ApiService. You should see the products from the CatalogService.
5. Open the **Traces** tab in the dashboard — you'll see a distributed trace spanning both services.

## Key Takeaways

| What you did manually | What Aspire gives you |
|-----------------------|-----------------------|
| Lab 4: Registered services in DI | ServiceDefaults registers OpenTelemetry, resilience, and service discovery in one call |
| Lab 8: Configured OpenTelemetry with console exporters | Aspire auto-configures OTLP export to the dashboard |
| Hard-coded `http://localhost:5001` for service calls | Service discovery resolves by name (`http://catalogservice`) |
| No dashboard — read console output | Rich dashboard with traces, logs, metrics, and resource health |
| Started each service manually | AppHost starts everything with a single `dotnet run` |

## Running
```bash
cd starter/Lab-AspNet-10.AppHost   # or solution/Lab-AspNet-10.AppHost
dotnet run
```
Open the dashboard URL from the console output. Call `http://localhost:5xxx/weather` and observe telemetry.

## Folder Structure
```
starter/   – Multi-project skeleton with TODOs
solution/  – Complete implementation with two services and service discovery
```
