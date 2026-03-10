# Lab 5: OpenAPI & Endpoint Filters

## Objective

Enable **OpenAPI 3.1/Swagger** documentation, build **endpoint filters** for validation, and use **ProblemDetails** for consistent error responses.

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Enable OpenAPI & Swagger

### Tasks

1. Ensure the project has Swagger packages and services registered.
2. Add metadata to endpoints using `.WithName()`, `.WithSummary()`, `.WithDescription()`, and `.Produces<T>()`.
3. Navigate to `/swagger` and verify documentation is generated.

---

## Exercise 2 – Endpoint Filters for Validation

**Endpoint filters** intercept requests before the endpoint handler runs. They are ideal for validation, logging, or transforming results.

```csharp
app.MapPost("/items", (Item item) => ...)
   .AddEndpointFilter(async (ctx, next) =>
   {
       var item = ctx.GetArgument<Item>(0);
       if (string.IsNullOrWhiteSpace(item.Name))
           return Results.ValidationProblem(
               new Dictionary<string, string[]>
               {
                   ["Name"] = ["Name is required"]
               });
       return await next(ctx);
   });
```

### Tasks

1. Create a `POST /products` endpoint that accepts a `Product` record.
2. Add an endpoint filter that validates:
   - `Name` is not empty
   - `Price` is greater than 0
3. Return `Results.ValidationProblem(...)` with field-specific error messages on failure.
4. Test with valid and invalid payloads from Swagger.

---

## Exercise 3 – Reusable Endpoint Filters

Instead of inline filters, create a reusable filter class.

```csharp
public class ValidationFilter<T> : IEndpointFilter { ... }
```

### Tasks

1. Create a `ValidationFilter<Product>` that implements `IEndpointFilter`.
2. Move the validation logic from Exercise 2 into the filter class.
3. Apply it using `.AddEndpointFilter<ValidationFilter<Product>>()`.

---

## Exercise 4 – ProblemDetails for Error Responses

`ProblemDetails` (RFC 9457) provides a standardised JSON error format.

### Tasks

1. Register `builder.Services.AddProblemDetails()`.
2. Configure exception handling to return `ProblemDetails`:
   ```csharp
   app.UseExceptionHandler();
   app.UseStatusCodePages();
   ```
3. Add an endpoint that returns `Results.Problem("Something went wrong", statusCode: 500)`.
4. Verify the response follows the ProblemDetails JSON format.
5. Test a 404 (unknown route) and verify it also returns ProblemDetails.

---

## Exercise 5 – Scalar as an Alternative to Swagger UI

[Scalar](https://github.com/scalar/scalar) is a modern, open-source OpenAPI UI that provides a cleaner developer experience than Swagger UI. It reads the same OpenAPI document, so both can coexist.

### Tasks

1. Add the **Scalar.AspNetCore** NuGet package:
   ```bash
   dotnet add package Scalar.AspNetCore
   ```
2. Add the Scalar UI middleware in `Program.cs`:
   ```csharp
   using Scalar.AspNetCore;

   app.MapScalarApiReference();   // serves at /scalar/v1
   ```
3. Run the app and navigate to `/scalar/v1`.
4. Compare the Scalar UI with Swagger UI (`/swagger`):
   - Explore the different layout and navigation.
   - Try the built-in "Try it" feature to send requests.
   - Notice the auto-generated code samples (cURL, Python, JS, etc.).
5. *(Optional)* Customise Scalar with options:
   ```csharp
   app.MapScalarApiReference(options =>
   {
       options.Title = "Lab 5 API";
       options.Theme = ScalarTheme.BluePlanet;
   });
   ```

> **Tip:** You don't have to choose — many teams serve both Swagger UI and Scalar side-by-side during development.

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed.
