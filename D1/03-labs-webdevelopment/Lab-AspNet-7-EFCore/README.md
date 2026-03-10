# Lab 7 – Data Access with EF Core 10

## Objectives
- Add **Entity Framework Core** with the **SQLite** provider to a Minimal API.
- Define a `DbContext` and model entities.
- Implement CRUD endpoints for a `Todo` entity.
- Explore modern EF Core features: complex types, many-to-many.

## Prerequisites
- .NET 10 SDK
- `dotnet ef` CLI tool (`dotnet tool install --global dotnet-ef`)

## Exercises

### Exercise 1 – Set Up the DbContext
1. Open `starter/` and restore packages.
2. Create a `Todo` class with properties: `Id`, `Title`, `IsComplete`, `DueDate`.
3. Create a `TodoDb : DbContext` and register a `DbSet<Todo>`.
4. Register the context in DI using `AddDbContext<TodoDb>` with SQLite.
5. Create and apply an initial migration.

### Exercise 2 – CRUD Endpoints
1. Add `GET /todos` – return all todos.
2. Add `GET /todos/{id}` – return a single todo or 404.
3. Add `POST /todos` – create a new todo.
4. Add `PUT /todos/{id}` – update a todo.
5. Add `DELETE /todos/{id}` – delete a todo.

### Exercise 3 – Filtering & Route Groups
1. Add a query parameter `?complete=true|false` to `GET /todos`.
2. Put all todo endpoints in a `MapGroup("/todos")`.

### Exercise 4 – Complex Type (Bonus)
1. Create an `Address` complex type (no key) and add it to a `Person` entity.
2. Add a few endpoints to store and retrieve persons with addresses.

## Running
```bash
cd starter   # or solution
dotnet ef migrations add Init
dotnet ef database update
dotnet run
```
Open `http://localhost:5000/swagger` to explore.

## Folder Structure
```
starter/   – Project skeleton with TODOs
solution/  – Complete implementation
```
