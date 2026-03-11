# Lab 3: Cryptographic Failures (OWASP A02:2021) — Instructor Guide

## Teaching Notes

This is the **#2 risk** in the OWASP Top 10. Many breaches stem from poor cryptographic practices. Students often underestimate the importance of proper password hashing.

### Key Points to Emphasise

1. **Plain text passwords = instant compromise** when the database leaks
2. **MD5/SHA1 are NOT password hashing algorithms** — they're message digests
3. **PBKDF2/bcrypt/Argon2** add salt and key stretching automatically
4. **ASP.NET Core's PasswordHasher** uses PBKDF2 with HMAC-SHA256 by default
5. **Data Protection API** handles key management, rotation, and encryption
6. **HTTPS is mandatory** for all production traffic

### Demo Flow

1. Show plain text storage and explain the impact of a database breach
2. Switch to password hashing and show the hash output is opaque
3. Demo Data Protection API for encrypting sensitive fields
4. Show HTTPS redirect and HSTS headers

---

## Exercise 1 – Solution

The vulnerable code stores passwords directly:

```csharp
var user = new User(request.Username, request.Password); // Plain text!
users.Add(user);
```

This means if the data store is compromised, all passwords are immediately exposed.

---

## Exercise 2 – Solution

```csharp
// Registration
builder.Services.AddSingleton<PasswordHasher<User>>();

app.MapPost("/auth/register", (RegisterRequest request, PasswordHasher<User> hasher) =>
{
    if (users.Any(u => u.Username == request.Username))
        return Results.Conflict("User already exists");

    var user = new User(request.Username, "");
    var hash = hasher.HashPassword(user, request.Password);
    users.Add(user with { PasswordHash = hash });

    return Results.Created($"/users/{request.Username}", new { request.Username });
});

// Login
app.MapPost("/auth/login", (LoginRequest request, PasswordHasher<User> hasher) =>
{
    var user = users.FirstOrDefault(u => u.Username == request.Username);
    if (user is null)
        return Results.Unauthorized();

    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
    if (result == PasswordVerificationResult.Failed)
        return Results.Unauthorized();

    return Results.Ok(new { message = "Login successful", user = user.Username });
});
```

> **Teaching note:** Show students the hash output — it contains the algorithm version, iteration count, salt, and hash all encoded together. ASP.NET Core automatically handles salt generation and versioning.

---

## Exercise 3 – Solution

```csharp
builder.Services.AddDataProtection();

app.MapGet("/protect", (string value, IDataProtectionProvider dp) =>
{
    var protector = dp.CreateProtector("LabDemo");
    var encrypted = protector.Protect(value);
    return Results.Ok(new { original = value, encrypted });
});

app.MapGet("/unprotect", (string value, IDataProtectionProvider dp) =>
{
    var protector = dp.CreateProtector("LabDemo");
    try
    {
        var decrypted = protector.Unprotect(value);
        return Results.Ok(new { encrypted = value, decrypted });
    }
    catch (Exception)
    {
        return Results.BadRequest("Failed to decrypt — invalid or tampered data");
    }
});
```

> **Teaching note:** Explain that purpose strings create isolated key spaces. Data protected with purpose "A" cannot be unprotected with purpose "B".

---

## Exercise 4 – Solution

```csharp
if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
```

---

## Common Student Issues

1. **Confusing encryption with hashing** — encryption is reversible, hashing is not
2. **Trying to "decrypt" a password hash** — not possible by design
3. **Using `SHA256.HashData()` for passwords** — missing salt and key stretching
4. **Forgetting to register `AddDataProtection()`** in DI
