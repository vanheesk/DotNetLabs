# Lab 1: Project Templates & Quickstart

## Objective

Create your first **ASP.NET Core Web API** project, add a simple endpoint, and explore the built-in Swagger/OpenAPI documentation.

---

## Prerequisites

- .NET 10 SDK installed
- Visual Studio 2022+ or VS Code with C# Dev Kit

---

## Exercise 1 – Create the Project

You can create the project using the CLI or Visual Studio.

**Using the CLI:**

```bash
dotnet new webapi -n Lab1-QuickStart --no-openapi false
```

Or simply open the pre-created project in the `starter` folder.

### Tasks

1. Open the `starter` folder in your editor.
2. Examine `Program.cs` — notice the **minimal hosting model** (no `Startup.cs`).
3. Build and run the project:
   ```bash
   dotnet run
   ```
4. Open the URL shown in the terminal (usually `http://localhost:5000` or similar).

---

## Exercise 2 – Add a Hello Endpoint

### Tasks

1. In `Program.cs`, add a `GET /hello/{name}` endpoint that returns a greeting:
   ```csharp
   app.MapGet("/hello/{name}", (string name) => $"Hello, {name}!");
   ```
2. Run the project and test the endpoint in your browser or with `curl`:
   ```bash
   curl http://localhost:5000/hello/World
   ```

---

## Exercise 3 – Explore Swagger/OpenAPI

ASP.NET Core Web API projects include **OpenAPI** support by default.

### Tasks

1. Run the project and navigate to `/openapi/v1.json` to see the raw OpenAPI document.
2. Add the **Swagger UI** NuGet package to get an interactive explorer:
   ```bash
   dotnet add package Swashbuckle.AspNetCore
   ```
3. In `Program.cs`, register and use Swagger:
   ```csharp
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();
   // ...
   app.UseSwagger();
   app.UseSwaggerUI();
   ```
4. Navigate to `/swagger` to explore your API interactively.
5. Test each endpoint from the Swagger UI.

---

## Exercise 4 – Add More Endpoints

### Tasks

1. Add a `GET /time` endpoint that returns the current UTC time.
2. Add a `GET /random/{min}/{max}` endpoint that returns a random integer.
3. Add a `POST /echo` endpoint that accepts a JSON body and echoes it back.
4. Verify all endpoints appear in Swagger and test them.

---

## Wrapping Up

```bash
dotnet run
```

Compare your code with the `solution` folder if you get stuck. You now have a running ASP.NET Core Web API with multiple endpoints and Swagger documentation.
