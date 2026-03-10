# Lab 2: Restore, Build, Test

## Objective

Understand the core build lifecycle of a .NET application. By the end of this lab you will know how to restore dependencies, build in different configurations, add a test project, and run tests — all from the command line.

---

## Prerequisites

- .NET 10 SDK installed (verified in Lab 0)
- Completed Lab 1 (or at least comfortable with `dotnet new` and `dotnet run`)

---

## Exercise 1 – Restore Dependencies

`dotnet restore` downloads NuGet packages that your project depends on. Most commands (build, run, test) do this implicitly, but understanding the explicit step is important for CI/CD pipelines.

### Tasks

1. Open a terminal in the `starter` folder of this lab.
2. Examine the existing solution — it contains a console project called `Calculator`:
   ```bash
   dotnet sln list
   ```
3. Restore dependencies explicitly:
   ```bash
   dotnet restore
   ```
4. Notice the `obj` folders that appear — these contain restored package references and build metadata.

> **Key insight:** In CI/CD pipelines, `dotnet restore` is often a separate step so packages can be cached independently from compilation.

---

## Exercise 2 – Build in Debug and Release

The default build configuration is **Debug**, which includes debug symbols and skips optimizations. **Release** builds are optimized for production.

### Tasks

1. Build the solution in Debug mode (the default):
   ```bash
   dotnet build
   ```
   Note the output path: `bin/Debug/net10.0/`

2. Build in Release mode:
   ```bash
   dotnet build -c Release
   ```
   Note the output path: `bin/Release/net10.0/`

3. Compare the contents of both output folders:
   - Debug contains `.pdb` files (debug symbols) and unoptimized assemblies.
   - Release contains optimized assemblies.

4. Run the app from both configurations:
   ```bash
   dotnet run --project Calculator
   dotnet run --project Calculator -c Release
   ```

---

## Exercise 3 – Add a Test Project

.NET has first-class support for testing. The `xunit` template creates a test project pre-configured with the xUnit testing framework.

### Tasks

1. Create a new xUnit test project:
   ```bash
   dotnet new xunit -n Calculator.Tests
   ```
2. Add it to the solution:
   ```bash
   dotnet sln add Calculator.Tests/Calculator.Tests.csproj
   ```
3. Add a reference from the test project to the Calculator project:
   ```bash
   dotnet add Calculator.Tests/Calculator.Tests.csproj reference Calculator/Calculator.csproj
   ```
4. Open `Calculator.Tests/UnitTest1.cs` and replace its content with:
   ```csharp
   using Calculator;

   namespace Calculator.Tests;

   public class MathServiceTests
   {
       [Fact]
       public void Add_TwoPositiveNumbers_ReturnsSum()
       {
           var result = MathService.Add(3, 5);
           Assert.Equal(8, result);
       }

       [Fact]
       public void Add_NegativeAndPositive_ReturnsCorrectResult()
       {
           var result = MathService.Add(-3, 5);
           Assert.Equal(2, result);
       }

       [Theory]
       [InlineData(0, 0, 0)]
       [InlineData(1, 1, 2)]
       [InlineData(-5, 5, 0)]
       [InlineData(100, 200, 300)]
       public void Add_VariousInputs_ReturnsExpectedSum(int a, int b, int expected)
       {
           var result = MathService.Add(a, b);
           Assert.Equal(expected, result);
       }

       [Fact]
       public void Subtract_ReturnsCorrectDifference()
       {
           var result = MathService.Subtract(10, 3);
           Assert.Equal(7, result);
       }

       [Fact]
       public void Multiply_ReturnsCorrectProduct()
       {
           var result = MathService.Multiply(4, 5);
           Assert.Equal(20, result);
       }

       [Fact]
       public void Divide_ByNonZero_ReturnsCorrectQuotient()
       {
           var result = MathService.Divide(10, 2);
           Assert.Equal(5, result);
       }

       [Fact]
       public void Divide_ByZero_ThrowsException()
       {
           Assert.Throws<DivideByZeroException>(() => MathService.Divide(10, 0));
       }
   }
   ```

---

## Exercise 4 – Run the Tests

### Tasks

1. Run all tests from the solution folder:
   ```bash
   dotnet test
   ```
2. Observe the test results in the terminal — all tests should pass.
3. Try running tests with more detail:
   ```bash
   dotnet test --verbosity normal
   ```
4. Run tests with a filter (only `Add` tests):
   ```bash
   dotnet test --filter "Add"
   ```

---

## Wrapping Up

✅ You now understand:
- The difference between `restore`, `build`, and `run`
- How Debug and Release configurations affect output
- How to create and run xUnit tests
- The build lifecycle that CI/CD pipelines automate

Compare your work with the `solution` folder if you get stuck.
