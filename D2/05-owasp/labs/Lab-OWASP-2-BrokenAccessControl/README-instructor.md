# Lab 2: Broken Access Control (OWASP A01:2021) — Instructor Guide

## Teaching Notes

This is the **#1 most critical** web application risk according to OWASP. Broken access control is extremely common because developers often focus on authentication but forget authorisation checks on individual resources.

### Key Points to Emphasise

1. **Authentication ≠ Authorisation** — knowing WHO the user is doesn't mean they can access EVERYTHING
2. **IDOR is trivial to exploit** — just change an ID in the URL
3. **Every endpoint needs access control** — don't rely on "security through obscurity"
4. **Server-side enforcement is mandatory** — never trust the client
5. **Deny by default** — endpoints should require authorisation unless explicitly public

### Demo Flow

1. Show the IDOR vulnerability — log in as Alice, access Bob's data
2. Fix with ownership checks
3. Show the unprotected admin endpoint
4. Fix with policy-based authorisation
5. Discuss audit logging for monitoring

---

## Exercise 1 – Solution

The vulnerability is in the endpoint that doesn't check ownership:

```csharp
app.MapGet("/users/{userId}/orders", (string userId) =>
{
    // No check that the current user == userId
    var userOrders = orders.Where(o => o.UserId == userId).ToList();
    return Results.Ok(userOrders);
}).RequireAuthorization();
```

Even though the endpoint requires authentication, it doesn't verify that the authenticated user is requesting their own data.

---

## Exercise 2 – Solution

```csharp
app.MapGet("/users/{userId}/orders", (string userId, HttpContext context) =>
{
    var currentUser = context.User.FindFirst("sub")?.Value;
    if (currentUser != userId)
    {
        return Results.Forbid();
    }

    var userOrders = orders.Where(o => o.UserId == userId).ToList();
    return Results.Ok(userOrders);
}).RequireAuthorization();
```

---

## Exercise 3 – Solution

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));

// Apply to the endpoint
app.MapGet("/admin/users", () => users)
    .RequireAuthorization("AdminOnly");
```

---

## Exercise 4 – Solution

```csharp
public class OwnershipFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var currentUser = httpContext.User.FindFirst("sub")?.Value;
        var routeUserId = httpContext.GetRouteValue("userId")?.ToString();

        if (currentUser != routeUserId)
        {
            var auditLog = httpContext.RequestServices.GetRequiredService<AuditLog>();
            auditLog.RecordDenied(currentUser ?? "anonymous", $"/users/{routeUserId}");
            return Results.Forbid();
        }

        return await next(context);
    }
}

public class AuditLog
{
    private readonly List<AuditEntry> _entries = [];

    public void RecordDenied(string user, string resource)
    {
        _entries.Add(new AuditEntry(DateTime.UtcNow, user, resource, "ACCESS_DENIED"));
    }

    public IReadOnlyList<AuditEntry> GetEntries() => _entries.AsReadOnly();
}

public record AuditEntry(DateTime Timestamp, string User, string Resource, string Action);
```

---

## Common Student Issues

1. **Confusing `FindFirst("sub")` with `Identity.Name`** — depends on JWT claims mapping
2. **Forgetting `.RequireAuthorization()`** on new endpoints
3. **Not understanding 401 vs 403** — 401 = not authenticated, 403 = authenticated but not authorised
