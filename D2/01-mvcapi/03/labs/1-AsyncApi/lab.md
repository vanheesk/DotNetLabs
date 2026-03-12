# Lab 1: Async API

## Objective

Learn how to properly work with `async`/`await` and cancellation tokens in ASP.NET Core Web API controllers.

## Prerequisites

- .NET 10 SDK
- A tool to send HTTP requests (e.g. the REST Client extension in VS Code, Postman, or curl)

## Getting Started

Open the `starter/AsyncApi` project:

```bash
cd starter/AsyncApi
dotnet run
```

The API will be available at `https://localhost:7043`.

---

## Part 1 — Making the PiesController Async

The `PiesController` currently uses **synchronous** methods. Your task is to convert them to use `async`/`await`.

### Step 1: Review the current code

Open `Controllers/PiesController.cs`. Notice that the methods call the repository synchronously.

### Step 2: Update `GetAll` to be async

1. Change the return type from `ActionResult<IEnumerable<Pie>>` to `Task<ActionResult<IEnumerable<Pie>>>`
2. Add the `async` keyword to the method signature
3. Use `await` when calling `_pieRepository.GetAllPiesAsync()`

### Step 3: Update `GetById` to be async

Apply the same pattern to the `GetById` method:
1. Change the return type to `Task<ActionResult<Pie>>`
2. Add the `async` keyword
3. Use `await` when calling `_pieRepository.GetPieByIdAsync(id)`

### Step 4: Test your changes

```
GET https://localhost:7043/api/pies
GET https://localhost:7043/api/pies/1
```

Both endpoints should return pie data as before, but now they're non-blocking.

---

## Part 2 — Using Cancellation Tokens

The `SlowController` simulates a slow endpoint (10-second delay). Currently, if a client cancels the request, the server continues processing until the delay completes.

### Step 5: Test the current behavior

1. Send a request to `GET https://localhost:7043/slow`
2. Cancel the request before the 10 seconds are up (close the browser tab, or cancel in Postman)
3. Observe the console output — notice that **"End slow request"** still appears even after cancellation

### Step 6: Add a `CancellationToken` parameter

Open `Controllers/SlowController.cs` and modify the `Get` method:

1. Add a `CancellationToken cancellationToken` parameter to the method signature
2. Pass the `cancellationToken` to `Task.Delay()` — e.g. `await Task.Delay(10_000, cancellationToken);`

> ASP.NET Core automatically provides a `CancellationToken` that is cancelled when the client disconnects.

### Step 7: Test with the cancellation token

1. Send a request to `GET https://localhost:7043/slow`
2. Cancel the request before it completes
3. Check the console — **"End slow request"** should **not** appear because the `Task.Delay` was cancelled

---

## Verification

When you have completed the lab:

- `GET /api/pies` returns all pies asynchronously
- `GET /api/pies/1` returns a single pie asynchronously
- `GET /slow` responds to cancellation — cancelling the client request immediately stops server-side processing
