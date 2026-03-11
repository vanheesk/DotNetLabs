# Lab 7: Broken Authentication (OWASP API2:2023) — Instructor Guide

## Teaching Notes

Broken Authentication in APIs is different from web application auth issues because APIs rely on tokens rather than session cookies. Many developers don't fully understand JWT or configure it incorrectly.

### Key Points to Emphasise

1. **JWTs are not encrypted by default** — they're just Base64-encoded, anyone can read them
2. **Long token lifetimes** are a security risk — stolen tokens remain valid
3. **Issuer/audience validation** prevents cross-service token reuse
4. **Refresh tokens** must be stored server-side and rotated
5. **Generic error messages** prevent username enumeration

### Demo Flow

1. Create a JWT with a 30-day expiry and decode it at jwt.io — show the payload is readable
2. Fix the configuration with proper validation
3. Implement refresh tokens with rotation
4. Demonstrate brute force protection

---

## Exercise 2 – Solution

```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = "https://lab-api.example.com",
    ValidateAudience = true,
    ValidAudience = "lab-api",
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32BytesLong!!")),
    ClockSkew = TimeSpan.FromMinutes(1)  // Reduce default 5 min skew
};
```

---

## Exercise 3 – Solution

```csharp
var refreshTokenStore = new Dictionary<string, RefreshTokenInfo>();

app.MapPost("/auth/refresh", (RefreshRequest request) =>
{
    if (!refreshTokenStore.TryGetValue(request.RefreshToken, out var tokenInfo))
        return Results.Unauthorized();

    if (tokenInfo.ExpiresAt < DateTime.UtcNow)
    {
        refreshTokenStore.Remove(request.RefreshToken);
        return Results.Unauthorized();
    }

    // Rotate: remove old refresh token
    refreshTokenStore.Remove(request.RefreshToken);

    // Issue new tokens
    var newRefreshToken = Guid.NewGuid().ToString();
    refreshTokenStore[newRefreshToken] = new RefreshTokenInfo(tokenInfo.UserId, DateTime.UtcNow.AddDays(7));

    var accessToken = GenerateAccessToken(tokenInfo.UserId);
    return Results.Ok(new { accessToken, refreshToken = newRefreshToken });
});

record RefreshTokenInfo(string UserId, DateTime ExpiresAt);
record RefreshRequest(string RefreshToken);
```

---

## Exercise 4 – Solution

```csharp
app.MapPost("/auth/login", (LoginRequest request) =>
{
    var user = users.FirstOrDefault(u => u.Username == request.Username);

    // Constant-time comparison + generic error message
    if (user is null || !CryptographicOperations.FixedTimeEquals(
        Encoding.UTF8.GetBytes(user.Password),
        Encoding.UTF8.GetBytes(request.Password)))
    {
        // Don't reveal whether the username or password was wrong
        return Results.Unauthorized();
    }

    return Results.Ok(new { token = GenerateAccessToken(user.Username) });
});
```

> **Teaching note:** Explain timing attacks — if you return early for "user not found" vs. checking the password, an attacker can measure response time to determine if a username exists.

---

## Common Student Issues

1. **Confusing access tokens with refresh tokens** — different purposes and lifetimes
2. **Storing refresh tokens in the client only** — they must be validated server-side
3. **Not understanding token rotation** — old tokens must be invalidated
4. **Using `==` instead of constant-time comparison** — subtle but important for security
