// ============================================================
// Lab 1: C# 8 Features – SOLUTION
// Topics: Nullable Reference Types, Asynchronous Streams,
//         Ranges and Indices, Switch Expressions, Using Declarations
// ============================================================

Console.WriteLine("=== Lab 1: C# 8 Features ===\n");

// ----- Exercise 1: Nullable Reference Types -----
Console.WriteLine("--- Exercise 1: Nullable Reference Types ---");

string nonNullName = "Alice";
string? nullableName = null;

Console.WriteLine($"Non-null name: {nonNullName}");
Console.WriteLine($"Nullable name length: {nullableName?.Length ?? 0}");

static string Greet(string? name)
{
    if (name is null)
        return "Hello, stranger!";
    return $"Hello, {name}!";
}

Console.WriteLine(Greet("Bob"));
Console.WriteLine(Greet(null));

Console.WriteLine();

// ----- Exercise 2: Ranges and Indices -----
Console.WriteLine("--- Exercise 2: Ranges and Indices ---");

int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// Index from end
int lastElement = numbers[^1];
int secondToLast = numbers[^2];
Console.WriteLine($"Last element: {lastElement}");
Console.WriteLine($"Second to last: {secondToLast}");

// Ranges
int[] firstThree = numbers[..3];
int[] lastThree = numbers[^3..];
int[] middle = numbers[3..7];

Console.WriteLine($"First three: {string.Join(", ", firstThree)}");
Console.WriteLine($"Last three: {string.Join(", ", lastThree)}");
Console.WriteLine($"Middle (index 3-6): {string.Join(", ", middle)}");

Console.WriteLine();

// ----- Exercise 3: Switch Expressions -----
Console.WriteLine("--- Exercise 3: Switch Expressions ---");

static string ClassifyTemperature(double temp) => temp switch
{
    < 0 => "Freezing",
    >= 0 and < 15 => "Cold",
    >= 15 and < 25 => "Comfortable",
    >= 25 and < 35 => "Warm",
    _ => "Hot"
};

double[] temperatures = [-5, 3, 18, 28, 40];
foreach (var t in temperatures)
{
    Console.WriteLine($"  {t}°C is {ClassifyTemperature(t)}");
}

Console.WriteLine();

// ----- Exercise 4: Using Declarations -----
Console.WriteLine("--- Exercise 4: Using Declarations ---");

static void WriteToFile(string path, string content)
{
    // Using declaration – disposed at end of enclosing scope
    using var writer = new StreamWriter(path);
    writer.WriteLine(content);
    Console.WriteLine($"  Wrote content to {Path.GetFileName(path)}");
}

var tempFile = Path.GetTempFileName();
WriteToFile(tempFile, "Hello from C# 8!");
Console.WriteLine($"  File content: {File.ReadAllText(tempFile).Trim()}");
File.Delete(tempFile);

Console.WriteLine();

// ----- Exercise 5: Asynchronous Streams -----
Console.WriteLine("--- Exercise 5: Asynchronous Streams ---");

static async IAsyncEnumerable<int> GenerateNumbersAsync(int count)
{
    for (int i = 1; i <= count; i++)
    {
        await Task.Delay(100); // Simulate async work
        yield return i;
    }
}

Console.Write("  Async stream values: ");
await foreach (var number in GenerateNumbersAsync(5))
{
    Console.Write($"{number} ");
}
Console.WriteLine();

Console.WriteLine("\nLab 1 completed!");
