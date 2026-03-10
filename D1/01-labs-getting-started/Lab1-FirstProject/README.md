# Lab 1: First .NET 10 Project with the CLI

## Objective

Build confidence using the .NET CLI without an IDE. By the end of this lab you will have created a console application, explored its project structure, and organized it inside a solution — all from the command line.

---

## Prerequisites

- .NET 10 SDK installed (verified in Lab 0)
- A terminal / command prompt

---

## Exercise 1 – Create a Console Application

The `dotnet new` command generates projects from built-in templates. The simplest is **console**.

### Tasks

1. Open a terminal in the `starter` folder of this lab.
2. Notice the folder already contains an empty solution file. Explore it:
   ```bash
   cat Lab1-FirstProject.sln
   ```
3. Create a new console application project:
   ```bash
   dotnet new console -n HelloDotNet
   ```
4. Navigate into the project folder and run it:
   ```bash
   cd HelloDotNet
   dotnet run
   ```
   You should see `Hello, World!` in the terminal.

---

## Exercise 2 – Explore the Project Structure

### Tasks

1. Open `Program.cs` in a text editor and examine the contents.
   - Notice there is **no `Main` method** — this is the **top-level statements** feature (C# 9+).
   - The entire file is just:
     ```csharp
     Console.WriteLine("Hello, World!");
     ```
2. Open the `.csproj` file (`HelloDotNet.csproj`) and note:
   - `<TargetFramework>net10.0</TargetFramework>` — targets .NET 10
   - `<OutputType>Exe</OutputType>` — builds an executable
   - `<Nullable>enable</Nullable>` — nullable reference types are on by default
   - `<ImplicitUsings>enable</ImplicitUsings>` — common `using` statements are auto-imported
3. Modify `Program.cs` to print a personalized greeting:
   ```csharp
   Console.Write("What is your name? ");
   string? name = Console.ReadLine();
   Console.WriteLine($"Hello, {name ?? "World"}!");
   ```
4. Run the project again with `dotnet run` and test it.

---

## Exercise 3 – Add the Project to a Solution

Solutions (`.sln` files) group related projects together. They are essential for multi-project applications and are used by IDEs like Visual Studio.

### Tasks

1. Navigate back to the `starter` folder (the parent of `HelloDotNet`):
   ```bash
   cd ..
   ```
2. Add the project to the existing solution:
   ```bash
   dotnet sln add HelloDotNet/HelloDotNet.csproj
   ```
3. Verify the project was added:
   ```bash
   dotnet sln list
   ```
   You should see `HelloDotNet\HelloDotNet.csproj` listed.
4. Build the entire solution from the solution folder:
   ```bash
   dotnet build
   ```

---

## Exercise 4 – Add a Second Project

### Tasks

1. Create a class library project:
   ```bash
   dotnet new classlib -n Greeter
   ```
2. Add it to the solution:
   ```bash
   dotnet sln add Greeter/Greeter.csproj
   ```
3. Open `Greeter/Class1.cs` and replace its content with:
   ```csharp
   namespace Greeter;

   public static class GreetingService
   {
       public static string Greet(string name) => $"Hello, {name}! Welcome to .NET 10.";
   }
   ```
4. Add a project reference from `HelloDotNet` to `Greeter`:
   ```bash
   dotnet add HelloDotNet/HelloDotNet.csproj reference Greeter/Greeter.csproj
   ```
5. Update `HelloDotNet/Program.cs` to use the library:
   ```csharp
   using Greeter;

   Console.Write("What is your name? ");
   string? name = Console.ReadLine();
   Console.WriteLine(GreetingService.Greet(name ?? "World"));
   ```
6. Build and run:
   ```bash
   dotnet build
   dotnet run --project HelloDotNet
   ```

---

## Wrapping Up

✅ You can now:
- Create .NET projects from the command line
- Understand the project structure (`Program.cs`, `.csproj`)
- Organize projects in a solution
- Add project references between projects

Compare your work with the `solution` folder if you get stuck.
