# Lab 6: Intro to GitHub Copilot

## Objective

Learn how to use GitHub Copilot as a **productivity assistant** — not an autopilot. By the end of this lab you will know how to generate code, generate tests, and refactor existing code with Copilot, while maintaining healthy skepticism and human oversight.

---

## Prerequisites

- .NET 10 SDK installed (verified in Lab 0)
- Visual Studio 2022+ or VS Code with:
  - [GitHub Copilot](https://marketplace.visualstudio.com/items?itemName=GitHub.copilot) extension installed
  - A valid GitHub Copilot subscription (Individual, Business, or Enterprise)
- Completed Labs 1–5

---

## Exercise 1 – Enable and Verify Copilot

### Tasks

1. Open your IDE (Visual Studio or VS Code).
2. Verify Copilot is active:
   - **VS Code:** Look for the Copilot icon (🤖) in the status bar. Click it to verify it's enabled.
   - **Visual Studio:** Check **Tools > Options > GitHub Copilot** — ensure it's signed in and enabled.
3. Open the `starter` folder of this lab.
4. Open `Program.cs` — you should see Copilot suggestions appear as you type (gray ghost text).

---

## Exercise 2 – Generate a Method with Copilot

Copilot excels at generating small, well-defined methods from comments or method signatures.

### Tasks

1. Open `StringUtilities.cs` in the `starter/CopilotDemo` project.
2. Notice the class has method signatures with `// TODO` comments describing what each method should do.
3. Place your cursor inside the first method body and start typing or press **Enter** after the comment.
4. Copilot should suggest an implementation. Press **Tab** to accept, or **Esc** to dismiss.
5. For each method:
   - Read the TODO comment
   - Let Copilot suggest an implementation
   - **Review the suggestion carefully** before accepting
   - Verify the implementation is correct
6. After completing all methods, run the project to test:
   ```bash
   dotnet run --project CopilotDemo
   ```

> **Important:** Always review Copilot's suggestions. It can produce code that *looks* right but has subtle bugs (off-by-one errors, incorrect edge-case handling, etc.).

---

## Exercise 3 – Generate Unit Tests with Copilot

One of Copilot's strongest use cases is generating unit tests for existing code.

### Tasks

1. Open `StringUtilitiesTests.cs` in the `CopilotDemo.Tests` project.
2. The file has test method stubs with `// TODO` comments.
3. For each test method:
   - Place your cursor inside the method body
   - Type a comment describing what you want to test, e.g.: `// Arrange - set up the input`
   - Let Copilot suggest the test implementation
   - Accept and verify the suggestion
4. Alternatively, use **Copilot Chat** (Ctrl+I in VS Code, or the Copilot chat panel):
   - Select the `StringUtilities` class
   - Ask: *"Generate xUnit tests for this class"*
   - Review and paste the generated tests
5. Run the tests:
   ```bash
   dotnet test
   ```

---

## Exercise 4 – Refactor with Copilot

Copilot can help refactor messy code into cleaner, more idiomatic C#.

### Tasks

1. Open `MessyCode.cs` — it contains a deliberately ugly method with:
   - Deep nesting
   - Magic numbers
   - Poor variable names
   - Repeated code
2. Select the entire method body.
3. Use **Copilot Chat** and ask:
   - *"Refactor this to be more readable and follow C# best practices"*
   - Or: *"Simplify this method using LINQ and pattern matching"*
4. Review the suggested refactoring:
   - Does it preserve the original behavior?
   - Is it actually more readable?
   - Are there any subtle changes in logic?
5. Apply the refactoring and verify by running the project.

---

## Exercise 5 – Copilot Best Practices Discussion

### Tasks

Think about and discuss (or note down) answers to these questions:

1. **When is Copilot most helpful?**
   - Boilerplate code (DTOs, CRUD operations)
   - Unit test generation
   - Regex patterns and string manipulation
   - Documentation comments

2. **When should you be cautious?**
   - Business logic with complex rules
   - Security-sensitive code (encryption, auth)
   - Code that interacts with external systems
   - Performance-critical paths

3. **What are good habits?**
   - Always read and understand generated code before accepting
   - Write clear comments and method signatures to guide Copilot
   - Use Copilot Chat for explanations: *"Explain this code"*
   - Don't accept large blocks of code without reviewing line by line

---

## Wrapping Up

✅ You now know how to:
- Use Copilot to generate method implementations from comments
- Generate unit tests for existing code
- Refactor code using Copilot Chat
- Apply healthy skepticism and best practices when using AI-assisted coding

Compare your work with the `solution` folder if you get stuck. Remember: **Copilot is a tool, not a replacement for understanding.**
