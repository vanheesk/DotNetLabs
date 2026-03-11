using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// ✅ SAFE: Strong JWT configuration
var jwtKey = "YourSuperSecretKeyThatIsAtLeast32BytesLong!!";
var issuer = "https://lab-api.example.com";
var audience = "lab-api";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

// ----- In-memory stores -----
var users = new List<UserCredential>
{
    new("alice", "password123"),
    new("bob", "secret456")
};

var refreshTokenStore = new Dictionary<string, RefreshTokenInfo>();

// =====================================================
// EXERCISE 2 & 4: Secure login endpoint
// =====================================================

app.MapPost("/auth/login", (LoginRequest request, ILogger<Program> logger, HttpContext context) =>
{
    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var user = users.FirstOrDefault(u => u.Username == request.Username);

    // ✅ SAFE: Constant-time comparison + generic error message
    var passwordBytes = Encoding.UTF8.GetBytes(request.Password);
    var storedBytes = user is not null
        ? Encoding.UTF8.GetBytes(user.Password)
        : Encoding.UTF8.GetBytes("dummy-password-for-timing");

    if (user is null || !CryptographicOperations.FixedTimeEquals(passwordBytes, storedBytes))
    {
        logger.LogWarning("Failed login attempt for {Username} from {IP}", request.Username, ip);
        // Generic message — don't reveal whether username or password was wrong
        return Results.Json(new { error = "Invalid credentials" }, statusCode: 401);
    }

    logger.LogInformation("Successful login for {Username} from {IP}", user.Username, ip);

    // ✅ SAFE: Short-lived access token (15 minutes)
    var accessToken = GenerateAccessToken(user.Username);

    // Exercise 3: Issue refresh token
    var refreshToken = Guid.NewGuid().ToString();
    refreshTokenStore[refreshToken] = new RefreshTokenInfo(user.Username, DateTime.UtcNow.AddDays(7));

    return Results.Ok(new { accessToken, refreshToken, expiresIn = 900 });
})
.WithName("Login")
.WithSummary("Login (SAFE — secure configuration)")
.WithTags("Auth");

// =====================================================
// EXERCISE 3: Refresh token endpoint
// =====================================================

app.MapPost("/auth/refresh", (RefreshRequest request) =>
{
    if (!refreshTokenStore.TryGetValue(request.RefreshToken, out var tokenInfo))
        return Results.Json(new { error = "Invalid refresh token" }, statusCode: 401);

    if (tokenInfo.ExpiresAt < DateTime.UtcNow)
    {
        refreshTokenStore.Remove(request.RefreshToken);
        return Results.Json(new { error = "Refresh token expired" }, statusCode: 401);
    }

    // ✅ Token rotation: invalidate old refresh token
    refreshTokenStore.Remove(request.RefreshToken);

    // Issue new tokens
    var newRefreshToken = Guid.NewGuid().ToString();
    refreshTokenStore[newRefreshToken] = new RefreshTokenInfo(tokenInfo.UserId, DateTime.UtcNow.AddDays(7));

    var accessToken = GenerateAccessToken(tokenInfo.UserId);
    return Results.Ok(new { accessToken, refreshToken = newRefreshToken, expiresIn = 900 });
})
.WithName("RefreshToken")
.WithSummary("Refresh access token (with rotation)")
.WithTags("Auth");

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

// ----- Helper -----
string GenerateAccessToken(string username)
{
    var claims = new[]
    {
        new Claim("sub", username),
        new Claim("name", username),
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(15), // ✅ Short-lived
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// ----- Types -----
public record UserCredential(string Username, string Password);
public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);
public record RefreshTokenInfo(string UserId, DateTime ExpiresAt);
