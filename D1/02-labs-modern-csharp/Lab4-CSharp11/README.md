# Lab 4: C# 11 Features

## Objective

Explore features from **C# 11**: raw string literals, list/slice patterns, required members, and generic math. These features improve readability, enforce initialization contracts, and enable type-safe numeric algorithms.

---

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

```bash
cd starter
dotnet build
```

---

## Exercise 1 – Raw String Literals

**Raw string literals** use `"""` (three or more double quotes) to define strings that can span multiple lines and include special characters without escaping.

```csharp
var text = """
    This is a raw string literal.
    It preserves "quotes" and whitespace.
    No need to escape \backslashes\.
    """;
```

The indentation of the closing `"""` determines how much leading whitespace is trimmed.

### Tasks

1. Create a raw string literal containing a multi-line JSON object (name, age, city).
2. Create a raw string literal with interpolation (`$"""..."""`) that embeds a variable.
3. Print both strings to verify formatting.

---

## Exercise 2 – List and Slice Patterns

**List patterns** let you match the contents of arrays and lists. **Slice patterns** (`..`) match zero or more elements.

```csharp
int[] arr = [1, 2, 3];

var result = arr switch
{
    [1, 2, 3] => "Exact match",
    [1, ..] => "Starts with 1",
    [.., 3] => "Ends with 3",
    _ => "No match"
};
```

### Tasks

1. Write a method `DescribeArray(int[] arr)` using list patterns in a switch expression:
   - Empty array `[]` → `"Empty"`
   - Single element `[var x]` → `"Single: {x}"`
   - Starts with 1, 2 `[1, 2, ..]` → `"Starts with 1, 2"`
   - Three elements `[var a, var b, var c]` → `"Triple: {a}, {b}, {c}"`
   - Discard pattern `_` → `"Other"`
2. Test with arrays: `[]`, `[42]`, `[1, 2, 3, 4]`, `[7, 8, 9]`, `[5, 6, 7, 8, 9]`.

---

## Exercise 3 – Required Members

The `required` modifier forces callers to initialize a property via object initializer. This catches missing initialization at **compile time**.

```csharp
public class User
{
    public required string Username { get; init; }
    public required string Email { get; init; }
}
```

### Tasks

1. Define a `User` class with `required` properties: `Username` (string) and `Email` (string). Use `init` setters.
2. Create a valid `User` with both properties set.
3. Try creating a `User` without setting one of the required properties — observe the compile error (comment it out after testing).
4. Print the user.

---

## Exercise 4 – Generic Math

**Generic math** uses the `INumber<T>` interface to write numeric algorithms that work with any number type (`int`, `double`, `decimal`, etc.).

```csharp
T Add<T>(T a, T b) where T : INumber<T> => a + b;
```

### Tasks

1. Write a generic method `T Sum<T>(params T[] values) where T : INumber<T>` that sums all values.
2. Write a generic method `T Average<T>(params T[] values) where T : INumber<T>` that computes the average.
3. Test both with `int`, `double`, and `decimal` arrays.

> **Hint:** Use `T.Zero` for the initial sum and `T.CreateChecked(values.Length)` to convert the count.

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed.
