// ============================================================
// Lab 6: GitHub Copilot – Demo App
// Use Copilot to implement methods, generate tests, and refactor.
// ============================================================

using CopilotDemo;

Console.WriteLine("=== GitHub Copilot Demo ===\n");

// Test StringUtilities
Console.WriteLine("--- String Utilities ---");
Console.WriteLine($"Reverse 'Hello': {StringUtilities.Reverse("Hello")}");
Console.WriteLine($"IsPalindrome 'racecar': {StringUtilities.IsPalindrome("racecar")}");
Console.WriteLine($"IsPalindrome 'hello': {StringUtilities.IsPalindrome("hello")}");
Console.WriteLine($"WordCount 'The quick brown fox': {StringUtilities.WordCount("The quick brown fox")}");
Console.WriteLine($"Truncate 'Hello World' to 7: {StringUtilities.Truncate("Hello World", 7)}");
Console.WriteLine($"ToTitleCase 'hello world': {StringUtilities.ToTitleCase("hello world")}");

Console.WriteLine();

// Test MessyCode
Console.WriteLine("--- Messy Code (before refactoring) ---");
var result = MessyCode.ProcessOrders(
[
    new Order("ORD-001", 150.00m, "Electronics", true),
    new Order("ORD-002", 25.00m, "Books", false),
    new Order("ORD-003", 500.00m, "Electronics", true),
    new Order("ORD-004", 75.00m, "Clothing", true),
    new Order("ORD-005", 10.00m, "Books", false),
]);
Console.WriteLine($"Processed result: {result}");
