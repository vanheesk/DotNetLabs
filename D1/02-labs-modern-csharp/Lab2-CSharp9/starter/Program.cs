// ============================================================
// Lab 2: C# 9 Features
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

// TODO: Define a class 'Product' with:
//       - public string Name { get; init; }
//       - public decimal Price { get; init; }

// TODO: Create a Product using an object initializer:
//       var laptop = new Product { Name = "Laptop", Price = 999.99m };

// TODO: Uncomment the line below to observe the compile error, then comment it back:
// laptop.Price = 1099.99m;  // Error: init-only property

// TODO: Print the product: Console.WriteLine($"  Product: {laptop.Name}, Price: {laptop.Price:C}");

Console.WriteLine();

// ----- Exercise 3: Records -----
Console.WriteLine("--- Exercise 3: Records ---");

// TODO: Define a positional record:
//       public record Person(string FirstName, string LastName, int Age);

// TODO: Create two Person instances with identical values
//       var person1 = new Person("Alice", "Smith", 30);
//       var person2 = new Person("Alice", "Smith", 30);

// TODO: Compare them with == and print the result (should be True)
//       Console.WriteLine($"  person1 == person2: {person1 == person2}");

// TODO: Print both persons (auto-generated ToString)

// TODO: Use a with-expression to create a new person with a different age:
//       var person3 = person1 with { Age = 31 };

// TODO: Print person3

// TODO: Deconstruct person1 into variables:
//       var (firstName, lastName, age) = person1;
//       Console.WriteLine($"  Deconstructed: {firstName} {lastName}, age {age}");

Console.WriteLine();

// ----- Exercise 4: Enhanced Pattern Matching -----
Console.WriteLine("--- Exercise 4: Enhanced Pattern Matching ---");

// TODO: Create a static method DescribeNumber(object obj) that returns a string:
//       Use pattern matching with is, and, or:
//       - obj is not int            => "Not a number"
//       - int and < 0               => "Negative"
//       - int and 0                 => "Zero"
//       - int and > 0 and <= 100    => "Small positive"
//       - otherwise                 => "Large positive"
//
//  Hint: static string DescribeNumber(object obj) => obj switch
//  {
//      not int => "Not a number",
//      int n and ...
//  };

// TODO: Test DescribeNumber with: "hello", -5, 0, 42, 200
//       Print each result

// TODO: Create a static method GetDiscount(decimal price) returning a decimal:
//       Use a switch expression:
//       - < 10       => 0m
//       - >= 10 and < 50  => 0.05m
//       - >= 50 and < 100 => 0.10m
//       - _          => 0.15m

// TODO: Test GetDiscount with prices: 5, 25, 75, 150
//       Print each with its discount percentage

Console.WriteLine("\nLab 2 completed!");

// TODO: Define your types below (outside top-level statements, at the end of the file)
// Classes and records should be defined here:
