# Lab 8 – Observability with OpenTelemetry

## Objectives
- Integrate **OpenTelemetry** into an ASP.NET Core Minimal API.
- Capture **metrics**, **logs**, and **traces**.
- Export telemetry to the console (and optionally to a dashboard).

## Prerequisites
- .NET 10 SDK

## Exercises

### Exercise 1 – Add OpenTelemetry Packages
1. Open `starter/` and restore packages.
2. Review the OpenTelemetry packages already referenced in the `.csproj`.

### Exercise 2 – Configure Tracing
1. Register OpenTelemetry tracing in DI.
2. Add the ASP.NET Core instrumentation source.
3. Add a custom `ActivitySource` and create a span inside an endpoint.

### Exercise 3 – Configure Metrics
1. Register OpenTelemetry metrics in DI.
2. Add the ASP.NET Core and runtime instrumentation meters.
3. Create a custom `Counter<int>` and increment it each time an endpoint is called.

### Exercise 4 – Configure Logging
1. Register OpenTelemetry logging.
2. Use `ILogger<T>` inside an endpoint and observe structured log output.

### Exercise 5 – Observe Output
1. Run the application and call the endpoints.
2. Review the console output for traces, metrics, and logs.
3. *(Bonus)* Export to Jaeger using the OTLP exporter.

## Running
```bash
cd starter   # or solution
dotnet run
```
Call `http://localhost:5000/weather` and observe console telemetry output.

## Folder Structure
```
starter/   – Project skeleton with TODOs
solution/  – Complete implementation
```
