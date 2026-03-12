# Lab 2: Content Negotiation

## Objective

Learn how the `Accept` header enables content negotiation, how to add XML support, handle unsupported MIME types, and create a custom CSV output formatter.

## Prerequisites

- .NET 10 SDK
- VS Code with the REST Client extension (for `.http` files), or another HTTP testing tool

## Getting Started

Open the `starter/PieShopApi` project:

```bash
cd starter/PieShopApi
dotnet run
```

The API will be available at `https://localhost:7043`.

Use the included `PieShopApi.http` file for testing.

---

## Part 1 — Adding XML Support

The starter project only supports JSON responses. You will add XML content negotiation.

### Step 1: Enable content negotiation and XML formatters

Open `Program.cs` and change `AddControllers()` to:

```csharp
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
}).AddXmlDataContractSerializerFormatters();
```

### Step 2: Test with the `.http` file

- **Without** `Accept` header → returns JSON (default)
- **With** `Accept: application/json` → returns JSON
- **With** `Accept: application/xml` → returns XML

---

## Part 2 — Handling Unsupported MIME Types

By default, ASP.NET Core falls back to JSON if the requested MIME type is not supported. You will change this to return `406 Not Acceptable`.

### Step 3: Test the current fallback behavior

Send a request with `Accept: application/csv` — notice it falls back to JSON.

### Step 4: Enable `ReturnHttpNotAcceptable`

In `Program.cs`, add `ReturnHttpNotAcceptable = true` to the controller options:

```csharp
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters();
```

### Step 5: Test again

Send the same `Accept: application/csv` request — it should now return **HTTP 406 Not Acceptable**.

---

## Part 3 — Custom CSV Formatter

Create a custom output formatter that produces `application/csv` responses.

### Step 6: Create the `PieCsvFormatter` class

Create a new file `Formatters/PieCsvFormatter.cs`:

```csharp
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using PieShopApi.Models;

namespace PieShopApi.Formatters;

public class PieCsvFormatter : TextOutputFormatter
{
    public PieCsvFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/csv"));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type? type)
        => typeof(PieDto).IsAssignableFrom(type)
            || typeof(IEnumerable<PieDto>).IsAssignableFrom(type);

    public override async Task WriteResponseBodyAsync(
        OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;
        var buffer = new StringBuilder();

        if (context.Object is IEnumerable<PieDto> pies)
        {
            foreach (var pie in pies)
            {
                FormatCsv(buffer, pie);
            }
        }
        else
        {
            FormatCsv(buffer, (PieDto)context.Object!);
        }

        await httpContext.Response.WriteAsync(buffer.ToString(), selectedEncoding);
    }

    private static void FormatCsv(StringBuilder buffer, PieDto pie)
    {
        buffer.AppendLine($"{pie.Id},{pie.Name},{pie.Description},{string.Join("|", pie.AllergyItems)}");
    }
}
```

### Step 7: Register the formatter in `Program.cs`

```csharp
using PieShopApi.Formatters;

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
    options.OutputFormatters.Add(new PieCsvFormatter());
}).AddXmlDataContractSerializerFormatters();
```

### Step 8: Test CSV output

- `GET /pies/1` with `Accept: application/csv` → returns a single pie as CSV
- `GET /pies` with `Accept: application/csv` → returns all pies as CSV

---

## Verification

When you have completed the lab:

- `Accept: application/json` returns JSON
- `Accept: application/xml` returns XML
- Unsupported MIME types return **406 Not Acceptable**
- `Accept: application/csv` returns CSV data using your custom formatter
