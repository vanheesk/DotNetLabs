using Greeter;

Console.Write("What is your name? ");
string? name = Console.ReadLine();
Console.WriteLine(GreetingService.Greet(name ?? "World"));
