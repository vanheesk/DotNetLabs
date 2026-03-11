# Lab 2: Broken Access Control (OWASP A01:2021)

## Objective

Understand **Broken Access Control** vulnerabilities including **Insecure Direct Object References (IDOR)** and **missing function-level access control**. Learn to mitigate them with proper authorisation checks.

---

## Background

**Broken Access Control** is the **#1 risk** in the OWASP Top 10 (2021). It occurs when users can act outside their intended permissions. Common failures include:

- **IDOR** — Accessing other users' resources by manipulating IDs
- **Missing function-level access control** — Accessing admin endpoints without authorisation
- **Elevation of privilege** — Acting as a user without being logged in, or acting as an admin when logged in as a user

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs and JWT authentication

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Identify IDOR Vulnerability

The starter project has user-specific order endpoints. The current implementation does **not verify** that the authenticated user owns the resource.

### Tasks

1. Run the starter project and navigate to `/swagger`.
2. Create a token for user `alice` using `POST /auth/token`.
3. Access Alice's orders via `GET /users/alice/orders` — this works correctly.
4. **Without changing the token**, try accessing `GET /users/bob/orders`.
5. Observe that Alice can see Bob's orders — this is an **IDOR vulnerability**.

---

## Exercise 2 – Fix IDOR with Ownership Checks

### Tasks

1. Modify the `GET /users/{userId}/orders` endpoint to verify that the authenticated user matches the `{userId}` parameter.
2. Extract the user identity from `HttpContext.User.FindFirst("sub")`.
3. Return `Results.Forbid()` when the user ID doesn't match.
4. Test that Alice can still access her own orders, but not Bob's.

> **Hint:**
> ```csharp
> var currentUser = context.User.FindFirst("sub")?.Value;
> if (currentUser != userId)
>     return Results.Forbid();
> ```

---

## Exercise 3 – Missing Function-Level Access Control

The starter has an admin endpoint `GET /admin/users` that lists all users but has **no authorisation** applied.

### Tasks

1. Access `GET /admin/users` without any token — it should fail but currently succeeds.
2. Add `.RequireAuthorization("AdminOnly")` to the admin endpoint.
3. Register the `AdminOnly` policy to require the `admin` role:
   ```csharp
   builder.Services.AddAuthorizationBuilder()
       .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
   ```
4. Test that a token with role `user` gets **403 Forbidden**, and a token with role `admin` succeeds.

---

## Exercise 4 – Defence in Depth

### Tasks

1. Create a reusable `OwnershipFilter` that checks if the authenticated user matches a route parameter.
2. Apply it to all user-specific endpoints.
3. Add an `AuditLog` that records denied access attempts with timestamp, user, and attempted resource.
4. Create a `GET /admin/audit-log` endpoint (admin only) to view the audit trail.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- Always verify **object-level** ownership before returning data
- Apply **function-level** access control to admin endpoints
- Use **policy-based authorisation** for role-based access
- Log access control failures for monitoring
