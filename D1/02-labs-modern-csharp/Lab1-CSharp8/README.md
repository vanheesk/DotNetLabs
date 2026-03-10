# Lab 1: C# 8 Features

## Objective

In this lab you will explore key features introduced in **C# 8**. By the end you will be comfortable with nullable reference types, asynchronous streams, ranges & indices, switch expressions, and using declarations.

---

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

Open a terminal in the `starter` folder and verify the project builds:

```bash
dotnet build
```

---

## Exercise 1 – Nullable Reference Types

C# 8 introduced **nullable reference types** to help you avoid `NullReferenceException` at compile time. When the feature is enabled (it is by default in modern .NET projects), the compiler warns whenever you dereference a variable that could be `null`.

### Key concepts

| Syntax | Meaning |
|--------|---------|
| `string` | Non-nullable reference – should never be null |
| `string?` | Nullable reference – may be null |
| `?.` | Null-conditional operator |
| `??` | Null-coalescing operator |

### Tasks

1. Declare a **non-nullable** `string` variable `nonNullName` set to `"Alice"`.
2. Declare a **nullable** `string?` variable `nullableName` set to `null`.
3. Print the non-null name directly.
4. Print the length of `nullableName` using `?.Length ?? 0` so it safely returns `0` when null.
5. Write a static method `Greet(string? name)` that returns `"Hello, stranger!"` when `name` is null, or `"Hello, {name}!"` otherwise. Use `is null` pattern matching.
6. Call `Greet` with `"Bob"` and with `null`, printing both results.

---

## Exercise 2 – Ranges and Indices

The **Index** (`^`) and **Range** (`..`) operators give you concise, readable syntax for slicing arrays and spans.

### Key concepts

| Expression | Meaning |
|------------|---------|
| `array[^1]` | Last element |
| `array[^2]` | Second-to-last element |
| `array[..3]` | First three elements (indices 0, 1, 2) |
| `array[^3..]` | Last three elements |
| `array[2..5]` | Elements at indices 2, 3, 4 |

### Tasks

1. Given the array `[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]`, use the `^` operator to retrieve the **last** and **second-to-last** elements.
2. Use the range operator `..` to slice:
   - The first three elements
   - The last three elements
   - Elements at indices 3 through 6
3. Print each slice with `string.Join(", ", slice)`.

---

## Exercise 3 – Switch Expressions

**Switch expressions** turn a multi-case `switch` statement into a concise expression that returns a value.

```csharp
var result = input switch
{
    pattern1 => value1,
    pattern2 => value2,
    _        => defaultValue
};
```

### Tasks

1. Create a static method `ClassifyTemperature(double temp)` that returns a `string` using a **switch expression**:
   - Below 0 → `"Freezing"`
   - 0 to 14.99 → `"Cold"`
   - 15 to 24.99 → `"Comfortable"`
   - 25 to 34.99 → `"Warm"`
   - 35 and above → `"Hot"`
2. Loop over the array `[-5, 3, 18, 28, 40]` and print each temperature with its classification.

> **Hint:** You can use relational patterns like `< 0`, `>= 0 and < 15`, etc.

---

## Exercise 4 – Using Declarations

A **using declaration** (`using var x = ...;`) removes the need for a `using` block. The resource is disposed automatically at the end of the enclosing scope (usually the method).

### Tasks

1. Create a static method `WriteToFile(string path, string content)`.
2. Inside it, create a `StreamWriter` with a **using declaration** (not a using block).
3. Write the content to the file.
4. Back in the main code, create a temp file (`Path.GetTempFileName()`), write `"Hello from C# 8!"`, read it back, print the content, then delete the file.

---

## Exercise 5 – Asynchronous Streams

**Async streams** (`IAsyncEnumerable<T>`) let you produce and consume sequences of data asynchronously using `yield return` inside an `async` method and `await foreach` to iterate.

### Tasks

1. Create a static async method `GenerateNumbersAsync(int count)` returning `IAsyncEnumerable<int>`.
2. Inside the method, loop from 1 to `count`. On each iteration, `await Task.Delay(100)` to simulate async work, then `yield return` the number.
3. In the main code, use `await foreach` to consume the stream and print each value.

---

## Wrapping Up

Run the completed project:

```bash
dotnet run
```

You should see output demonstrating all five features. Compare your code with the `solution` folder if you get stuck.
