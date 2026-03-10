# Lab 0: Environment Validation

## Objective

Ensure your development environment is correctly set up before we begin. By the end of this lab you will have verified that the .NET SDK, runtime, and supporting tools are working properly.

---

## Prerequisites

- A modern operating system (Windows 10+, macOS, or Linux)
- Internet access (for optional SDK installation)

---

## Exercise 1 – Verify .NET SDK Installation

The .NET SDK includes everything you need to build and run .NET applications. Let's make sure it's installed and accessible from the command line.

### Tasks

1. Open a terminal (Windows Terminal, PowerShell, bash, or zsh).
2. Run the following commands and verify the output:

   ```bash
   dotnet --version
   ```
   You should see a version number like `10.0.xxx`. If you get an error, the SDK is not installed or not on your PATH.

3. List all installed SDKs:
   ```bash
   dotnet --list-sdks
   ```
   Verify that a .NET 10 SDK appears in the list.

4. List all installed runtimes:
   ```bash
   dotnet --list-runtimes
   ```
   You should see entries for `Microsoft.NETCore.App`, `Microsoft.AspNetCore.App`, etc.

> **Troubleshooting:** If `dotnet` is not recognized, install the .NET 10 SDK from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) and restart your terminal.

---

## Exercise 2 – Confirm OS-Specific Tooling

### Windows

1. Verify that **Windows Terminal** is available (search for it in the Start menu).
2. Confirm `winget` is available:
   ```powershell
   winget --version
   ```
3. (Optional) Install the .NET SDK via winget if needed:
   ```powershell
   winget install Microsoft.DotNet.SDK.10
   ```

### macOS / Linux

1. Confirm your shell (bash, zsh, etc.) is working:
   ```bash
   echo $SHELL
   ```
2. Confirm a package manager is available:
   - macOS: `brew --version`
   - Ubuntu/Debian: `apt --version`
   - Fedora: `dnf --version`

---

## Exercise 3 – Pin the SDK Version with `global.json`

A `global.json` file lets you pin the SDK version for a project or folder. This ensures all team members use the same SDK version and prevents surprises when new SDKs are installed.

### Tasks

1. In any empty folder, create a `global.json` file:
   ```bash
   dotnet new globaljson --sdk-version 10.0.100
   ```
2. View the contents of the file:
   ```bash
   cat global.json
   ```
   You should see something like:
   ```json
   {
     "sdk": {
       "version": "10.0.100"
     }
   }
   ```
3. Run `dotnet --version` again in the same folder — it should now report `10.0.100` (or whichever version you pinned).
4. Delete or rename the `global.json` and run `dotnet --version` again to see the difference.

> **Why does this matter?** In teams and CI/CD pipelines, SDK pinning prevents "works on my machine" issues caused by different SDK versions.

---

## Exercise 4 – Quick Smoke Test

Let's confirm the full toolchain works end-to-end.

### Tasks

1. Create a temporary console app:
   ```bash
   mkdir smoke-test && cd smoke-test
   dotnet new console
   ```
2. Run it:
   ```bash
   dotnet run
   ```
   You should see `Hello, World!` printed to the console.
3. Clean up:
   ```bash
   cd ..
   rm -rf smoke-test
   ```
   On Windows (PowerShell):
   ```powershell
   cd ..
   Remove-Item -Recurse -Force smoke-test
   ```

---

## Wrapping Up

✅ You have verified:
- The .NET SDK is installed and accessible from the command line
- OS-specific tooling is in place
- SDK version pinning with `global.json` works
- A simple .NET application compiles and runs

You are ready to proceed to **Lab 1**!
