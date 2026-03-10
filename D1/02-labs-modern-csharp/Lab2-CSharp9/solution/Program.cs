// ============================================================
// Lab 2: C# 9 Features – SOLUTION
// Topics: Records, Enhanced Pattern Matching,
//         Top-Level Statements, Init-Only Setters
// ============================================================

Console.WriteLine("=== Lab 2: C# 9 Features ===\n");

// ----- Exercise 1: Top-Level Statements -----
Console.WriteLine("--- Exercise 1: Top-Level Statements ---");
Console.WriteLine("  This file uses top-level statements — no Main method needed!");
Console.WriteLine();

// ----- Exercise 2: Init-Only Setters -----
Console.WriteLine("--- Exercise 2: Init-Only Setters ---");

var laptop = new Product { Name = "Laptop", Price = 999.99m };
// laptop.Price = 1099.99m;  // Error: init-only property cannot be assigned after initialization
Console.WriteLine($"  Product: {laptop.Name}, Price: {laptop.Price:C}");

Console.WriteLine();

// ----- Exercise 3: Records -----
Console.WriteLine("--- Exercise 3: Records ---");

var person1 = new Person("Alice", "Smith", 30);
var person2 = new Person("Alice", "Smith", 30);

// Value equality
Console.WriteLine($"  person1 == person2: {person1 == person2}");  // True

// Auto-generated ToString
Console.WriteLine($"  person1: {person1}");
Console.WriteLine($"  person2: {person2}");

// With-expression: create a copy with one property changed
var person3 = person1 with { Age = 31 };
Console.WriteLine($"  person3 (aged up): {person3}");

// Deconstruction
var (firstName, lastName, age) = person1;
Console.WriteLine($"  Deconstructed: {firstName} {lastName}, age {age}");

Console.WriteLine();

// ----- Exercise 4: Enhanced Pattern Matching -----
Console.WriteLine("--- Exercise 4: Enhanced Pattern Matching ---");

static string DescribeNumber(object obj) => obj switch
{
    not int => "Not a number",
    int n when n < 0 => "Negative",
    0 => "Zero",
    int n when n > 0 && n <= 100 => "Small positive",
    _ => "Large positive"
};

object[] testValues = ["hello", -5, 0, 42, 200];
foreach (var val in testValues)
{
    Console.WriteLine($"  DescribeNumber({val}) => {DescribeNumber(val)}");
}

Console.WriteLine();

static decimal GetDiscount(decimal price) => price switch
{
    < 10m => 0m,
    >= 10m and < 50m => 0.05m,
    >= 50m and < 100m => 0.10m,
    _ => 0.15m
};

decimal[] prices = [5m, 25m, 75m, 150m];
foreach (var p in prices)
{
    Console.WriteLine($"  Price: {p:C} => Discount: {GetDiscount(p):P0}");
}

Console.WriteLine("\nLab 2 completed!");

// ----- Type definitions -----

public class Product
{
    public required string Name { get; init; }
    public decimal Price { get; init; }
}

public record Person(string FirstName, string LastName, int Age);
