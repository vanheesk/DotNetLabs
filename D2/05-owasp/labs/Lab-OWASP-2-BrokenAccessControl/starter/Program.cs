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

builder.Services.AddAuthorization();

// TODO (Exercise 3): Add AdminOnly policy
// builder.Services.AddAuthorizationBuilder()
//     .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));

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
// EXERCISE 1 & 2: IDOR Vulnerability
// =====================================================

// ⚠️ VULNERABLE: No ownership check — any authenticated user can access any user's orders
app.MapGet("/users/{userId}/orders", (string userId) =>
{
    // TODO (Exercise 2): Add ownership check
    // Get the current user from HttpContext.User.FindFirst("sub")
    // Compare with the userId parameter
    // Return Results.Forbid() if they don't match

    var userOrders = orders.Where(o => o.UserId == userId).ToList();
    return Results.Ok(userOrders);
})
.RequireAuthorization()
.WithName("GetUserOrders")
.WithSummary("Get orders for a user (VULNERABLE — no ownership check)")
.WithTags("Orders");

// =====================================================
// EXERCISE 3: Missing Function-Level Access Control
// =====================================================

// ⚠️ VULNERABLE: Admin endpoint with no authorisation — anyone can access it
app.MapGet("/admin/users", () => users)
// TODO (Exercise 3): Add .RequireAuthorization("AdminOnly")
.WithName("GetAllUsers")
.WithSummary("List all users (VULNERABLE — no admin check)")
.WithTags("Admin");

// =====================================================
// EXERCISE 4: Audit Log
// =====================================================

// TODO (Exercise 4): Create an AuditLog service, register it as singleton
// TODO (Exercise 4): Create a GET /admin/audit-log endpoint (admin only)
// TODO (Exercise 4): Create an OwnershipFilter that logs denied attempts

app.MapGet("/", () => "Lab OWASP-2: Broken Access Control (A01:2021)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record User(string Id, string Name, string Role);
public record Order(int Id, string UserId, string Product, decimal Price);
public record LoginRequest(string Username, string? Role);
