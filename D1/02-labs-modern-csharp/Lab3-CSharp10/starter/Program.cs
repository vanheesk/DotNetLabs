using System.Runtime.CompilerServices;
using Lab3.Models;

// ============================================================
// Lab 3: C# 10 Features
// Topics: File-Scoped Namespaces, Global Usings, Record Structs,
//         Lambda Improvements, CallerArgumentExpression
// ============================================================

Console.WriteLine("=== Lab 3: C# 10 Features ===\n");

// ----- Exercise 1: File-Scoped Namespaces -----
Console.WriteLine("--- Exercise 1: File-Scoped Namespaces ---");
Console.WriteLine("  Open Models.cs and convert the namespace to a file-scoped namespace.");

// TODO: Once you've fixed Models.cs, the following should work:
// var customer = new Customer { Name = "Alice", Email = "alice@example.com" };
// Console.WriteLine($"  Customer: {customer}");

Console.WriteLine();

// ----- Exercise 2: Global Usings -----
Console.WriteLine("--- Exercise 2: Global Usings ---");

// TODO: After adding 'global using System.Text.Json;' in GlobalUsings.cs,
//       the following should work WITHOUT a local using directive:
// var data = new { Message = "Hello", Year = 2026 };
// string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
// Console.WriteLine($"  Serialized JSON:\n{json}");

Console.WriteLine();

// ----- Exercise 3: Record Structs -----
Console.WriteLine("--- Exercise 3: Record Structs ---");

// TODO: Define a record struct: record struct Coordinate(double Latitude, double Longitude);
// TODO: Define a readonly record struct: readonly record struct Money(decimal Amount, string Currency);

// TODO: Create two Coordinate instances with the same values and test equality:
//       var coord1 = new Coordinate(52.3676, 4.9041);
//       var coord2 = new Coordinate(52.3676, 4.9041);
//       Console.WriteLine($"  coord1 == coord2: {coord1 == coord2}");

// TODO: Show that Coordinate is mutable:
//       coord1.Latitude = 48.8566;
//       Console.WriteLine($"  Modified coord1: {coord1}");

// TODO: Create a Money instance and use a with-expression:
//       var price = new Money(29.99m, "EUR");
//       var discounted = price with { Amount = 24.99m };
//       Console.WriteLine($"  Original: {price}");
//       Console.WriteLine($"  Discounted: {discounted}");

// TODO: Show that Money is immutable (uncomment to see error, then comment back):
// price.Amount = 0;  // Error: readonly

Console.WriteLine();

// ----- Exercise 4: Lambda Improvements -----
Console.WriteLine("--- Exercise 4: Lambda Improvements ---");

// TODO: Assign a lambda with natural type inference using var:
//       var toUpper = (string s) => s.ToUpper();

// TODO: Assign a lambda with an explicit return type:
//       var parseNumber = int (string s) => int.Parse(s);

// TODO: Call both and print results:
//       Console.WriteLine($"  toUpper(\"hello\"): {toUpper("hello")}");
//       Console.WriteLine($"  parseNumber(\"42\"): {parseNumber("42")}");

Console.WriteLine();

// ----- Exercise 5: CallerArgumentExpression -----
Console.WriteLine("--- Exercise 5: CallerArgumentExpression ---");

// TODO: Create a static class 'Guard' with a method:
//       public static void EnsureNotNull(
//           object? value,
//           [CallerArgumentExpression(nameof(value))] string? expression = null)
//       {
//           if (value is null)
//               throw new ArgumentNullException(expression);
//       }

// TODO: Test it:
//       string? myVariable = null;
//       try
//       {
//           Guard.EnsureNotNull(myVariable);
//       }
//       catch (ArgumentNullException ex)
//       {
//           Console.WriteLine($"  Caught: {ex.ParamName} was null (expression: '{ex.ParamName}')");
//       }

Console.WriteLine("\nLab 3 completed!");

// ----- Type definitions below -----
