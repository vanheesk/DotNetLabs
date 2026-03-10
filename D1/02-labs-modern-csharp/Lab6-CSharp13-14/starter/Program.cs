using System.Numerics;

// ============================================================
// Lab 6: C# 13 and C# 14 Features
// Topics: Enhanced params, Lock, field keyword,
//         Null-conditional assignment, Span support
// ============================================================

Console.WriteLine("=== Lab 6: C# 13 & C# 14 Features ===\n");

// ----- Exercise 1: Enhanced params Collections -----
Console.WriteLine("--- Exercise 1: Enhanced params Collections ---");

// TODO: Create a static method that uses params with ReadOnlySpan:
//       static void PrintAll(params ReadOnlySpan<string> items)
//       {
//           foreach (var item in items)
//               Console.Write($"{item} ");
//           Console.WriteLine();
//       }

// TODO: Create a generic sum method using params with ReadOnlySpan:
//       static T Sum<T>(params ReadOnlySpan<T> values) where T : INumber<T>
//       {
//           T result = T.Zero;
//           foreach (var v in values)
//               result += v;
//           return result;
//       }

// TODO: Call them:
//       Console.Write("  Items: ");
//       PrintAll("Alpha", "Beta", "Gamma");
//       Console.WriteLine($"  Sum: {Sum(10, 20, 30, 40)}");

Console.WriteLine();

// ----- Exercise 2: Refined Lock Semantics -----
Console.WriteLine("--- Exercise 2: Refined Lock Semantics ---");

// TODO: Define a 'Counter' class (see bottom of file) with:
//       - A private readonly Lock _lock = new();
//       - A private int _value = 0;
//       - public void Increment() { lock (_lock) { _value++; } }
//       - public int GetValue() { lock (_lock) { return _value; } }

// TODO: Test thread safety:
//       var counter = new Counter();
//       var tasks = Enumerable.Range(0, 100)
//           .Select(_ => Task.Run(() =>
//           {
//               for (int i = 0; i < 1000; i++)
//                   counter.Increment();
//           }));
//       await Task.WhenAll(tasks);
//       Console.WriteLine($"  Counter value (expected 100000): {counter.GetValue()}");

Console.WriteLine();

// ----- Exercise 3: The field Keyword -----
Console.WriteLine("--- Exercise 3: The field Keyword ---");

// TODO: Define a 'Person' class with a Name property that uses the field keyword:
//       public class Person
//       {
//           public required string Name
//           {
//               get => field;
//               set => field = value?.Trim() ?? throw new ArgumentNullException(nameof(value));
//           }
//       }

// TODO: Define a 'Percentage' class with a Value property clamped to 0-100:
//       public class Percentage
//       {
//           public double Value
//           {
//               get => field;
//               set => field = Math.Clamp(value, 0, 100);
//           }
//       }

// TODO: Test both:
//       var person = new Person { Name = "  Alice  " };
//       Console.WriteLine($"  Person name: '{person.Name}'");  // 'Alice'
//
//       var pct = new Percentage { Value = 150 };
//       Console.WriteLine($"  Percentage: {pct.Value}");  // 100
//       pct.Value = -10;
//       Console.WriteLine($"  Percentage: {pct.Value}");  // 0

Console.WriteLine();

// ----- Exercise 4: Null-Conditional Assignment -----
Console.WriteLine("--- Exercise 4: Null-Conditional Assignment ---");

// TODO: Define a 'Settings' class:
//       public class Settings
//       {
//           public string Theme { get; set; } = "Light";
//       }

// TODO: Test null-conditional assignment:
//       Settings? settings = null;
//       settings?.Theme = "Dark";     // No exception, assignment is skipped
//       Console.WriteLine($"  settings is null: {settings is null}");
//
//       settings = new Settings();
//       settings?.Theme = "Dark";     // Assignment succeeds
//       Console.WriteLine($"  Theme: {settings.Theme}");

Console.WriteLine();

// ----- Exercise 5: First-Class Span Support -----
Console.WriteLine("--- Exercise 5: First-Class Span Support ---");

// TODO: Create a method that counts vowels in a ReadOnlySpan<char>:
//       static int CountVowels(ReadOnlySpan<char> text)
//       {
//           int count = 0;
//           foreach (char c in text)
//               if ("aeiouAEIOU".Contains(c))
//                   count++;
//           return count;
//       }

// TODO: Create a method that finds an element in a Span<int>:
//       static int FindIndex(Span<int> data, int target)
//       {
//           for (int i = 0; i < data.Length; i++)
//               if (data[i] == target) return i;
//           return -1;
//       }

// TODO: Test:
//       string greeting = "Hello, World!";
//       Console.WriteLine($"  Vowels in '{greeting}': {CountVowels(greeting)}");
//
//       int[] nums = [10, 20, 30, 40, 50];
//       Console.WriteLine($"  Index of 30: {FindIndex(nums, 30)}");
//       Console.WriteLine($"  Index of 99: {FindIndex(nums, 99)}");

Console.WriteLine("\nLab 6 completed!");

// ----- Type definitions below -----
