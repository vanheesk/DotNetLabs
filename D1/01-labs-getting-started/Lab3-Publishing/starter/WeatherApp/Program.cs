// ============================================================
// Lab 3: Publishing – WeatherApp
// A simple console app to demonstrate different publish modes.
// ============================================================

Console.WriteLine("=== Weather Forecast ===");
Console.WriteLine();

string[] cities = ["Amsterdam", "Brussels", "London", "New York", "Tokyo"];
string[] conditions = ["Sunny", "Cloudy", "Rainy", "Snowy", "Windy", "Foggy"];

var random = new Random(42); // Fixed seed for reproducible output

foreach (var city in cities)
{
    int temperature = random.Next(-10, 40);
    string condition = conditions[random.Next(conditions.Length)];
    Console.WriteLine($"  {city,-15} {temperature,3}°C  {condition}");
}

Console.WriteLine();
Console.WriteLine($"Runtime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"OS:      {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
Console.WriteLine($"Arch:    {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
