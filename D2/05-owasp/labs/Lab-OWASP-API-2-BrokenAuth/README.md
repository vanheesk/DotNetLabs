# Lab 7: Broken Authentication (OWASP API2:2023)

## Objective

Understand **Broken Authentication** in APIs, including weak token validation, missing expiration, and insecure credential handling. Learn to implement proper JWT authentication with secure defaults.

---

## Background

**Broken Authentication** is the **#2 API security risk**. APIs are especially vulnerable because:

- Authentication mechanisms are often complex and error-prone
- APIs frequently use **tokens** (JWT) which can be misconfigured
- APIs may **not rate-limit** authentication endpoints
- APIs may accept **weakly signed** or **expired** tokens

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs and JWT

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Identify Weak Token Validation

The starter project has several JWT configuration issues that an attacker can exploit.

### Tasks

1. Examine the JWT configuration in the starter project.
2. Identify the following weaknesses:
   - Token lifetime is set to **30 days** (too long)
   - Token validation **skips issuer and audience checks**
   - The signing key is too simple/short
3. Test that tokens work, then discuss the risks of each weakness.

---

## Exercise 2 – Fix Token Configuration

### Tasks

1. Set token expiration to a reasonable duration (e.g., **15 minutes** for access tokens).
2. Enable issuer and audience validation:
   ```csharp
   ValidateIssuer = true,
   ValidIssuer = "https://your-api.example.com",
   ValidateAudience = true,
   ValidAudience = "your-api-audience",
   ```
3. Use a strong, random signing key (at least 32 bytes / 256 bits).
4. Test that tokens with wrong issuer or audience are rejected.

---

## Exercise 3 – Implement Refresh Tokens

Short-lived access tokens need a mechanism to get new tokens without re-authenticating.

### Tasks

1. Create a `POST /auth/refresh` endpoint that accepts a refresh token.
2. Issue refresh tokens alongside access tokens during login.
3. Refresh tokens should have a longer lifetime (e.g., 7 days).
4. Implement refresh token **rotation** — each refresh invalidates the previous one.
5. Store refresh tokens server-side and validate them on use.

> **Hint:**
> ```csharp
> var refreshToken = Guid.NewGuid().ToString();
> refreshTokens[refreshToken] = new RefreshTokenInfo(userId, DateTime.UtcNow.AddDays(7));
> ```

---

## Exercise 4 – Secure the Login Endpoint

### Tasks

1. Add rate limiting to the login endpoint to prevent brute force attacks.
2. Use constant-time comparison for credentials (avoid timing attacks).
3. Return the same error message for "user not found" and "wrong password" — don't reveal which one failed.
4. Log failed authentication attempts for monitoring.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- Keep access tokens **short-lived** (15–60 minutes)
- Always validate **issuer**, **audience**, and **signature**
- Implement **refresh token rotation** for better security
- **Rate limit** authentication endpoints
- Return **generic error messages** — don't leak information about valid usernames
