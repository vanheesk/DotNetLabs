# Lab 8: Broken Object Property Level Authorization (OWASP API3:2023)

## Objective

Understand **Mass Assignment** and **Excessive Data Exposure** vulnerabilities. Learn to use DTOs, input validation, and projections to protect object properties from unauthorized access and modification.

---

## Background

**API3:2023** combines two related issues:

1. **Mass Assignment** — An API endpoint binds client input directly to internal data models, allowing attackers to modify properties they shouldn't (e.g., setting `IsAdmin = true`)
2. **Excessive Data Exposure** — An API returns more data than the client needs, exposing sensitive fields (e.g., returning password hashes, internal IDs, or salary information)

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Identify Mass Assignment Vulnerability

The starter has a user profile update endpoint that binds directly to the domain model.

### Tasks

1. Run the starter project and navigate to `/swagger`.
2. Create a user via `POST /api/users` with `{ "name": "Alice", "email": "alice@example.com" }`.
3. Update the user via `PUT /api/users/1` to change the name — this works correctly.
4. Now include an extra property in the body:
   ```json
   { "name": "Alice", "email": "alice@example.com", "role": "admin", "salary": 999999 }
   ```
5. Fetch the user with `GET /api/users/1` — the role and salary were changed! This is **mass assignment**.

---

## Exercise 2 – Fix with Input DTOs

### Tasks

1. Create a `CreateUserRequest` DTO with only the allowed input fields: `Name` and `Email`.
2. Create an `UpdateUserRequest` DTO with only `Name` and `Email`.
3. Replace the domain model in the endpoint handlers with these DTOs.
4. Map from DTO to domain model explicitly, ignoring protected fields.
5. Test that submitting `role` or `salary` in the request body has no effect.

> **Hint:**
> ```csharp
> public record CreateUserRequest(string Name, string Email);
> public record UpdateUserRequest(string Name, string Email);
> ```

---

## Exercise 3 – Fix Excessive Data Exposure

The API returns the full user object including `PasswordHash`, `InternalNotes`, and `Salary` to all consumers.

### Tasks

1. Create a `UserResponse` DTO with only public fields: `Id`, `Name`, `Email`, `Role`.
2. Modify GET endpoints to return `UserResponse` instead of the full domain model.
3. Create a `UserAdminResponse` DTO that includes additional fields for admin users.
4. Return the appropriate DTO based on the caller's role.
5. Verify that sensitive fields are no longer in the API response.

---

## Exercise 4 – Read-Only and Write-Only Properties

### Tasks

1. Mark `Id` and `CreatedAt` as read-only — they should appear in responses but not be modifiable via input.
2. Mark `Password` as write-only — it should be accepted in registration but never returned.
3. Use `[JsonIgnore]` or separate DTOs to enforce this.
4. Verify the contract using the Swagger documentation.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- **Never bind input directly to domain models** — always use input DTOs
- **Never return full domain objects** — use response DTOs with only public fields
- **Map explicitly** between DTOs and domain models
- Different **roles may see different fields** — use role-specific response DTOs
- **Swagger/OpenAPI** should reflect the actual contract (input ≠ output ≠ domain)
