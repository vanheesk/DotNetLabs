# Lab 5: C# 12 Features

## Objective

Explore features from **C# 12**: primary constructors, collection expressions, default lambda parameters, inline arrays, and type aliases. These features improve **conciseness**, support **functional patterns**, and enable **performance-focused** programming.

---

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

```bash
cd starter
dotnet build
```

---

## Exercise 1 – Primary Constructors

**Primary constructors** let you declare constructor parameters directly on the class or struct declaration. The parameters are available throughout the class body.

```csharp
public class Logger(string prefix)
{
    public void Log(string message) => Console.WriteLine($"[{prefix}] {message}");
}
```

### Tasks

1. Create a `BankAccount` class with a primary constructor accepting `string owner` and `decimal initialBalance`.
2. Store the balance in a private field and expose a read-only `Balance` property.
3. Add `Deposit(decimal amount)` and `Withdraw(decimal amount)` methods.
4. Create an instance and perform some transactions, printing the balance after each.

---

## Exercise 2 – Collection Expressions

**Collection expressions** use `[...]` syntax for creating arrays, lists, spans, and more. The **spread operator** `..` merges collections.

```csharp
int[] numbers = [1, 2, 3];
List<string> names = ["Alice", "Bob"];
int[] combined = [..numbers, 4, 5];
```

### Tasks

1. Create an array, a list, and a span using collection expression syntax `[...]`.
2. Use the spread operator `..` to merge two collections into a new one.
3. Print the results.

---

## Exercise 3 – Default Lambda Parameters

C# 12 lets you specify **default parameter values** in lambdas, making them more flexible for functional pipelines.

```csharp
var greet = (string name, string greeting = "Hello") => $"{greeting}, {name}!";
```

### Tasks

1. Create a lambda with a default parameter for a greeting message.
2. Create a lambda with a default parameter for a multiplier.
3. Call each lambda with and without the optional argument.

---

## Exercise 4 – Inline Arrays

**Inline arrays** are fixed-size arrays stored directly in a struct — no heap allocation. They are useful in performance-critical code.

```csharp
[System.Runtime.CompilerServices.InlineArray(4)]
public struct FourInts
{
    private int _element;
}
```

### Tasks

1. Define an inline array struct `Buffer8` with 8 `int` elements.
2. Create an instance, fill it with values, and iterate over it.

---

## Exercise 5 – Type Aliases (`using` Alias)

C# 12 extends `using` aliases to support **any type**, including tuples, arrays, and generics.

```csharp
using Point = (double X, double Y);
using NumberList = System.Collections.Generic.List<int>;
```

### Tasks

1. Create a type alias for a tuple `(string Name, int Age)` called `PersonInfo`.
2. Create a type alias for `Dictionary<string, List<int>>` called `ScoreBoard`.
3. Use both aliases to create and manipulate instances.

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed.
