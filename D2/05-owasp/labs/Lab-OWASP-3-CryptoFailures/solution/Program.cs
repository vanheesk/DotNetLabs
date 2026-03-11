using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// Exercise 2: Password hasher
builder.Services.AddSingleton<PasswordHasher<User>>();

// Exercise 3: Data Protection
builder.Services.AddDataProtection();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Exercise 4: HTTPS enforcement
if (!app.Environment.IsDevelopment())
    app.UseHsts();
app.UseHttpsRedirection();

// ----- In-memory user store -----
var users = new List<User>();

// =====================================================
// Vulnerable endpoints (for comparison/demonstration)
// =====================================================

app.MapPost("/auth/register/vulnerable", (RegisterRequest request) =>
{
    if (users.Any(u => u.Username == request.Username))
        return Results.Conflict("User already exists");

    // ⚠️ VULNERABLE: Storing password in plain text
    var user = new User(request.Username, request.Password);
    users.Add(user);

    return Results.Created($"/users/{request.Username}", new { request.Username });
})
.WithName("RegisterVulnerable")
.WithSummary("Register — VULNERABLE (plain text)")
.WithTags("Vulnerable");

// =====================================================
// EXERCISE 2: Safe registration with password hashing
// =====================================================

app.MapPost("/auth/register", (RegisterRequest request, PasswordHasher<User> hasher) =>
{
    if (users.Any(u => u.Username == request.Username))
        return Results.Conflict("User already exists");

    // ✅ SAFE: Hash the password before storing
    var user = new User(request.Username, "");
    var hash = hasher.HashPassword(user, request.Password);
    var hashedUser = user with { PasswordHash = hash };
    users.Add(hashedUser);

    return Results.Created($"/users/{request.Username}", new { request.Username });
})
.WithName("RegisterSafe")
.WithSummary("Register — SAFE (hashed password)")
.WithTags("Secure");

app.MapPost("/auth/login", (LoginRequest request, PasswordHasher<User> hasher) =>
{
    var user = users.FirstOrDefault(u => u.Username == request.Username);
    if (user is null)
        return Results.Unauthorized();

    // ✅ SAFE: Verify the hash instead of comparing plain text
    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
    if (result == PasswordVerificationResult.Failed)
        return Results.Unauthorized();

    return Results.Ok(new { message = "Login successful", user = user.Username });
})
.WithName("LoginSafe")
.WithSummary("Login — SAFE (hash verification)")
.WithTags("Secure");

// Debug endpoint to show that passwords are hashed
app.MapGet("/admin/users-debug", () =>
    users.Select(u => new { u.Username, PasswordHashPreview = u.PasswordHash[..Math.Min(20, u.PasswordHash.Length)] + "..." }))
    .WithName("DebugUsers")
    .WithSummary("Show stored users (passwords are hashed)")
    .WithTags("Debug");

// =====================================================
// EXERCISE 3: Data Protection API
// =====================================================

app.MapGet("/protect", (string value, IDataProtectionProvider dp) =>
{
    var protector = dp.CreateProtector("LabDemo");
    var encrypted = protector.Protect(value);
    return Results.Ok(new { original = value, encrypted });
})
.WithName("ProtectData")
.WithSummary("Encrypt a value using Data Protection API")
.WithTags("DataProtection");

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
})
.WithName("UnprotectData")
.WithSummary("Decrypt a value using Data Protection API")
.WithTags("DataProtection");

app.MapGet("/", () => "Lab OWASP-3: Cryptographic Failures (A02:2021)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record User(string Username, string PasswordHash);
public record RegisterRequest(string Username, string Password);
public record LoginRequest(string Username, string Password);
