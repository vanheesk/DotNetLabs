using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// ⚠️ VULNERABLE: Weak JWT configuration
var jwtKey = "short-key"; // Too short!
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,       // ⚠️ Not validating issuer
            ValidateAudience = false,     // ⚠️ Not validating audience
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

// ----- In-memory users -----
var users = new List<UserCredential>
{
    new("alice", "password123"),
    new("bob", "secret456")
};

// =====================================================
// EXERCISE 1 & 4: Login endpoint
// =====================================================

app.MapPost("/auth/login", (LoginRequest request) =>
{
    var user = users.FirstOrDefault(u => u.Username == request.Username);

    // ⚠️ VULNERABLE: Reveals whether username exists vs wrong password
    if (user is null)
        return Results.BadRequest("User not found");

    if (user.Password != request.Password)
        return Results.BadRequest("Wrong password");

    // ⚠️ VULNERABLE: Token expires in 30 days
    var claims = new[]
    {
        new Claim("sub", user.Username),
        new Claim("name", user.Username),
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddDays(30), // ⚠️ Too long!
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
})
.WithName("Login")
.WithSummary("Login (VULNERABLE — multiple issues)")
.WithTags("Auth");

// TODO (Exercise 2): Fix JWT configuration
//   - Validate issuer and audience
//   - Use a strong signing key (32+ bytes)
//   - Set short expiration (15 minutes)

// TODO (Exercise 3): Create POST /auth/refresh endpoint
//   - Accept a refresh token
//   - Validate and rotate it
//   - Return new access + refresh tokens

// TODO (Exercise 4): Fix login endpoint
//   - Use generic error messages
//   - Add rate limiting
//   - Use constant-time comparison

// =====================================================
// Protected endpoint
// =====================================================

app.MapGet("/api/profile", (HttpContext context) =>
{
    var username = context.User.FindFirst("sub")?.Value;
    return Results.Ok(new { username, message = "You are authenticated!" });
})
.RequireAuthorization()
.WithName("GetProfile")
.WithSummary("Get your profile (requires auth)")
.WithTags("API");

app.MapGet("/", () => "Lab OWASP-API-2: Broken Authentication (API2:2023)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record UserCredential(string Username, string Password);
public record LoginRequest(string Username, string Password);
