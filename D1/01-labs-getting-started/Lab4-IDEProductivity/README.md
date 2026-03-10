# Lab 4: IDE Productivity – Visual Studio or VS Code

## Objective

Get productive quickly in your preferred IDE. By the end of this lab you will be able to open solutions, run and debug projects, execute tests, and use essential productivity features — whether you choose **Visual Studio** or **VS Code**.

---

## Prerequisites

- .NET 10 SDK installed (verified in Lab 0)
- **One** of the following installed:
  - [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community, Professional, or Enterprise)
  - [VS Code](https://code.visualstudio.com/) with the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension

---

## Choose Your Path

This lab has **two parallel tracks**. Pick the IDE you plan to use for the rest of the training.

---

## Path A – Visual Studio

### Exercise A1 – Open and Explore a Solution

1. Open Visual Studio.
2. Select **Open a project or solution** and navigate to the `starter` folder of this lab.
3. Open `Lab4-IDE.sln`.
4. In **Solution Explorer**, explore the project structure:
   - `TodoApp` — a console application
   - `TodoApp.Tests` — an xUnit test project
5. Double-click `Program.cs` in the `TodoApp` project and read through the code.

### Exercise A2 – Build and Run

1. Press **Ctrl+Shift+B** to build the solution (or **Build > Build Solution**).
2. Check the **Output** window for build results.
3. Set `TodoApp` as the startup project (right-click > **Set as Startup Project**).
4. Press **F5** to run with debugging, or **Ctrl+F5** to run without debugging.
5. Observe the console output.

### Exercise A3 – Debugging

1. Open `TodoService.cs` in the `TodoApp` project.
2. Set a **breakpoint** on the `AddTodo` method by clicking in the left margin (or pressing **F9**).
3. Press **F5** to start debugging.
4. When the breakpoint is hit:
   - Hover over variables to see their values.
   - Use the **Locals** and **Watch** windows.
   - Press **F10** to step over, **F11** to step into.
   - Press **F5** to continue.
5. Try using **Edit and Continue** — change a string value while paused and continue.

### Exercise A4 – Run Tests

1. Open **Test Explorer** (**Test > Test Explorer** or **Ctrl+E, T**).
2. Click **Run All Tests**.
3. Observe results — green checks for passing, red X for failing.
4. Double-click a test to navigate to its source code.
5. Right-click a test and select **Debug Test** to debug a specific test.

### Exercise A5 – Productivity Features

1. **Go to Definition:** Right-click a method name and select **Go to Definition** (or **F12**).
2. **Find All References:** Right-click a method name and select **Find All References** (or **Shift+F12**).
3. **Rename Refactoring:** Right-click a variable or method name > **Rename** (or **F2**). Change it and see all references update.
4. **Quick Actions:** Place your cursor on a squiggly underline and press **Ctrl+.** to see suggested fixes.
5. **Integrated Terminal:** Open it via **View > Terminal** (or **Ctrl+`**) and run `dotnet --version`.

---

## Path B – VS Code

### Exercise B1 – Open and Explore

1. Open VS Code.
2. Open the `starter` folder of this lab (**File > Open Folder**).
3. If prompted, trust the workspace.
4. VS Code should automatically detect the solution. Check the **Solution Explorer** panel in the sidebar (provided by C# Dev Kit).
5. Open `Program.cs` in the `TodoApp` project.

### Exercise B2 – Build and Run

1. Open the integrated terminal (**Ctrl+`**).
2. Build the solution:
   ```bash
   dotnet build
   ```
3. Run the project:
   ```bash
   dotnet run --project TodoApp
   ```
4. Alternatively, click the **Run** button (▶) that appears above `Main` or the top-level statements.

### Exercise B3 – Debugging

1. Open `TodoService.cs`.
2. Set a breakpoint by clicking in the left margin next to a line number.
3. Press **F5** to start debugging (you may need to select a launch configuration the first time).
4. When the breakpoint hits:
   - Hover over variables to see values.
   - Use the **Variables**, **Watch**, and **Call Stack** panels.
   - Press **F10** to step over, **F11** to step into.
5. Open the **Debug Console** to evaluate expressions while paused.

### Exercise B4 – Run Tests from Test Explorer

1. Open the **Testing** panel in the sidebar (flask icon).
2. You should see the tests from `TodoApp.Tests` listed.
3. Click the **Run All** button.
4. Observe results — passing tests show green checks.
5. Click a test to navigate to its source code.
6. Right-click a test and select **Debug Test** to debug it.

### Exercise B5 – Productivity Features

1. **Go to Definition:** Right-click a symbol > **Go to Definition** (or **F12**).
2. **Find All References:** Right-click > **Find All References** (or **Shift+F12**).
3. **Rename Symbol:** **F2** on any symbol, type a new name, and press Enter.
4. **Quick Fix:** Click the lightbulb (💡) that appears for warnings/suggestions, or press **Ctrl+.**.
5. **Command Palette:** **Ctrl+Shift+P** — search for ".NET" to see available commands.
6. **IntelliSense:** Start typing in a `.cs` file and observe auto-complete suggestions.

---

## Wrapping Up

✅ Regardless of which IDE you chose, you can now:
- Open and navigate a .NET solution
- Build and run projects
- Set breakpoints and debug code
- Run and debug unit tests
- Use essential refactoring and navigation features

The `starter` folder contains a ready-to-use project for both paths. The `solution` folder is identical since this lab is about IDE skills, not code changes.
