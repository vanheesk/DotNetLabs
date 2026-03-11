using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// ----- JWT Authentication Setup -----
var jwtKey = "SuperSecretKeyForLabPurposesOnly!AtLeast32Chars";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Exercise 3: AdminOnly policy
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));

// Exercise 4: Audit log service
builder.Services.AddSingleton<AuditLog>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

// ----- In-memory data -----
var users = new List<User>
{
    new("alice", "Alice Smith", "user"),
    new("bob", "Bob Jones", "user"),
    new("admin1", "Admin User", "admin")
};

var orders = new List<Order>
{
    new(1, "alice", "Laptop", 999.99m),
    new(2, "alice", "Mouse", 29.99m),
    new(3, "bob", "Keyboard", 79.99m),
    new(4, "bob", "Monitor", 449.00m),
    new(5, "bob", "Headset", 89.99m)
};

// ----- Token endpoint (for testing) -----
app.MapPost("/auth/token", (LoginRequest request) =>
{
    var user = users.FirstOrDefault(u => u.Id == request.Username);
    if (user is null)
        return Results.NotFound("User not found");

    var claims = new[]
    {
        new Claim("sub", user.Id),
        new Claim("name", user.Name),
        new Claim(ClaimTypes.Role, request.Role ?? user.Role)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
})
.WithName("GetToken")
.WithSummary("Get a JWT token for testing")
.WithTags("Auth");

// =====================================================
// EXERCISE 2: Fixed IDOR — with ownership check
// =====================================================

app.MapGet("/users/{userId}/orders", (string userId, HttpContext context, AuditLog auditLog) =>
{
    // ✅ SAFE: Verify the authenticated user matches the requested userId
    var currentUser = context.User.FindFirst("sub")?.Value;
    if (currentUser != userId)
    {
        auditLog.RecordDenied(currentUser ?? "anonymous", $"/users/{userId}/orders");
        return Results.Forbid();
    }

    var userOrders = orders.Where(o => o.UserId == userId).ToList();
    return Results.Ok(userOrders);
})
.RequireAuthorization()
.WithName("GetUserOrders")
.WithSummary("Get orders for a user (SAFE — with ownership check)")
.WithTags("Orders");

// =====================================================
// EXERCISE 3: Fixed admin endpoint — with authorisation policy
// =====================================================

app.MapGet("/admin/users", () => users)
    .RequireAuthorization("AdminOnly")
    .WithName("GetAllUsers")
    .WithSummary("List all users (SAFE — admin only)")
    .WithTags("Admin");

// =====================================================
// EXERCISE 4: Audit log endpoint
// =====================================================

app.MapGet("/admin/audit-log", (AuditLog auditLog) => auditLog.GetEntries())
    .RequireAuthorization("AdminOnly")
    .WithName("GetAuditLog")
    .WithSummary("View access denial audit log (admin only)")
    .WithTags("Admin");

app.MapGet("/", () => "Lab OWASP-2: Broken Access Control (A01:2021)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record User(string Id, string Name, string Role);
public record Order(int Id, string UserId, string Product, decimal Price);
public record LoginRequest(string Username, string? Role);

// ----- Audit Log Service -----
public class AuditLog
{
    private readonly List<AuditEntry> _entries = [];

    public void RecordDenied(string user, string resource)
    {
        _entries.Add(new AuditEntry(DateTime.UtcNow, user, resource, "ACCESS_DENIED"));
    }

    public IReadOnlyList<AuditEntry> GetEntries() => _entries.AsReadOnly();
}

public record AuditEntry(DateTime Timestamp, string User, string Resource, string Action);
