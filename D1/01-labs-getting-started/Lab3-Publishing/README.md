# Lab 3: Running & Publishing Applications (FDD vs SCD)

## Objective

Learn how to publish .NET applications for deployment. By the end of this lab you will understand the difference between **Framework-Dependent Deployments (FDD)** and **Self-Contained Deployments (SCD)**, and know when to use each.

---

## Prerequisites

- .NET 10 SDK installed (verified in Lab 0)
- Completed Labs 1 and 2

---

## Exercise 1 – Run the Application

Before publishing, let's review how to run an application in different ways.

### Tasks

1. Open a terminal in the `starter` folder of this lab.
2. Run the application using `dotnet run`:
   ```bash
   dotnet run --project WeatherApp
   ```
3. Build the application and run the resulting DLL directly:
   ```bash
   dotnet build --project WeatherApp
   dotnet WeatherApp/bin/Debug/net10.0/WeatherApp.dll
   ```
4. Notice both approaches produce the same output — `dotnet run` simply combines build + execute.

---

## Exercise 2 – Framework-Dependent Deployment (FDD)

A **Framework-Dependent Deployment** produces a small output because it relies on the .NET runtime being installed on the target machine.

### Tasks

1. Publish the application as framework-dependent (the default):
   ```bash
   dotnet publish WeatherApp -c Release -o publish/fdd
   ```
2. Examine the output folder:
   ```bash
   dir publish/fdd
   ```
   On macOS/Linux:
   ```bash
   ls -la publish/fdd
   ```
3. Note what's included:
   - `WeatherApp.dll` — the application assembly
   - `WeatherApp.exe` — a small native host (Windows) or no exe (Linux/macOS)
   - `WeatherApp.deps.json` — dependency manifest
   - `WeatherApp.runtimeconfig.json` — runtime configuration
4. Run the published app:
   ```bash
   dotnet publish/fdd/WeatherApp.dll
   ```
5. Note the total folder size — it should be very small (a few hundred KB).

> **When to use FDD:** When you control the target environment and can guarantee the .NET runtime is installed. Great for containers, servers, and managed environments.

---

## Exercise 3 – Self-Contained Deployment (SCD)

A **Self-Contained Deployment** bundles the entire .NET runtime with your application. It runs on machines **without** .NET installed.

### Tasks

1. Publish as self-contained for your current OS:
   ```bash
   dotnet publish WeatherApp -c Release -r win-x64 --self-contained true -o publish/scd
   ```
   > **Note:** Replace `win-x64` with your runtime identifier:
   > - Windows: `win-x64` or `win-arm64`
   > - macOS: `osx-x64` or `osx-arm64`
   > - Linux: `linux-x64` or `linux-arm64`

2. Examine the output folder:
   ```bash
   dir publish/scd
   ```
   Notice the **much larger** number of files — the entire .NET runtime is included.

3. Run the published app directly (no `dotnet` command needed):

   Windows:
   ```powershell
   .\publish\scd\WeatherApp.exe
   ```
   macOS/Linux:
   ```bash
   ./publish/scd/WeatherApp
   ```

4. Compare folder sizes between FDD and SCD:
   ```powershell
   # Windows PowerShell
   (Get-ChildItem publish/fdd -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
   (Get-ChildItem publish/scd -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
   ```

> **When to use SCD:** When deploying to machines where you cannot guarantee the runtime is installed, or when you need full control over the runtime version.

---

## Exercise 4 – Trimming and Single-File Publishing

You can reduce the size of self-contained deployments using **trimming** and **single-file** publishing.

### Tasks

1. Publish as a single file:
   ```bash
   dotnet publish WeatherApp -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/single
   ```
2. Examine the output — you should see mostly a single executable file.

3. (Optional) Enable trimming to further reduce size:
   ```bash
   dotnet publish WeatherApp -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -o publish/trimmed
   ```

4. Compare all four output sizes:

   | Publish Type | Approximate Size |
   |---|---|
   | FDD | ~100 KB |
   | SCD | ~70-80 MB |
   | Single File | ~70-80 MB (one file) |
   | Trimmed | ~15-20 MB |

5. Run each published version to verify they all work correctly.

---

## Wrapping Up

✅ You now understand:
- The difference between FDD and SCD
- When to choose each deployment strategy
- How to reduce deployment size with single-file and trimming
- The trade-offs between portability, size, and runtime dependency

Compare your work with the `solution` folder if you get stuck.
