using System.Runtime.CompilerServices;

// Type aliases (Exercise 5)
// TODO: Add type aliases here:
// using PersonInfo = (string Name, int Age);
// using ScoreBoard = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>;

// ============================================================
// Lab 5: C# 12 Features
// Topics: Primary Constructors, Collection Expressions,
//         Default Lambda Parameters, Inline Arrays, Type Aliases
// ============================================================

Console.WriteLine("=== Lab 5: C# 12 Features ===\n");

// ----- Exercise 1: Primary Constructors -----
Console.WriteLine("--- Exercise 1: Primary Constructors ---");

// TODO: Define a class 'BankAccount' with a primary constructor:
//       public class BankAccount(string owner, decimal initialBalance)
//       {
//           private decimal _balance = initialBalance;
//           public string Owner => owner;
//           public decimal Balance => _balance;
//           public void Deposit(decimal amount) => _balance += amount;
//           public void Withdraw(decimal amount) => _balance -= amount;
//       }

// TODO: Create a BankAccount and perform transactions:
//       var account = new BankAccount("Alice", 1000m);
//       Console.WriteLine($"  {account.Owner}'s balance: {account.Balance:C}");
//       account.Deposit(500m);
//       Console.WriteLine($"  After deposit: {account.Balance:C}");
//       account.Withdraw(200m);
//       Console.WriteLine($"  After withdrawal: {account.Balance:C}");

Console.WriteLine();

// ----- Exercise 2: Collection Expressions -----
Console.WriteLine("--- Exercise 2: Collection Expressions ---");

// TODO: Create collections using [...] syntax:
//       int[] numbers = [1, 2, 3, 4, 5];
//       List<string> fruits = ["Apple", "Banana", "Cherry"];
//       Span<double> values = [1.1, 2.2, 3.3];

// TODO: Use the spread operator (..) to combine collections:
//       int[] moreNumbers = [6, 7, 8];
//       int[] allNumbers = [..numbers, ..moreNumbers, 9, 10];
//       Console.WriteLine($"  All numbers: {string.Join(", ", allNumbers)}");

// TODO: Print the fruits list:
//       Console.WriteLine($"  Fruits: {string.Join(", ", fruits)}");

Console.WriteLine();

// ----- Exercise 3: Default Lambda Parameters -----
Console.WriteLine("--- Exercise 3: Default Lambda Parameters ---");

// TODO: Create a lambda with a default greeting:
//       var greet = (string name, string greeting = "Hello")
//           => $"{greeting}, {name}!";

// TODO: Create a lambda with a default multiplier:
//       var multiply = (int value, int multiplier = 2) => value * multiplier;

// TODO: Call each with and without the optional parameter:
//       Console.WriteLine($"  {greet("Alice")}");
//       Console.WriteLine($"  {greet("Bob", "Hi")}");
//       Console.WriteLine($"  multiply(5): {multiply(5)}");
//       Console.WriteLine($"  multiply(5, 10): {multiply(5, 10)}");

Console.WriteLine();

// ----- Exercise 4: Inline Arrays -----
Console.WriteLine("--- Exercise 4: Inline Arrays ---");

// TODO: Define an inline array struct (see below the top-level statements):
//       [InlineArray(8)]
//       public struct Buffer8
//       {
//           private int _element;
//       }

// TODO: Create a Buffer8, fill it, and iterate:
//       var buffer = new Buffer8();
//       for (int i = 0; i < 8; i++)
//           buffer[i] = i * 10;
//       Console.Write("  Buffer values: ");
//       foreach (var val in buffer)
//           Console.Write($"{val} ");
//       Console.WriteLine();

Console.WriteLine();

// ----- Exercise 5: Type Aliases -----
Console.WriteLine("--- Exercise 5: Type Aliases ---");

// TODO: After adding the 'using' aliases at the top of this file:
//       PersonInfo person = ("Alice", 30);
//       Console.WriteLine($"  Person: {person.Name}, Age: {person.Age}");

//       ScoreBoard scores = new()
//       {
//           ["Alice"] = [95, 87, 92],
//           ["Bob"] = [78, 85, 90]
//       };
//       foreach (var (student, grades) in scores)
//       {
//           Console.WriteLine($"  {student}: {string.Join(", ", grades)}");
//       }

Console.WriteLine("\nLab 5 completed!");

// ----- Type definitions below -----
