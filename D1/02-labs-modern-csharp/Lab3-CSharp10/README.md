# Lab 3: C# 10 Features

## Objective

Explore features from **C# 10** that reduce boilerplate and improve code organization: file-scoped namespaces, global usings, record structs, lambda improvements, and the `CallerArgumentExpression` attribute.

---

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

```bash
cd starter
dotnet build
```

---

## Exercise 1 – File-Scoped Namespaces

Traditional namespace declarations add a level of indentation around your entire file. **File-scoped namespaces** remove this nesting with a single line.

```csharp
// Before (C# 9 and earlier)
namespace MyApp.Models
{
    public class Customer { }
}

// After (C# 10+)
namespace MyApp.Models;

public class Customer { }
```

### Tasks

1. Open `Models.cs` in the starter project.
2. Convert the traditional namespace block to a **file-scoped namespace** declaration.
3. Ensure the project still builds.

---

## Exercise 2 – Global Usings

**Global usings** let you declare `using` directives that apply to every file in the project, eliminating repetitive imports.

```csharp
// GlobalUsings.cs
global using System.Text.Json;
global using MyApp.Models;
```

### Tasks

1. Open `GlobalUsings.cs` in the starter project.
2. Add a `global using` for `System.Text.Json` so it is available everywhere.
3. In `Program.cs`, use `JsonSerializer.Serialize(...)` without a local `using System.Text.Json;` — it should work because of the global using.

---

## Exercise 3 – Record Structs

C# 10 introduced `record struct` — a **value type** record that provides the same convenience features (value equality, `ToString()`, `with` expressions) as `record class` but allocated on the stack.

```csharp
public record struct Point(double X, double Y);
```

> **Note:** A plain `record struct` is **mutable** by default. Use `readonly record struct` for immutability.

### Tasks

1. Define a `record struct Coordinate(double Latitude, double Longitude)`.
2. Define a `readonly record struct Money(decimal Amount, string Currency)`.
3. Create instances of both, demonstrate value equality and `with` expressions.
4. Show that `Coordinate` is mutable (you can change its properties) but `Money` is not.

---

## Exercise 4 – Lambda Improvements

C# 10 allows the compiler to infer a **natural type** for lambdas (e.g., `Func<>` or `Action<>`), making `var` assignments possible. You can also add **attributes** and **return types** to lambdas.

```csharp
// Natural type inference
var greet = (string name) => $"Hello, {name}!";

// Explicit return type
var parse = int (string s) => int.Parse(s);
```

### Tasks

1. Assign a lambda to a `var` variable that takes a `string` and returns its uppercase version.
2. Assign a lambda with an **explicit return type** that parses a string to an `int`.
3. Call both and print the results.

---

## Exercise 5 – CallerArgumentExpression Attribute

The `[CallerArgumentExpression]` attribute captures the **source text** of an argument, useful for building assertion/guard helpers.

```csharp
public static void ThrowIfNull(
    object? value,
    [CallerArgumentExpression(nameof(value))] string? expression = null)
{
    if (value is null)
        throw new ArgumentNullException(expression);
}
```

### Tasks

1. Create a static `Guard.EnsureNotNull` method that throws `ArgumentNullException` with the caller's expression.
2. Test it with a null variable and observe the exception message includes the variable name.

---

## Wrapping Up

```bash
dotnet run
```

Compare your output with the `solution` folder if needed.
