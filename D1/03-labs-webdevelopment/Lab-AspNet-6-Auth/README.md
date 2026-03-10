# Lab 6: Authentication & Authorisation

## Objective

Configure **JWT Bearer** authentication, protect endpoints with **RequireAuthorization()**, define **policies**, and test 401/403 flows using a development token endpoint.

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Configure JWT Authentication

### Tasks

1. Add the JWT Bearer package:
   ```bash
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   ```
2. Configure JWT authentication in `Program.cs`:
   ```csharp
   builder.Services.AddAuthentication("Bearer")
       .AddJwtBearer(options => { ... });
   ```
3. Set up a symmetric signing key (for development purposes only). Store it in `appsettings.json`.
4. Add `app.UseAuthentication()` and `app.UseAuthorization()` in the correct order.

---

## Exercise 2 – Create a Dev Token Endpoint

For testing without a real identity provider, create an endpoint that issues JWT tokens.

### Tasks

1. Create a `POST /auth/token` endpoint that accepts a `LoginRequest(string Username, string Role)`.
2. Generate a JWT token with claims for `sub`, `name`, and `role`.
3. Return the token in the response.
4. Use the token in Swagger's "Authorize" button or in `curl` with `Authorization: Bearer <token>`.

---

## Exercise 3 – Protect Endpoints

### Tasks

1. Create a `GET /secure` endpoint and protect it with `.RequireAuthorization()`.
2. Verify that calling `/secure` without a token returns **401 Unauthorized**.
3. Obtain a token from `/auth/token` and call `/secure` with the `Authorization: Bearer <token>` header — it should succeed.
4. Return the authenticated user's claims in the response.

---

## Exercise 4 – Policy-Based Authorisation

### Tasks

1. Define an authorisation policy that requires the `Admin` role:
   ```csharp
   builder.Services.AddAuthorizationBuilder()
       .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
   ```
2. Create a `GET /admin` endpoint protected by the `AdminOnly` policy.
3. Generate a token with `Role = "User"` — verify you get **403 Forbidden**.
4. Generate a token with `Role = "Admin"` — verify access is granted.

---

## Wrapping Up

```bash
dotnet run
```

Compare with the `solution` folder if needed. In production, you would use a real identity provider (Azure AD, Auth0, etc.) instead of the dev token endpoint.
