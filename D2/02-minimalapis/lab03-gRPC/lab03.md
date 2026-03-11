# Lab 03: Inter-Service Communication with gRPC

## Lab Overview

So far, your Pie Shop API handles everything in a single process — endpoints query the database directly. In production systems, APIs often need to communicate with other services. **gRPC** is a high-performance, contract-first RPC framework that is ideal for service-to-service communication in .NET.

In this lab, you will:

- Build a **gRPC Pie Catalog service** that owns the pie data
- Implement server streaming to efficiently deliver large datasets
- Call the gRPC service from your Minimal API, creating an API gateway pattern
- Apply resilience to the gRPC client channel

> **Estimated time:** 75 minutes

## Learning Objectives

By the end of this lab, you will be able to:

- Define `.proto` service contracts and generate C# code from them
- Implement a gRPC service backed by EF Core
- Use server streaming for efficient data delivery
- Register and use typed gRPC clients in ASP.NET Core
- Apply resilience pipelines to gRPC client channels
- Test gRPC services using `grpcurl` or gRPC reflection

## Prerequisites

- .NET 10 SDK installed
- **Completed Lab 02** (or use `lab03/start/` which contains the Lab 02 final solution plus a scaffolded gRPC project)
- `grpcurl` installed (optional, for testing) — [install guide](https://github.com/fullstorydev/grpcurl#installation)

> **Starter solutions available.** Each step has a matching starter project in the `lab03/` folder.
> ```shell
> cp -r lab03/step02-complete .
> cd PieShopApi && dotnet run
> ```

---

## Architecture

After this lab, you will have two projects:

```
PieShopApi          (Minimal API — HTTP endpoints, acts as API gateway)
    ↓ gRPC client
PieShop.Grpc        (gRPC service — owns pie data via EF Core)
```

The API project calls the gRPC service for catalog data, while still handling API-specific concerns (caching, rate limiting, HTTP responses).

---

## Step 1 — Create the gRPC Project & Proto Contract

> **Starting point:** `lab03/start/`

### 1a — Create the gRPC Project

```shell
dotnet new grpc -n PieShop.Grpc
```

### 1b — Define the Proto Contract

Replace the default `Protos/greet.proto` with `Protos/pie_catalog.proto`:

```protobuf
syntax = "proto3";

option csharp_namespace = "PieShop.Grpc";

package piecatalog;

service PieCatalogService {
  rpc GetPie (GetPieRequest) returns (PieReply);
  rpc ListPies (ListPiesRequest) returns (ListPiesReply);
  rpc GetPieStream (ListPiesRequest) returns (stream PieReply);
}

message GetPieRequest {
  int32 pie_id = 1;
}

message ListPiesRequest {
  string filter = 1;
  int32 page_size = 2;
  int32 after_id = 3;
}

message PieReply {
  int32 pie_id = 1;
  string name = 2;
  string short_description = 3;
  double price = 4;
  bool is_pie_of_the_week = 5;
  string category_name = 6;
}

message ListPiesReply {
  repeated PieReply pies = 1;
}
```

### 1c — Update the .csproj

Update `PieShop.Grpc.csproj` to reference the new proto file:

```xml
<ItemGroup>
  <Protobuf Include="Protos\pie_catalog.proto" GrpcServices="Server" />
</ItemGroup>
```

Remove the old `greet.proto` reference and the `GreeterService.cs` file.

### 1d — Add EF Core & Shared Data Layer

The gRPC service needs access to the same database. Add the EF Core packages:

```shell
cd PieShop.Grpc
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Copy the `Data/` folder from the PieShopApi project (the entity classes, DbContext, and configurations). Both projects will share the same database schema.

> **Checkpoint:** The project compiles. Running `dotnet build` produces no errors. The generated C# types from the proto file are available.

---

## Step 2 — Implement the gRPC Service

> **Falling behind?** Start from `lab03/step01-complete/`.

### 2a — Implement PieCatalogService

Create `Services/PieCatalogServiceImpl.cs`:

```csharp
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PieShop.Grpc;
using PieShopApi.Data;

namespace PieShop.Grpc.Services;

public class PieCatalogServiceImpl(PieShopDbContext db, ILogger<PieCatalogServiceImpl> logger)
    : PieCatalogService.PieCatalogServiceBase
{
    public override async Task<PieReply> GetPie(GetPieRequest request, ServerCallContext context)
    {
        logger.LogInformation("gRPC GetPie called for ID {PieId}", request.PieId);

        var pie = await db.Pies.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.PieId == request.PieId, context.CancellationToken);

        if (pie is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Pie {request.PieId} not found"));

        return MapToReply(pie);
    }

    public override async Task<ListPiesReply> ListPies(ListPiesRequest request, ServerCallContext context)
    {
        logger.LogInformation("gRPC ListPies called");

        var pies = await BuildQuery(request).ToListAsync(context.CancellationToken);

        var reply = new ListPiesReply();
        reply.Pies.AddRange(pies.Select(MapToReply));
        return reply;
    }

    // TODO: Implement GetPieStream in Step 3

    private IQueryable<PieEntity> BuildQuery(ListPiesRequest request)
    {
        IQueryable<PieEntity> query = db.Pies.Include(p => p.Category).OrderBy(p => p.PieId);

        if (!string.IsNullOrEmpty(request.Filter))
            query = query.Where(p => p.Name.Contains(request.Filter));

        if (request.AfterId > 0)
            query = query.Where(p => p.PieId > request.AfterId);

        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        return query.Take(pageSize);
    }

    private static PieReply MapToReply(PieEntity pie) => new()
    {
        PieId = pie.PieId,
        Name = pie.Name,
        ShortDescription = pie.ShortDescription ?? "",
        Price = (double)pie.Price,
        IsPieOfTheWeek = pie.IsPieOfTheWeek,
        CategoryName = pie.Category.Name
    };
}
```

### 2b — Register the Service

First, add the gRPC reflection package for testing with `grpcurl`:

```shell
dotnet add package Grpc.AspNetCore.Server.Reflection
```

In `Program.cs` of `PieShop.Grpc`:

```csharp
using Microsoft.EntityFrameworkCore;
using PieShop.Grpc.Services;
using PieShopApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddDbContext<PieShopDbContext>(options =>
    options.UseSqlite("Data Source=../PieShopApi/pieshop.db"));

var app = builder.Build();

app.MapGrpcService<PieCatalogServiceImpl>();
app.MapGrpcReflectionService();

app.Run();
```

### 2c — Test with grpcurl

Run the gRPC service:

```shell
cd PieShop.Grpc
dotnet run
```

In another terminal, test with grpcurl:

```shell
grpcurl -plaintext localhost:5001 list
grpcurl -plaintext -d '{"pie_id": 1}' localhost:5001 piecatalog.PieCatalogService/GetPie
grpcurl -plaintext -d '{"page_size": 3}' localhost:5001 piecatalog.PieCatalogService/ListPies
```

If you don't have grpcurl, you can use the `.http` file or write a quick console client.

> **Checkpoint:** The gRPC service responds correctly to `GetPie` and `ListPies` calls. Reflection works with `grpcurl list`.

---

## Step 3 — Server Streaming

> **Falling behind?** Start from `lab03/step02-complete/`.

Server streaming sends a sequence of messages over a single RPC call. This is efficient when returning large datasets — the client starts processing items before the entire result set is ready.

### 3a — Implement GetPieStream

Add this method to `PieCatalogServiceImpl`:

```csharp
public override async Task GetPieStream(
    ListPiesRequest request,
    IServerStreamWriter<PieReply> responseStream,
    ServerCallContext context)
{
    logger.LogInformation("gRPC GetPieStream started");

    var pies = BuildQuery(request);

    await foreach (var pie in pies.AsAsyncEnumerable().WithCancellation(context.CancellationToken))
    {
        await responseStream.WriteAsync(MapToReply(pie), context.CancellationToken);
        logger.LogInformation("Streamed pie {PieId}: {Name}", pie.PieId, pie.Name);
    }

    logger.LogInformation("gRPC GetPieStream completed");
}
```

### 3b — Test Streaming

```shell
grpcurl -plaintext -d '{"page_size": 10}' localhost:5001 piecatalog.PieCatalogService/GetPieStream
```

You should see each pie arrive as a separate JSON object in the output.

### 3c — Observe Cancellation

With grpcurl, press Ctrl+C mid-stream. On the server, the `CancellationToken` fires and the method stops streaming.

> **Checkpoint:** Server streaming works. Cancellation propagates correctly.

---

## Step 4 — gRPC Client in the API

> **Falling behind?** Start from `lab03/step03-complete/`.

Now connect the dots: make PieShopApi call PieShop.Grpc to fetch catalog data via gRPC instead of querying the database directly.

### 4a — Add gRPC Client Packages

In the `PieShopApi` project:

```shell
cd PieShopApi
dotnet add package Google.Protobuf
dotnet add package Grpc.Net.Client
dotnet add package Grpc.Net.ClientFactory
dotnet add package Grpc.Tools
```

### 4b — Reference the Proto File

Copy `pie_catalog.proto` to `PieShopApi/Protos/` and add to the `.csproj`:

```xml
<ItemGroup>
  <Protobuf Include="Protos\pie_catalog.proto" GrpcServices="Client" />
</ItemGroup>
```

Note: `GrpcServices="Client"` — this generates only the client stub, not the server implementation.

### 4c — Register the Typed gRPC Client

In `PieShopApi/Program.cs`:

```csharp
builder.Services.AddGrpcClient<PieCatalogService.PieCatalogServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:5001");
});
```

### 4d — Add a Catalog Endpoint

Add a new route group that fetches data via gRPC:

```csharp
var catalogGroup = app.MapGroup("/catalog");

