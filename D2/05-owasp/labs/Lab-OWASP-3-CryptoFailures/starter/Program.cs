var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// TODO (Exercise 2): Register PasswordHasher<User>
// builder.Services.AddSingleton<PasswordHasher<User>>();

// TODO (Exercise 3): Register Data Protection
// builder.Services.AddDataProtection();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// TODO (Exercise 4): Add HTTPS redirection and HSTS
// if (!app.Environment.IsDevelopment()) app.UseHsts();
// app.UseHttpsRedirection();

// ----- In-memory user store -----
var users = new List<User>();

// =====================================================
// EXERCISE 1: Vulnerable registration — plain text passwords
// =====================================================

app.MapPost("/auth/register", (RegisterRequest request) =>
{
    if (users.Any(u => u.Username == request.Username))
        return Results.Conflict("User already exists");

    // ⚠️ VULNERABLE: Storing password in plain text
    var user = new User(request.Username, request.Password);
    users.Add(user);

    return Results.Created($"/users/{request.Username}", new { request.Username });
})
.WithName("Register")
.WithSummary("Register a new user (VULNERABLE — plain text passwords)")
.WithTags("Auth");

app.MapPost("/auth/login", (LoginRequest request) =>
{
    var user = users.FirstOrDefault(u => u.Username == request.Username);
    if (user is null)
        return Results.Unauthorized();

    // ⚠️ VULNERABLE: Comparing plain text passwords
    if (user.PasswordHash != request.Password)
        return Results.Unauthorized();

    return Results.Ok(new { message = "Login successful", user = user.Username });
})
.WithName("Login")
.WithSummary("Login (VULNERABLE — plain text comparison)")
.WithTags("Auth");

// Debug endpoint to show stored data (demonstrates the vulnerability)
app.MapGet("/admin/users-debug", () => users)
    .WithName("DebugUsers")
    .WithSummary("Show stored users (demonstrates plain text storage)")
    .WithTags("Debug");

// =====================================================
// EXERCISE 2: TODO — Fix with password hashing
// =====================================================

// TODO: Create POST /auth/register/safe that hashes the password
// TODO: Create POST /auth/login/safe that verifies the hash

// =====================================================
// EXERCISE 3: TODO — Data Protection API
// =====================================================

// TODO: Create GET /protect?value=secret that encrypts the value
// TODO: Create GET /unprotect?value=encrypted that decrypts the value

app.MapGet("/", () => "Lab OWASP-3: Cryptographic Failures (A02:2021)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record User(string Username, string PasswordHash);
public record RegisterRequest(string Username, string Password);
public record LoginRequest(string Username, string Password);
