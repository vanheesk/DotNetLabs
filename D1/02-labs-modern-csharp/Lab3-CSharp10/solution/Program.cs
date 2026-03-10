using System.Runtime.CompilerServices;
using Lab3.Models;

// ============================================================
// Lab 3: C# 10 Features – SOLUTION
// Topics: File-Scoped Namespaces, Global Usings, Record Structs,
//         Lambda Improvements, CallerArgumentExpression
// ============================================================

Console.WriteLine("=== Lab 3: C# 10 Features ===\n");

// ----- Exercise 1: File-Scoped Namespaces -----
Console.WriteLine("--- Exercise 1: File-Scoped Namespaces ---");
Console.WriteLine("  See Models.cs — uses file-scoped namespace.");
var customer = new Customer { Name = "Alice", Email = "alice@example.com" };
Console.WriteLine($"  Customer: {customer}");

Console.WriteLine();

// ----- Exercise 2: Global Usings -----
Console.WriteLine("--- Exercise 2: Global Usings ---");
// JsonSerializer is available without a local using — see GlobalUsings.cs
var data = new { Message = "Hello", Year = 2026 };
string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine($"  Serialized JSON:\n{json}");

Console.WriteLine();

// ----- Exercise 3: Record Structs -----
Console.WriteLine("--- Exercise 3: Record Structs ---");

var coord1 = new Coordinate(52.3676, 4.9041);  // Amsterdam
var coord2 = new Coordinate(52.3676, 4.9041);
Console.WriteLine($"  coord1 == coord2: {coord1 == coord2}");  // True
Console.WriteLine($"  coord1: {coord1}");

// Coordinate is mutable (record struct)
coord1.Latitude = 48.8566;
Console.WriteLine($"  Modified coord1: {coord1}");

// Money is immutable (readonly record struct)
var price = new Money(29.99m, "EUR");
var discounted = price with { Amount = 24.99m };
Console.WriteLine($"  Original price: {price}");
Console.WriteLine($"  Discounted: {discounted}");
// price.Amount = 0;  // Error: readonly record struct properties cannot be assigned

Console.WriteLine();

// ----- Exercise 4: Lambda Improvements -----
Console.WriteLine("--- Exercise 4: Lambda Improvements ---");

// Natural type inference — var works for lambdas now
var toUpper = (string s) => s.ToUpper();

// Explicit return type on a lambda
var parseNumber = int (string s) => int.Parse(s);

Console.WriteLine($"  toUpper(\"hello\"): {toUpper("hello")}");
Console.WriteLine($"  parseNumber(\"42\"): {parseNumber("42")}");

Console.WriteLine();

// ----- Exercise 5: CallerArgumentExpression -----
Console.WriteLine("--- Exercise 5: CallerArgumentExpression ---");

string? myVariable = null;
try
{
    Guard.EnsureNotNull(myVariable);
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"  Caught: '{ex.ParamName}' was null");
}

Console.WriteLine("\nLab 3 completed!");

// ----- Type definitions -----

public record struct Coordinate(double Latitude, double Longitude);

public readonly record struct Money(decimal Amount, string Currency);

public static class Guard
{
    public static void EnsureNotNull(
        object? value,
        [CallerArgumentExpression(nameof(value))] string? expression = null)
    {
        if (value is null)
            throw new ArgumentNullException(expression);
    }
}
