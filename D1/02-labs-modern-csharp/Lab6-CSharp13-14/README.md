# Lab 6: C# 13 and C# 14 Features

## Objective

Explore the latest C# language features spanning **C# 13** (.NET 9) and **C# 14** (.NET 10): enhanced params collections, the new `Lock` type, the `field` keyword in properties, null-conditional assignment, and first-class `Span<T>` support.

---

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

```bash
cd starter
dotnet build
```

> **Note:** This lab uses `<LangVersion>preview</LangVersion>` in the project file to ensure access to the latest C# 14 features.

---

## Exercise 1 – Enhanced `params` Collections (C# 13)

In earlier C# versions, `params` only worked with arrays. Starting with C# 13, `params` can target **any collection type**: `Span<T>`, `ReadOnlySpan<T>`, `IEnumerable<T>`, `List<T>`, and more.

```csharp
void PrintAll(params ReadOnlySpan<string> items)
{
    foreach (var item in items)
        Console.Write($"{item} ");
}
```

This avoids the hidden array allocation that `params T[]` always incurred.

### Tasks

1. Write a method `PrintAll(params ReadOnlySpan<string> items)` that prints each item.
2. Write a method `T Sum<T>(params ReadOnlySpan<T> values) where T : INumber<T>` that sums the values without allocating an array.
3. Call both methods to verify they work.

---

## Exercise 2 – Refined Lock Semantics (C# 13)

C# 13 introduces `System.Threading.Lock`, a purpose-built lock type that is more efficient and expressive than locking on an arbitrary `object`.

```csharp
private readonly Lock _lock = new();

void SafeIncrement()
{
    lock (_lock)  // Uses Lock.EnterScope() under the hood
    {
        _counter++;
    }
}
```

### Tasks

1. Create a `Counter` class with a `System.Threading.Lock` field.
2. Implement thread-safe `Increment()` and `GetValue()` methods using `lock (_lock)`.
3. Spawn multiple tasks that increment the counter concurrently and verify the final value is correct.

---

## Exercise 3 – The `field` Keyword (C# 14)

The `field` keyword gives you access to the **compiler-generated backing field** of an auto-property, enabling validation or transformation without declaring a manual field.

```csharp
public class Temperature
{
    public double Celsius
    {
        get => field;
        set => field = value < -273.15 ? -273.15 : value;
    }
}
```

### Tasks

1. Create a `Person` class with a `Name` property that uses `field` with a setter that trims whitespace.
2. Create a `Percentage` class with a `Value` property that clamps to 0–100.
3. Test both with edge-case values.

---

## Exercise 4 – Null-Conditional Assignment (C# 14)

You can now assign to a property through a null-conditional chain. If the left side evaluates to `null`, the assignment is skipped.

```csharp
config?.Theme = "Dark";  // Only assigns if config is not null
```

### Tasks

1. Create a `Settings` class with a `Theme` property.
2. Demonstrate null-conditional assignment when the variable is null (no exception).
3. Demonstrate it when the variable is non-null (assignment succeeds).

---

## Exercise 5 – First-Class `Span<T>` Support (C# 14)

C# 14 improves `Span<T>` and `ReadOnlySpan<T>` support, allowing them in more places such as `params`, extension methods on spans, and implicit conversions from arrays to spans in more contexts.

### Tasks

1. Write a method that accepts `ReadOnlySpan<char>` and counts vowels.
2. Call it with a `string` (which implicitly converts to `ReadOnlySpan<char>`).
3. Write a method that searches a `Span<int>` for a value and returns its index.

---

## Bonus: Extension Members (C# 14)

C# 14 introduces **extension members** — a new way to declare extension methods, properties, and more using an `extension` block. Extension members complement traditional static extension methods with a richer syntax.

> **Note:** If your SDK version does not yet support extension member syntax, use traditional extension methods as a fallback:
>
> ```csharp
> public static class StringExtensions
> {
>     public static bool IsPalindrome(this string s)
>         => s.SequenceEqual(s.Reverse());
> }
> ```

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed.
