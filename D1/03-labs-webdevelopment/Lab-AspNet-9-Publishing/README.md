# Lab 9 – Publishing & Containers

## Objectives
- Publish an ASP.NET Core app using different strategies.
- Build a **container image** directly from the .NET SDK.
- Compare **framework-dependent**, **self-contained**, and **NativeAOT** publish variants.

## Prerequisites
- .NET 10 SDK
- Docker Desktop (for container exercises)

## Exercises

### Exercise 1 – Framework-Dependent Publish
1. Open `starter/` and explore the Minimal API.
2. Publish as framework-dependent:
   ```bash
   dotnet publish -c Release -o out/fdd
   ```
3. Inspect `out/fdd/` — note the small output size and lack of runtime files.

### Exercise 2 – Self-Contained Publish
1. Publish as self-contained for your current runtime:
   ```bash
   dotnet publish -c Release --self-contained -o out/sc
   ```
2. Compare the size of `out/sc/` with `out/fdd/`.
3. Enable trimming and re-publish:
   ```bash
   dotnet publish -c Release --self-contained -p:PublishTrimmed=true -o out/sc-trimmed
   ```
4. Compare sizes again.

### Exercise 3 – Container Image from SDK
1. Publish as a container image using the .NET SDK:
   ```bash
   dotnet publish -c Release /t:PublishContainer
   ```
2. Run the container:
   ```bash
   docker run -p 8080:8080 lab-aspnet-9
   ```
3. Navigate to `http://localhost:8080/info` to verify.

### Exercise 4 – Customise the Container
1. Open the `.csproj` and review the container properties.
2. Change the base image, container name, or port.
3. Re-publish and verify.

### Exercise 5 – NativeAOT (Bonus)
1. Uncomment `<PublishAot>true</PublishAot>` in the `.csproj`.
2. Publish:
   ```bash
   dotnet publish -c Release -o out/aot
   ```
3. Compare startup time and file size with previous variants.

## Folder Structure
```
starter/   – Project skeleton with TODOs
solution/  – Complete implementation with container config
```
