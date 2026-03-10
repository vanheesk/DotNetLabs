// ============================================================
// Lab 7: Advanced Pattern Matching
// Topics: Property Patterns, List Patterns,
//         Relational/Logical Patterns, Combining Patterns
// ============================================================

Console.WriteLine("=== Lab 7: Advanced Pattern Matching ===\n");

// ----- Exercise 1: Type and Declaration Patterns -----
Console.WriteLine("--- Exercise 1: Type and Declaration Patterns ---");

// TODO: Write a static method DescribeObject(object? obj) returning a string:
//       Use a switch expression with type patterns:
//       null             => "null"
//       string s         => $"String of length {s.Length}"
//       int n            => $"Integer: {n}"
//       double d         => $"Double: {d:F2}"
//       IEnumerable<object> list => $"Collection with {list.Count()} items"
//       _                => $"Unknown type: {obj.GetType().Name}"

// TODO: Test with different values:
//       object?[] testValues = [null, "Hello", 42, 3.14, new List<object> { 1, 2, 3 }, DateTime.Now];
//       foreach (var val in testValues)
//           Console.WriteLine($"  {val ?? "null"} => {DescribeObject(val)}");

Console.WriteLine();

// ----- Exercise 2: Property Patterns -----
Console.WriteLine("--- Exercise 2: Property Patterns ---");

// TODO: Define a record:
//       record Order(string Customer, decimal Amount, string Status, string Country);

// TODO: Write a static method EvaluateOrder(Order order) returning a string:
//       Use a switch expression with property patterns:
//       { Status: "Cancelled" }                    => "Order cancelled"
//       { Amount: > 1000, Country: "US" }          => "Large US order — needs review"
//       { Amount: > 1000 }                         => "Large international order"
//       { Status: "Pending", Amount: < 50 }        => "Small pending order — auto-approve"
//       _                                          => "Standard order"

// TODO: Test with various orders:
//       Order[] orders = [
//           new("Alice", 50m, "Pending", "NL"),
//           new("Bob", 2000m, "Confirmed", "US"),
//           new("Charlie", 1500m, "Confirmed", "DE"),
//           new("Diana", 25m, "Pending", "NL"),
//           new("Eve", 100m, "Cancelled", "US")
//       ];
//       foreach (var order in orders)
//           Console.WriteLine($"  {order.Customer}: {EvaluateOrder(order)}");

Console.WriteLine();

// ----- Exercise 3: Relational and Logical Patterns -----
Console.WriteLine("--- Exercise 3: Relational and Logical Patterns ---");

// TODO: Write a static method ClassifyBMI(double bmi) returning a string:
//       < 18.5             => "Underweight"
//       >= 18.5 and < 25   => "Normal"
//       >= 25 and < 30     => "Overweight"
//       _                  => "Obese"

// TODO: Write a static method ValidateAge(int age) returning a string:
//       < 0                => "Invalid"
//       <= 12              => "Child"
//       <= 17              => "Teenager"
//       <= 64              => "Adult"
//       _                  => "Senior"

// TODO: Write a static method IsNonEmptyString(object? value) returning bool:
//       Use: value is not null and not ""

// TODO: Test all three methods:
//       double[] bmis = [16.0, 22.5, 27.0, 35.0];
//       foreach (var bmi in bmis)
//           Console.WriteLine($"  BMI {bmi}: {ClassifyBMI(bmi)}");
//
//       int[] ages = [-1, 5, 15, 30, 70];
//       foreach (var age in ages)
//           Console.WriteLine($"  Age {age}: {ValidateAge(age)}");
//
//       Console.WriteLine($"  IsNonEmptyString(null): {IsNonEmptyString(null)}");
//       Console.WriteLine($"  IsNonEmptyString(\"\"): {IsNonEmptyString("")}");
//       Console.WriteLine($"  IsNonEmptyString(\"Hi\"): {IsNonEmptyString("Hi")}");

Console.WriteLine();

// ----- Exercise 4: List Patterns -----
Console.WriteLine("--- Exercise 4: List Patterns ---");

// TODO: Write a static method AnalyzeSequence(int[] seq) returning a string:
//       []                                          => "Empty sequence"
//       [var only]                                  => $"Single value: {only}"
//       [var first, .., var last] when first == last => $"Palindromic endpoints: {first}"
//       [> 0, > 0, > 0, ..]                        => "Starts with three positive numbers"
//       [.., 0]                                     => "Ends with zero"
//       _                                           => $"Unclassified sequence of length {seq.Length}"

// TODO: Test:
//       int[][] sequences = [
//           [],
//           [42],
//           [5, 3, 7, 5],
//           [1, 2, 3, 4],
//           [8, -1, 0],
//           [-1, 2, 3]
//       ];
//       foreach (var seq in sequences)
//           Console.WriteLine($"  [{string.Join(", ", seq)}] => {AnalyzeSequence(seq)}");

Console.WriteLine();

// ----- Exercise 5: Combining Patterns (HTTP Response) -----
Console.WriteLine("--- Exercise 5: Combining Patterns ---");

// TODO: Define a record:
//       record HttpResponse(int StatusCode, string? Body, Dictionary<string, string> Headers);

// TODO: Write a static method AnalyzeResponse(HttpResponse response) returning a string:
//       { StatusCode: 200, Body: not null }                 => "Success with data"
//       { StatusCode: 200, Body: null }                     => "Success but empty"
//       { StatusCode: 301 or 302 }                          => "Redirect"
//       { StatusCode: >= 400 and < 500, StatusCode: var c } => $"Client error ({c})"
//       { StatusCode: >= 500, StatusCode: var c }           => $"Server error ({c})"
//       { StatusCode: var c }                               => $"Unexpected status: {c}"

// TODO: Test:
//       HttpResponse[] responses = [
//           new(200, "{\"ok\":true}", new()),
//           new(200, null, new()),
//           new(301, null, new() { ["Location"] = "/new-url" }),
//           new(404, "Not Found", new()),
//           new(500, null, new())
//       ];
//       foreach (var resp in responses)
//           Console.WriteLine($"  HTTP {resp.StatusCode}: {AnalyzeResponse(resp)}");

Console.WriteLine("\nLab 7 completed!");

// ----- Type definitions below -----
