using System.Runtime.CompilerServices;

// Type aliases (Exercise 5)
using PersonInfo = (string Name, int Age);
using ScoreBoard = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>;

// ============================================================
// Lab 5: C# 12 Features – SOLUTION
// Topics: Primary Constructors, Collection Expressions,
//         Default Lambda Parameters, Inline Arrays, Type Aliases
// ============================================================

Console.WriteLine("=== Lab 5: C# 12 Features ===\n");

// ----- Exercise 1: Primary Constructors -----
Console.WriteLine("--- Exercise 1: Primary Constructors ---");

var account = new BankAccount("Alice", 1000m);
Console.WriteLine($"  {account.Owner}'s balance: {account.Balance:C}");
account.Deposit(500m);
Console.WriteLine($"  After deposit of 500: {account.Balance:C}");
account.Withdraw(200m);
Console.WriteLine($"  After withdrawal of 200: {account.Balance:C}");

Console.WriteLine();

// ----- Exercise 2: Collection Expressions -----
Console.WriteLine("--- Exercise 2: Collection Expressions ---");

int[] numbers = [1, 2, 3, 4, 5];
List<string> fruits = ["Apple", "Banana", "Cherry"];
Span<double> values = [1.1, 2.2, 3.3];

// Spread operator
int[] moreNumbers = [6, 7, 8];
int[] allNumbers = [.. numbers, .. moreNumbers, 9, 10];

Console.WriteLine($"  All numbers: {string.Join(", ", allNumbers)}");
Console.WriteLine($"  Fruits: {string.Join(", ", fruits)}");
Console.Write("  Span values: ");
foreach (var v in values)
    Console.Write($"{v} ");
Console.WriteLine();

Console.WriteLine();

// ----- Exercise 3: Default Lambda Parameters -----
Console.WriteLine("--- Exercise 3: Default Lambda Parameters ---");

var greet = (string name, string greeting = "Hello") => $"{greeting}, {name}!";
var multiply = (int value, int multiplier = 2) => value * multiplier;

Console.WriteLine($"  {greet("Alice")}");
Console.WriteLine($"  {greet("Bob", "Hi")}");
Console.WriteLine($"  multiply(5): {multiply(5)}");
Console.WriteLine($"  multiply(5, 10): {multiply(5, 10)}");

Console.WriteLine();

// ----- Exercise 4: Inline Arrays -----
Console.WriteLine("--- Exercise 4: Inline Arrays ---");

var buffer = new Buffer8();
for (int i = 0; i < 8; i++)
    buffer[i] = i * 10;

Console.Write("  Buffer values: ");
foreach (var val in buffer)
    Console.Write($"{val} ");
Console.WriteLine();

Console.WriteLine();

// ----- Exercise 5: Type Aliases -----
Console.WriteLine("--- Exercise 5: Type Aliases ---");

PersonInfo person = ("Alice", 30);
Console.WriteLine($"  Person: {person.Name}, Age: {person.Age}");

ScoreBoard scores = new()
{
    ["Alice"] = [95, 87, 92],
    ["Bob"] = [78, 85, 90]
};
foreach (var (student, grades) in scores)
{
    Console.WriteLine($"  {student}: {string.Join(", ", grades)}");
}

Console.WriteLine("\nLab 5 completed!");

// ----- Type definitions -----

public class BankAccount(string owner, decimal initialBalance)
{
    private decimal _balance = initialBalance;

    public string Owner => owner;
    public decimal Balance => _balance;

    public void Deposit(decimal amount) => _balance += amount;

    public void Withdraw(decimal amount)
    {
        if (amount > _balance)
            throw new InvalidOperationException("Insufficient funds");
        _balance -= amount;
    }
}

[InlineArray(8)]
public struct Buffer8
{
    private int _element;
}
