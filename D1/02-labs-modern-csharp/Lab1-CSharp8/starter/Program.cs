// ============================================================
// Lab 1: C# 8 Features
// Topics: Nullable Reference Types, Asynchronous Streams,
//         Ranges and Indices, Switch Expressions, Using Declarations
// ============================================================

Console.WriteLine("=== Lab 1: C# 8 Features ===\n");

// ----- Exercise 1: Nullable Reference Types -----
Console.WriteLine("--- Exercise 1: Nullable Reference Types ---");

// TODO: Declare a non-nullable string variable called 'nonNullName' with value "Alice"

// TODO: Declare a nullable string variable called 'nullableName' set to null
//       (Hint: use string? as the type)

// TODO: Print the non-null name

// TODO: Print the length of nullableName safely using the null-conditional (?.)
//       and null-coalescing (??) operators so it prints 0 when null
//       Example: Console.WriteLine($"Nullable name length: {nullableName?.Length ?? 0}");

// TODO: Create a static method 'Greet' that takes a nullable string parameter 'name'
//       - If name is null (use 'is null'), return "Hello, stranger!"
//       - Otherwise, return $"Hello, {name}!"

// TODO: Call Greet with "Bob" and with null, printing both results

Console.WriteLine();

// ----- Exercise 2: Ranges and Indices -----
Console.WriteLine("--- Exercise 2: Ranges and Indices ---");

int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// TODO: Use the ^ (index from end) operator to get the last element
//       Example: int lastElement = numbers[^1];

// TODO: Use the ^ operator to get the second-to-last element

// TODO: Print both values

// TODO: Use the range operator (..) to get the first three elements: numbers[..3]

// TODO: Use the range operator to get the last three elements: numbers[^3..]

// TODO: Use the range operator to get elements at indices 3 through 6: numbers[3..7]

// TODO: Print each slice using string.Join(", ", slice)

Console.WriteLine();

// ----- Exercise 3: Switch Expressions -----
Console.WriteLine("--- Exercise 3: Switch Expressions ---");

// TODO: Create a static method 'ClassifyTemperature' that takes a double and returns a string
//       Use a switch EXPRESSION (not a switch statement) with these rules:
//       - Below 0        => "Freezing"
//       - 0 to 14.99     => "Cold"
//       - 15 to 24.99    => "Comfortable"
//       - 25 to 34.99    => "Warm"
//       - 35 and above   => "Hot"
//
//       Example syntax:
//       static string ClassifyTemperature(double temp) => temp switch
//       {
//           < 0 => "Freezing",
//           ...
//           _ => "Hot"
//       };

double[] temperatures = [-5, 3, 18, 28, 40];

// TODO: Loop through temperatures and print each with its classification
//       Example output: "-5°C is Freezing"

Console.WriteLine();

// ----- Exercise 4: Using Declarations -----
Console.WriteLine("--- Exercise 4: Using Declarations ---");

// TODO: Create a static method 'WriteToFile' that takes a string path and string content
//       Inside, use a 'using declaration' (using var writer = new StreamWriter(path);)
//       Write the content to the file using writer.WriteLine(content)
//       The writer is automatically disposed at the end of the method scope

// TODO: Create a temp file with Path.GetTempFileName()
// TODO: Call WriteToFile to write "Hello from C# 8!" to the temp file
// TODO: Read back and print the file content: File.ReadAllText(tempFile).Trim()
// TODO: Delete the temp file: File.Delete(tempFile)

Console.WriteLine();

// ----- Exercise 5: Asynchronous Streams -----
Console.WriteLine("--- Exercise 5: Asynchronous Streams ---");

// TODO: Create a static async method 'GenerateNumbersAsync(int count)'
//       that returns IAsyncEnumerable<int>
//       Inside, loop from 1 to count:
//         - await Task.Delay(100) to simulate async work
//         - yield return the current number

// TODO: Use 'await foreach' to consume the async stream and print each value
//       Example:
//       await foreach (var number in GenerateNumbersAsync(5))
//       {
//           Console.Write($"{number} ");
//       }

Console.WriteLine("\nLab 1 completed!");
