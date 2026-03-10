using System.Numerics;

// ============================================================
// Lab 4: C# 11 Features – SOLUTION
// Topics: Raw String Literals, List/Slice Patterns,
//         Required Members, Generic Math
// ============================================================

Console.WriteLine("=== Lab 4: C# 11 Features ===\n");

// ----- Exercise 1: Raw String Literals -----
Console.WriteLine("--- Exercise 1: Raw String Literals ---");

var jsonString = """
    {
        "name": "Alice",
        "age": 30,
        "city": "Amsterdam"
    }
    """;
Console.WriteLine($"  JSON:\n{jsonString}");

var name = "Bob";
var greeting = $"""
    Hello, {name}!
    Welcome to the C# 11 lab.
    """;
Console.WriteLine(greeting);

Console.WriteLine();

// ----- Exercise 2: List and Slice Patterns -----
Console.WriteLine("--- Exercise 2: List and Slice Patterns ---");

static string DescribeArray(int[] arr) => arr switch
{
    [] => "Empty",
    [var x] => $"Single: {x}",
    [1, 2, ..] => "Starts with 1, 2",
    [var a, var b, var c] => $"Triple: {a}, {b}, {c}",
    _ => "Other"
};

int[][] testArrays = [[], [42], [1, 2, 3, 4], [7, 8, 9], [5, 6, 7, 8, 9]];
foreach (var arr in testArrays)
{
    Console.WriteLine($"  [{string.Join(", ", arr)}] => {DescribeArray(arr)}");
}

Console.WriteLine();

// ----- Exercise 3: Required Members -----
Console.WriteLine("--- Exercise 3: Required Members ---");

var user = new User { Username = "alice", Email = "alice@example.com" };
// var badUser = new User { Username = "bob" };  // Error: required member 'Email' must be set
Console.WriteLine($"  User: {user.Username} ({user.Email})");

Console.WriteLine();

// ----- Exercise 4: Generic Math -----
Console.WriteLine("--- Exercise 4: Generic Math ---");

static T Sum<T>(params T[] values) where T : INumber<T>
{
    T result = T.Zero;
    foreach (var value in values)
        result += value;
    return result;
}

static T Average<T>(params T[] values) where T : INumber<T>
{
    T sum = Sum(values);
    return sum / T.CreateChecked(values.Length);
}

Console.WriteLine($"  Sum (int): {Sum(1, 2, 3, 4, 5)}");
Console.WriteLine($"  Sum (double): {Sum(1.5, 2.5, 3.5)}");
Console.WriteLine($"  Sum (decimal): {Sum(10.0m, 20.0m, 30.0m)}");
Console.WriteLine($"  Average (int): {Average(10, 20, 30)}");
Console.WriteLine($"  Average (double): {Average(1.0, 2.0, 3.0, 4.0)}");

Console.WriteLine("\nLab 4 completed!");

// ----- Type definitions -----

public class User
{
    public required string Username { get; init; }
    public required string Email { get; init; }
}
