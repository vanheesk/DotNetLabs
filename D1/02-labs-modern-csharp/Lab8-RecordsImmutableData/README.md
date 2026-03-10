# Lab 8: Applying Records for Immutable Data Models

## Objective

Deep-dive into **records** as the foundation for immutable data modelling in C#. You will explore value equality semantics, `with`-expressions for non-destructive mutation, inheritance with records, and how records integrate seamlessly with pattern matching.

---

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

```bash
cd starter
dotnet build
```

---

## Exercise 1 – Value Equality in Records

Records provide **structural (value) equality** by default: two record instances are equal if all their properties have the same values, unlike classes which use reference equality.

```csharp
var a = new Point(1, 2);
var b = new Point(1, 2);
Console.WriteLine(a == b);          // True (value equality)
Console.WriteLine(ReferenceEquals(a, b)); // False (different objects)
```

### Tasks

1. Define a `record Address(string Street, string City, string Country)`.
2. Create two `Address` instances with identical values.
3. Verify `==` returns `true` and `ReferenceEquals` returns `false`.
4. Create a third `Address` with a different city — verify it is **not** equal.
5. Demonstrate that `GetHashCode()` is the same for equal records.

---

## Exercise 2 – With-Expressions for Non-Destructive Mutation

Since records are immutable, you create modified copies using **with-expressions**. The original instance is never changed.

```csharp
var original = new Product("Laptop", 999m);
var updated = original with { Price = 899m };
// original.Price is still 999m
```

### Tasks

1. Define a `record Employee(string Name, string Department, decimal Salary)`.
2. Create an employee.
3. Use `with` to create a version with a different department (promotion).
4. Use `with` to create a version with a salary raise.
5. Chain `with` to change multiple properties at once.
6. Verify the original is untouched after all mutations.

---

## Exercise 3 – Record Inheritance

Records support **inheritance**. Derived records inherit properties and equality behaviour. Equality checks also consider the runtime type.

```csharp
public record Shape(string Color);
public record Circle(string Color, double Radius) : Shape(Color);
```

### Tasks

1. Define a record hierarchy:
   - `record Vehicle(string Make, int Year)`
   - `record Car(string Make, int Year, int Doors) : Vehicle(Make, Year)`
   - `record Truck(string Make, int Year, double PayloadTons) : Vehicle(Make, Year)`
2. Create instances of `Car` and `Truck`.
3. Verify that a `Car` and `Truck` with the same `Make` and `Year` are **not** equal (different types).
4. Use `with` on a derived record to create a copy.

---

## Exercise 4 – Deconstruction and Positional Patterns

Positional records automatically generate a `Deconstruct` method, enabling deconstruction and positional pattern matching.

```csharp
var (name, age) = person;  // Deconstruct

if (person is ("Alice", _, > 18))
    Console.WriteLine("Adult Alice");
```

### Tasks

1. Define a `record Temperature(string Location, double Celsius, DateTime MeasuredAt)`.
2. Deconstruct a `Temperature` into variables and print them.
3. Write a method `ClassifyReading(Temperature reading)` using positional patterns:
   - `(_, < 0, _)` → `"Freezing"`
   - `(_, >= 0 and < 15, _)` → `"Cold"`
   - `(_, >= 15 and < 30, _)` → `"Comfortable"`
   - `(_, >= 30, _)` → `"Hot"`
4. Test with various temperature readings.

---

## Exercise 5 – Records and Pattern Matching (Real-World Scenario)

Combine records with pattern matching to build a **payment processor**.

### Tasks

1. Define a record hierarchy for payment methods:
   - `abstract record Payment(decimal Amount)`
   - `record CreditCard(decimal Amount, string CardNumber, string Expiry) : Payment(Amount)`
   - `record BankTransfer(decimal Amount, string IBAN) : Payment(Amount)`
   - `record DigitalWallet(decimal Amount, string Provider) : Payment(Amount)`
2. Write a method `ProcessPayment(Payment payment)` that returns a string using pattern matching:
   - `CreditCard { Amount: > 10000 }` → `"Credit card over limit — declined"`
   - `CreditCard c` → `"Processing credit card ending in {last4}"`
   - `BankTransfer { Amount: > 50000 }` → `"Large transfer — requires verification"`
   - `BankTransfer b` → `"Processing bank transfer to {IBAN}"`
   - `DigitalWallet { Provider: "PayPal" or "Apple Pay" }` → `"Processing {provider} payment"`
   - `DigitalWallet w` → `"Unknown wallet provider: {w.Provider}"`
3. Test with various payment types and amounts.

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed. Records combined with pattern matching form a powerful foundation for building clean, immutable domain models.
