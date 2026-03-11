# Lab 3: Cryptographic Failures (OWASP A02:2021)

## Objective

Understand **Cryptographic Failures** by exploring insecure password storage, learning proper password hashing, using the ASP.NET Core Data Protection API, and enforcing HTTPS.

---

## Background

**Cryptographic Failures** (formerly "Sensitive Data Exposure") is the **#2 risk** in the OWASP Top 10 (2021). It covers failures related to cryptography that often lead to exposure of sensitive data. Common issues include:

- Storing passwords in **plain text** or using weak hashing
- Using **outdated algorithms** (MD5, SHA1 for passwords)
- Transmitting data over **unencrypted channels**
- Missing **encryption at rest** for sensitive data

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Identify Plain Text Password Storage

The starter project stores user passwords in **plain text** — a critical vulnerability.

### Tasks

1. Run the starter project and navigate to `/swagger`.
2. Register a new user via `POST /auth/register` with a username and password.
3. Access `GET /admin/users-debug` to see stored user data.
4. Observe that the password is stored in **plain text** — anyone with database access can read it.
5. Understand why this is catastrophic (data breaches, credential reuse, etc.).

---

## Exercise 2 – Fix with Password Hashing

### Tasks

1. Use `Microsoft.AspNetCore.Identity.PasswordHasher<T>` to hash passwords.
2. Register the `PasswordHasher<User>` in the DI container.
3. Modify the registration endpoint to **hash** the password before storing.
4. Modify the login endpoint to **verify** the hash instead of comparing plain text.
5. Verify that stored passwords are no longer readable.

> **Hint:**
> ```csharp
> // Hash during registration
> var hash = passwordHasher.HashPassword(user, request.Password);
>
> // Verify during login
> var result = passwordHasher.VerifyHashedPassword(user, storedHash, inputPassword);
> if (result == PasswordVerificationResult.Success) { ... }
> ```

---

## Exercise 3 – Protect Sensitive Data with Data Protection API

ASP.NET Core's **Data Protection API** provides encryption for sensitive data like tokens, cookies, and personal information.

### Tasks

1. Register Data Protection: `builder.Services.AddDataProtection()`.
2. Create a `GET /protect` endpoint that accepts a `value` query parameter and returns an encrypted version.
3. Create a `GET /unprotect` endpoint that accepts an encrypted string and returns the decrypted value.
4. Use `IDataProtectionProvider` to create a protector with a purpose string.
5. Verify that the encrypted value is opaque and cannot be read without the correct key.

> **Hint:**
> ```csharp
> var protector = dataProtection.CreateProtector("MyPurpose");
> var encrypted = protector.Protect(value);
> var decrypted = protector.Unprotect(encrypted);
> ```

---

## Exercise 4 – Enforce HTTPS

### Tasks

1. Add HTTPS redirection middleware: `app.UseHttpsRedirection()`.
2. Add HSTS (HTTP Strict Transport Security) for production:
   ```csharp
   if (!app.Environment.IsDevelopment())
       app.UseHsts();
   ```
3. Test that HTTP requests are redirected to HTTPS.
4. Examine the `Strict-Transport-Security` response header.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- **Never** store passwords in plain text
- Use **proven hashing algorithms** (PBKDF2, bcrypt, Argon2)
- Use the **Data Protection API** for encrypting sensitive data at rest
- **Enforce HTTPS** for all traffic in production
- **HSTS** prevents protocol downgrade attacks
