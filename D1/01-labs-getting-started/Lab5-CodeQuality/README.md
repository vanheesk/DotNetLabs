# Lab 5: Code Quality & Consistency Basics

## Objective

Introduce professional code quality practices early. By the end of this lab you will have configured SDK pinning, nullable reference types, an `.editorconfig` file, and observed how analyzers provide automated feedback on your code.

---

## Prerequisites

- .NET 10 SDK installed (verified in Lab 0)
- A code editor (Visual Studio 2022+ or VS Code with C# Dev Kit)

---

## Exercise 1 – Pin the SDK with `global.json`

A `global.json` file ensures every developer and CI pipeline uses the same SDK version.

### Tasks

1. Open a terminal in the `starter` folder of this lab.
2. Check your current SDK version:
   ```bash
   dotnet --version
   ```
3. Examine the existing `global.json`:
   ```bash
   cat global.json
   ```
4. Notice the `rollForward` policy — it controls what happens when the exact version isn't available:
   ```json
   {
     "sdk": {
       "version": "10.0.100",
       "rollForward": "latestFeature"
     }
   }
   ```
5. Try changing the version to something you don't have installed and observe the error.
6. Restore it to a valid version.

> **Key insight:** `rollForward: "latestFeature"` means "use 10.0.xxx but allow a newer feature band". This is the safest setting for teams.

---

## Exercise 2 – Nullable Reference Types

Nullable reference types (NRT) help you avoid `NullReferenceException` at compile time. They've been the default since .NET 6, but understanding the warnings is essential.

### Tasks

1. Open `QualityApp/Program.cs` and build the project:
   ```bash
   dotnet build
   ```
2. Notice the **warnings** in the output — these come from nullable reference type analysis.
3. Open `QualityApp/UserService.cs` and examine the code. It has intentional nullable issues:
   - Methods that might return `null` but are declared as non-nullable
   - Parameters that could be `null` but aren't checked
4. Fix the warnings by:
   - Adding `?` to return types and parameters that can be null
   - Adding null checks where appropriate
   - Using the null-forgiving operator `!` **only** when you're certain a value isn't null
5. Build again and verify all nullable warnings are resolved.

---

## Exercise 3 – Add an `.editorconfig`

An `.editorconfig` file standardizes coding style across your team — indentation, naming conventions, brace placement, and more.

### Tasks

1. Examine the `.editorconfig` file in the `starter` folder. It contains common C# conventions:
   ```ini
   [*.cs]
   # Indentation
   indent_style = space
   indent_size = 4

   # Naming conventions
   dotnet_naming_rule.private_fields_should_be_camel_case.severity = warning
   ```
2. Build the project and notice any new warnings related to naming or style.
3. In `UserService.cs`, there are some intentional style violations:
   - A private field named `Users` instead of `_users`
   - Inconsistent brace placement
   - Missing `readonly` on fields that don't change
4. Fix these style issues according to the `.editorconfig` rules.
5. Build again — the style warnings should be resolved.

---

## Exercise 4 – Built-In Analyzers

.NET includes several built-in code analyzers. Let's see them in action.

### Tasks

1. Open `QualityApp.csproj` and note the analyzer settings:
   ```xml
   <AnalysisLevel>latest-recommended</AnalysisLevel>
   <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
   ```
2. Build the project:
   ```bash
   dotnet build
   ```
3. Review any remaining analyzer warnings. Common categories include:
   - **CA1XXX** — Design and usage rules
   - **IDE0XXX** — Code style preferences
   - **CS8XXX** — Nullable reference type warnings
4. Fix at least three analyzer warnings by following the suggested changes.
5. (Optional) To see **all** available analyzers:
   ```bash
   dotnet build /p:ReportAnalyzer=true
   ```

---

## Exercise 5 – Treat Warnings as Errors (Optional)

In CI/CD pipelines, you often want to enforce zero warnings.

### Tasks

1. Add the following to `QualityApp.csproj`:
   ```xml
   <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
   ```
2. Build the project:
   ```bash
   dotnet build
   ```
3. Any remaining warnings will now cause build failures.
4. Fix all issues until the build succeeds.
5. Remove `TreatWarningsAsErrors` when done (or keep it if you prefer strict builds).

---

## Wrapping Up

✅ You now understand:
- How `global.json` ensures SDK consistency across a team
- How nullable reference types prevent null-related bugs
- How `.editorconfig` enforces coding conventions
- How built-in analyzers catch quality issues automatically

Compare your work with the `solution` folder if you get stuck.
