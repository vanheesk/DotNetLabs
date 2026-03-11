# Lab 8: Broken Object Property Level Authorization (OWASP API3:2023) — Instructor Guide

## Teaching Notes

Mass assignment is a very common vulnerability, especially in APIs. Developers often bind request bodies directly to domain models for convenience, accidentally exposing internal properties.

### Key Points to Emphasise

1. **Domain models ≠ API contracts** — they serve different purposes
2. **DTOs are essential** — input DTOs prevent mass assignment, output DTOs prevent data exposure
3. **AutoMapper doesn't solve the problem** — it can map bad data just as easily
4. **Swagger should document the actual contract** — not the internal model
5. **Role-specific responses** — admins may see more fields than regular users

### Demo Flow

1. Show mass assignment — submit extra fields and watch them get accepted
2. Fix with input DTOs — extra fields are now ignored
3. Show excessive data exposure — full model with password hashes returned
4. Fix with response DTOs — only appropriate fields returned

---

## Exercise 1 – Vulnerability Explanation

The vulnerable code directly binds to the domain model:

```csharp
app.MapPut("/api/users/{id}", (int id, UserEntity user) =>
{
    // user object has ALL properties including Role, Salary, etc.
    // The API framework deserializes whatever the client sends
    existingUser.Name = user.Name;
    existingUser.Email = user.Email;
    existingUser.Role = user.Role;  // Attacker-controlled!
    existingUser.Salary = user.Salary; // Attacker-controlled!
});
```

---

## Exercise 2 – Solution

```csharp
public record CreateUserRequest(string Name, string Email);
public record UpdateUserRequest(string Name, string Email);

app.MapPut("/api/users/{id}", (int id, UpdateUserRequest request) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    // Only map allowed properties
    user.Name = request.Name;
    user.Email = request.Email;
    // Role, Salary, etc. are NOT updated

    return Results.Ok(ToResponse(user));
});
```

---

## Exercise 3 – Solution

```csharp
public record UserResponse(int Id, string Name, string Email, string Role);
public record UserAdminResponse(int Id, string Name, string Email, string Role, decimal Salary, string? InternalNotes);

static UserResponse ToResponse(UserEntity user) =>
    new(user.Id, user.Name, user.Email, user.Role);

static UserAdminResponse ToAdminResponse(UserEntity user) =>
    new(user.Id, user.Name, user.Email, user.Role, user.Salary, user.InternalNotes);

// Endpoint returns appropriate DTO based on role
app.MapGet("/api/users/{id}", (int id, HttpContext context) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    if (context.User.IsInRole("admin"))
        return Results.Ok(ToAdminResponse(user));

    return Results.Ok(ToResponse(user));
});
```

---

## Exercise 4 – Solution

Using separate DTOs naturally enforces read/write boundaries:

- `CreateUserRequest` — accepts `Name`, `Email`, `Password` (write-only fields)
- `UserResponse` — returns `Id`, `Name`, `Email`, `Role` (read-only fields like `Id`)
- `Password` never appears in any response DTO

---

## Common Student Issues

1. **Still using the domain model in handlers** — habits die hard
2. **Mapping every field** — defeats the purpose of DTOs
3. **Not updating Swagger** — when DTOs change, ensure `.Produces<UserResponse>()` is updated
4. **Creating too many DTOs** — find the right balance for your API
