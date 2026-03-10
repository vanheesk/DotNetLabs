# Lab 7: Advanced Pattern Matching

## Objective

Master the full range of **pattern matching** features across modern C#. This lab brings together property patterns, list patterns, relational and logical patterns, and positional patterns to write highly **expressive conditional logic**.

---

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

```bash
cd starter
dotnet build
```

---

## Exercise 1 – Type and Declaration Patterns

The simplest patterns check and cast types at the same time.

```csharp
object obj = "Hello";
if (obj is string s)
    Console.WriteLine(s.Length);
```

### Tasks

1. Write a method `DescribeObject(object obj)` that returns a description based on the type:
   - `null` → `"null"`
   - `string s` → `"String of length {s.Length}"`
   - `int n` → `"Integer: {n}"`
   - `double d` → `"Double: {d:F2}"`
   - `IEnumerable<object> list` → `"Collection with {list.Count()} items"`
   - Anything else → `"Unknown type: {obj.GetType().Name}"`
2. Test with null, a string, int, double, a list, and a DateTime.

---

## Exercise 2 – Property Patterns

**Property patterns** let you match on an object's property values directly.

```csharp
if (address is { City: "Amsterdam", Country: "NL" })
    Console.WriteLine("You're in Amsterdam!");
```

### Tasks

1. Define a `record Order(string Customer, decimal Amount, string Status, string Country)`.
2. Write a method `EvaluateOrder(Order order)` using a switch expression with property patterns:
   - `{ Status: "Cancelled" }` → `"Order cancelled"`
   - `{ Amount: > 1000, Country: "US" }` → `"Large US order — needs review"`
   - `{ Amount: > 1000 }` → `"Large international order"`
   - `{ Status: "Pending", Amount: < 50 }` → `"Small pending order — auto-approve"`
   - Default → `"Standard order"`
3. Test with various orders.

---

## Exercise 3 – Relational and Logical Patterns

**Relational patterns** (`<`, `>`, `<=`, `>=`) and **logical patterns** (`and`, `or`, `not`) combine for range checks and complex conditions.

```csharp
string Grade(int score) => score switch
{
    >= 90 and <= 100 => "A",
    >= 80 and < 90   => "B",
    < 0 or > 100     => "Invalid",
    _                => "C or below"
};
```

### Tasks

1. Write a `ClassifyBMI(double bmi)` method using a switch expression:
   - Below 18.5 → `"Underweight"`
   - 18.5 to 24.9 → `"Normal"`
   - 25.0 to 29.9 → `"Overweight"`
   - 30 and above → `"Obese"`
2. Write a `ValidateAge(int age)` method that returns a string:
   - Negative → `"Invalid"`
   - 0 to 12 → `"Child"`
   - 13 to 17 → `"Teenager"`
   - 18 to 64 → `"Adult"`
   - 65+ → `"Senior"`
3. Use `not` pattern: write a method that checks if a value is **not** null **and not** an empty string.

---

## Exercise 4 – List Patterns

**List patterns** match elements of arrays and lists by position and content.

| Pattern | Meaning |
|---------|---------|
| `[1, 2, 3]` | Exact match |
| `[1, ..]` | Starts with 1, any length |
| `[.., 0]` | Ends with 0 |
| `[_, var middle, _]` | Three elements, capture middle |
| `[>0, >0, >0]` | Three positive elements |

### Tasks

1. Write a method `AnalyzeSequence(int[] seq)` that returns a string:
   - `[]` → `"Empty sequence"`
   - `[var only]` → `"Single value: {only}"`
   - `[var first, .., var last]` when first == last → `"Palindromic endpoints: {first}"`
   - `[> 0, > 0, > 0, ..]` → `"Starts with three positive numbers"`
   - `[.., 0]` → `"Ends with zero"`
   - Default → `"Unclassified sequence of length {seq.Length}"`
2. Test with various arrays.

---

## Exercise 5 – Combining Patterns (Real-World Scenario)

Combine all pattern types in a realistic scenario: an **HTTP response analyzer**.

### Tasks

1. Define a `record HttpResponse(int StatusCode, string? Body, Dictionary<string, string> Headers)`.
2. Write a method `AnalyzeResponse(HttpResponse response)` using nested patterns:
   - Status 200 with non-null body → `"Success with data"`
   - Status 200 with null body → `"Success but empty"`
   - Status 301 or 302 → `"Redirect"`
   - Status >= 400 and < 500 → `"Client error ({code})"`
   - Status >= 500 → `"Server error ({code})"`
   - Default → `"Unexpected status: {code}"`
3. Test with multiple response scenarios.

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed. Pattern matching is most powerful when you combine multiple pattern types together.
