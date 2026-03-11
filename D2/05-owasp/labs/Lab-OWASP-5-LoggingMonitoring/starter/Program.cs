var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// TODO (Exercise 2): Register AuditService as singleton
// TODO (Exercise 3): Register SuspiciousActivityDetector as singleton

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();

// TODO (Exercise 4): Add middleware to log access to sensitive endpoints

// ----- In-memory user store -----
var users = new List<UserAccount>
{
    new("alice", "password123", "user"),
    new("bob", "secret456", "user"),
    new("admin1", "adminpass", "admin")
};

// =====================================================
// EXERCISE 1: Login with no security logging
// =====================================================

app.MapPost("/auth/login", (LoginRequest request, HttpContext context) =>
{
    // TODO (Exercise 1): Add structured logging for login attempts
    // TODO (Exercise 3): Add brute force detection

    var user = users.FirstOrDefault(u => u.Username == request.Username);
    if (user is null || user.Password != request.Password)
    {
        // No logging of failed attempt — invisible to monitoring
        return Results.Unauthorized();
    }

    // No logging of successful login — no audit trail
    return Results.Ok(new { message = "Login successful", user = user.Username });
})
.WithName("Login")
.WithSummary("Login (NO security logging)")
.WithTags("Auth");

// =====================================================
// Sensitive data endpoint (Exercise 4)
// =====================================================

app.MapGet("/users/{username}/profile", (string username) =>
{
    var user = users.FirstOrDefault(u => u.Username == username);
    if (user is null)
        return Results.NotFound();

    // TODO (Exercise 4): Tag this endpoint as "Sensitive"
    return Results.Ok(new { user.Username, user.Role, Email = $"{user.Username}@example.com" });
})
.WithName("GetProfile")
.WithSummary("Get user profile (sensitive data)")
.WithTags("Users");

// =====================================================
// EXERCISE 2: TODO — Create audit trail endpoint
// =====================================================

// TODO: Create GET /admin/audit endpoint to view the audit trail

app.MapGet("/", () => "Lab OWASP-5: Security Logging & Monitoring (A09:2021)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record UserAccount(string Username, string Password, string Role);
public record LoginRequest(string Username, string Password);
