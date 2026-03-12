# Lab 6: Throttling (Rate Limiting)

## Objective
Learn how to protect your API from excessive requests using ASP.NET Core's built-in rate limiting middleware with a **fixed window** policy.

## Concepts Covered
- `AddRateLimiter` with `AddFixedWindowLimiter`
- `UseRateLimiter` middleware
- `[EnableRateLimiting]` attribute on controller actions
- Configuring `PermitLimit`, `Window`, `QueueLimit`, and `QueueProcessingOrder`
- Changing the rejection status code to **429 Too Many Requests**

## Prerequisites
- .NET 10 SDK
- VS Code with REST Client extension (for `.http` files), or a browser

## Starting Point
Open the **starter** project. It contains a Pie Shop API with a `PiesController` that has `GetAll` and `GetPie` endpoints. There is **no rate limiting** configured yet.

---

## Instructions

### Part 1 — Add a Fixed Window Rate Limiter

1. Open `Program.cs`.

2. Add the required `using` statements at the top:
   ```csharp
   using System.Threading.RateLimiting;
   using Microsoft.AspNetCore.RateLimiting;
   ```

3. Register a rate limiter service with a **fixed window** policy named `"myWindowLimiter"`:
   - **PermitLimit**: 4 requests per window
   - **Window**: 60 seconds
   - **QueueProcessingOrder**: `OldestFirst`
   - **QueueLimit**: 2 (additional requests that can wait in a queue)

4. Add the `UseRateLimiter()` middleware call in the request pipeline (after `UseCors` and before `MapControllers`).

### Part 2 — Apply Rate Limiting to a Controller Action

5. Open `Controllers/PiesController.cs`.

6. Add the `[EnableRateLimiting("myWindowLimiter")]` attribute to the `GetPie` method only (not `GetAll`).
   - You will need: `using Microsoft.AspNetCore.RateLimiting;`

### Part 3 — Test the Rate Limiter

7. Run the API:
   ```bash
   dotnet run --launch-profile https
   ```

8. Use the `.http` file to call `GET /pies/1` multiple times rapidly:
   - Calls 1–4: **200 OK** (within the permit limit)
   - Calls 5–6: **queued** (within the queue limit, may respond slowly)
   - Call 7+: **503 Service Unavailable** (rejected — default status code)

9. Verify that `GET /pies` (GetAll) is **not** rate limited.

### Part 4 — Change the Rejection Status Code

10. The default rejection status code is 503 (Service Unavailable), but the semantically correct code is **429 Too Many Requests** ([RFC 6585](https://www.rfc-editor.org/rfc/rfc6585#section-4)).

11. In `Program.cs`, set the `RejectionStatusCode` property on the rate limiter to `429`:
    ```csharp
    builder.Services.AddRateLimiter(_ => _
        .AddFixedWindowLimiter(policyName: "myWindowLimiter", options =>
        {
            // ... options ...
        })
        .RejectionStatusCode = 429);
    ```

12. Restart the API and repeat the rapid calls. Rejected requests now return **429 Too Many Requests**.

---

## Validation
- `GET /pies` always returns **200 OK** regardless of request count (no rate limiting).
- `GET /pies/1` returns **200 OK** for the first 4 calls within 60 seconds.
- Calls 5–6 are queued and may respond after some delay.
- Calls beyond 6 return **429 Too Many Requests**.
- After waiting 60 seconds, the window resets and requests succeed again.

## Solution
Compare your work with the **solution** folder.