catalogGroup.MapGet("/pies", async (PieCatalogService.PieCatalogServiceClient client) =>
{
    var reply = await client.ListPiesAsync(new ListPiesRequest { PageSize = 20 });
    return TypedResults.Ok(reply.Pies.Select(p => new PieDto(
        p.PieId, p.Name, p.ShortDescription, (decimal)p.Price, p.IsPieOfTheWeek, p.CategoryName)));
})
.WithName("Catalog_ListPies");

catalogGroup.MapGet("/pies/{id:int}", async (int id, PieCatalogService.PieCatalogServiceClient client) =>
{
    try
    {
        var reply = await client.GetPieAsync(new GetPieRequest { PieId = id });
        return Results.Ok(new PieDto(
            reply.PieId, reply.Name, reply.ShortDescription, (decimal)reply.Price, reply.IsPieOfTheWeek, reply.CategoryName));
    }
    catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
    {
        return Results.NotFound();
    }
})
.WithName("Catalog_GetPie");
```

### 4e — Test End-to-End

1. Start PieShop.Grpc: `cd PieShop.Grpc && dotnet run`
2. Start PieShopApi: `cd PieShopApi && dotnet run`
3. Browse to `GET /catalog/pies` — data comes through the gRPC service
4. Compare with `GET /pies` — data comes directly from the database

> **Checkpoint:** `/catalog/pies` returns data fetched via gRPC from PieShop.Grpc. `/pies` still queries the database directly. Both work simultaneously.

---

## Step 5 — Resilience on the gRPC Client

> **Falling behind?** Start from `lab03/step04-complete/`.

What happens when the gRPC service is down? Without resilience, your API returns a 500 error immediately. Let's add retry and circuit-breaker behavior to the gRPC channel.

### 5a — Add Resilience to the gRPC Client

```shell
dotnet add package Microsoft.Extensions.Http.Resilience
```

Update the gRPC client registration:

```csharp
builder.Services.AddGrpcClient<PieCatalogService.PieCatalogServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:5001");
})
.AddStandardResilienceHandler();
```

`AddStandardResilienceHandler()` provides a pre-configured resilience pipeline with:
- Retry (exponential backoff)
- Circuit breaker
- Timeout
- Rate limiter (concurrency)

### 5b — Test Failure Scenarios

1. Start both services and verify `/catalog/pies` works
2. Stop the gRPC service (Ctrl+C on PieShop.Grpc)
3. Call `/catalog/pies` — observe retry attempts in the API logs
4. After multiple failures, the circuit breaker opens and subsequent requests fail fast
5. Restart the gRPC service — after the break duration, requests succeed again

### 5c — Customize the Pipeline (Optional)

If you want finer control, replace `AddStandardResilienceHandler()` with a custom configuration:

```csharp
.AddResilienceHandler("grpc-pipeline", pipelineBuilder =>
{
    pipelineBuilder
        .AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            BackoffType = DelayBackoffType.Exponential
        })
        .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            BreakDuration = TimeSpan.FromSeconds(10)
        })
        .AddTimeout(TimeSpan.FromSeconds(3));
});
```

> **Checkpoint:** The gRPC client retries on failure. When the gRPC service is down, the circuit breaker prevents flood failures.

---

## Step 6 — Stretch: Deadlines & Cancellation

> **Falling behind?** Start from `lab03/step05-complete/`.

gRPC supports **deadlines** — a hard timeout that propagates from client to server. Unlike HTTP timeouts, gRPC deadlines are part of the protocol and are respected by both sides.

### 6a — Set a Deadline on the Client

```csharp
catalogGroup.MapGet("/pies/slow", async (PieCatalogService.PieCatalogServiceClient client) =>
{
    var deadline = DateTime.UtcNow.AddSeconds(2);
    var reply = await client.ListPiesAsync(
        new ListPiesRequest { PageSize = 100 },
        deadline: deadline);

    return TypedResults.Ok(reply.Pies.Count);
});
```

### 6b — Simulate a Slow Server

In `PieCatalogServiceImpl.ListPies`, add a delay:

```csharp
await Task.Delay(5000, context.CancellationToken); // Simulate slow processing
```

### 6c — Observe Behavior

1. Call `/catalog/pies/slow`
2. After 2 seconds, the client receives a `DeadlineExceeded` RpcException
3. On the server, the `CancellationToken` fires on the delay call
4. The server stops processing — no wasted work

### 6d — Handle the Deadline Gracefully

```csharp
try
{
    var reply = await client.ListPiesAsync(request, deadline: deadline);
    return Results.Ok(reply.Pies.Count);
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
{
    return Results.Problem(
        title: "Catalog service timeout",
        detail: "The catalog service did not respond within the deadline.",
        statusCode: 504);
}
```

> **Checkpoint:** Deadlines propagate from client to server. The client gets a timeout error. The server stops processing.

---

## Lab Checkpoint

Your Pie Shop now has two services:

- ✅ **PieShopApi** — HTTP Minimal API with direct DB access, output caching, resilience, and rate limiting
- ✅ **PieShop.Grpc** — gRPC service with unary and server-streaming RPCs
- ✅ API gateway pattern — `/catalog/*` endpoints proxy to gRPC service
- ✅ Client-side resilience with retry and circuit-breaker
- ✅ gRPC deadlines with cancellation propagation

---

## Reflection Questions

1. When would you choose gRPC over HTTP/REST for inter-service communication?
2. How does server streaming compare to returning a large JSON array in a single response?
3. What is the difference between a gRPC deadline and an HTTP timeout?
4. How would you share proto files between projects in a real-world solution?

---

## Lab Complete

> **Reference solution:** `lab03/step06-complete/` contains the final state of both projects.

You have completed all three labs in the Minimal APIs module. Your Pie Shop now demonstrates:

- Modern CRUD API with TypedResults and validation (Lab 01)
- EF Core persistence with caching, resilience, and rate limiting (Lab 02)
- Inter-service communication with gRPC (Lab 03)
