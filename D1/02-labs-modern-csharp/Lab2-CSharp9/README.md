# Lab 2: C# 9 Features

## Objective

Explore key features introduced in **C# 9**: records, enhanced pattern matching, top-level statements, and init-only setters. These features promote **immutable design** and **cleaner code**.

---

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

```bash
cd starter
dotnet build
```

---

## Exercise 1 – Top-Level Statements

C# 9 introduced **top-level statements**, which let you write executable code without the boilerplate of a `Main` method and `Program` class. The starter file already uses this style — notice how there is no `class Program` or `static void Main`.

### Tasks

1. Observe that the starter `Program.cs` has no `Main` method — the code runs directly.
2. No changes needed; just understand that `args` is implicitly available if needed.

---

## Exercise 2 – Init-Only Setters

**Init-only setters** (`init`) allow properties to be set during object initialization but become read-only afterwards. This promotes immutability without requiring constructor parameters.

```csharp
public class Product
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}
```

### Tasks

1. Define a `Product` class with `Name` (string) and `Price` (decimal) properties using `init` setters.
2. Create a `Product` using object initializer syntax.
3. Try to modify a property after initialization — observe the compile error (comment it out after testing).
4. Print the product details.

---

## Exercise 3 – Records

**Records** are reference types designed for **immutable data** with built-in value equality, `ToString()`, and deconstruction.

```csharp
public record Person(string FirstName, string LastName, int Age);
```

### Tasks

1. Define a positional `record` called `Person` with `FirstName`, `LastName`, and `Age`.
2. Create two `Person` instances with identical values.
3. Compare them with `==` — verify they are equal (value equality).
4. Print both to see the auto-generated `ToString()`.
5. Use a **with-expression** to create a new person based on an existing one but with a different age.
6. **Deconstruct** a person into individual variables and print them.

---

## Exercise 4 – Enhanced Pattern Matching

C# 9 added **relational**, **logical**, and **type** patterns to make conditional logic more expressive.

| Pattern | Example |
|---------|---------|
| Relational | `< 0`, `>= 100` |
| Logical | `and`, `or`, `not` |
| Type | `is string s` |

### Tasks

1. Write a method `DescribeNumber(object obj)` that uses pattern matching:
   - If `obj` is not an `int`, return `"Not a number"`.
   - If the int is negative, return `"Negative"`.
   - If zero, return `"Zero"`.
   - If between 1 and 100 (inclusive), return `"Small positive"`.
   - Otherwise, return `"Large positive"`.
2. Write a method `GetDiscount(decimal price)` using a switch expression with relational/logical patterns:
   - Price < 10 → 0%
   - Price >= 10 and < 50 → 5%
   - Price >= 50 and < 100 → 10%
   - Price >= 100 → 15%
3. Test both methods with various inputs.

---

## Wrapping Up

```bash
dotnet run
```

Compare your output with the `solution` folder if needed. Pay special attention to how records provide value equality and immutability out of the box.
