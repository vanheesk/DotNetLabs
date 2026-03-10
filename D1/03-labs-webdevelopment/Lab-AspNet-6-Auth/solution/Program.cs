using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----- Exercise 1: JWT Authentication -----
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true
        };
    });

// ----- Exercise 4: Policy-Based Authorisation -----
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// ----- Exercise 2: Dev Token Endpoint -----
app.MapPost("/auth/token", (LoginRequest request) =>
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, request.Username),
        new Claim(JwtRegisteredClaimNames.Name, request.Username),
        new Claim(ClaimTypes.Role, request.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds);

    return Results.Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token),
        expires = token.ValidTo
    });
}).WithTags("Auth");

// ----- Exercise 3: Protected Endpoint -----
app.MapGet("/secure", (ClaimsPrincipal user) =>
{
    var claims = user.Claims.Select(c => new { c.Type, c.Value });
    return Results.Ok(new
    {
        message = $"Hello, {user.Identity?.Name}!",
        claims
    });
}).RequireAuthorization()
  .WithTags("Secure");

// ----- Exercise 4: Admin Endpoint -----
app.MapGet("/admin", (ClaimsPrincipal user) =>
    Results.Ok(new { message = $"Welcome Admin {user.Identity?.Name}!" }))
    .RequireAuthorization("AdminOnly")
    .WithTags("Admin");

app.MapGet("/", () => "Lab 6: Authentication & Authorisation");

app.Run();

// ----- Types -----
public record LoginRequest(string Username, string Role);
