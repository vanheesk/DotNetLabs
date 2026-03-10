using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----- Exercise 1: Configure JWT Authentication -----
// TODO: Add authentication services
// builder.Services.AddAuthentication("Bearer")
//     .AddJwtBearer(options =>
//     {
//         var key = builder.Configuration["Jwt:Key"]!;
//         options.TokenValidationParameters = new()
//         {
//             ValidateIssuer = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidateAudience = true,
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
//             ValidateLifetime = true
//         };
//     });

// ----- Exercise 4: Policy-Based Authorisation -----
// TODO: Add authorisation with an AdminOnly policy
// builder.Services.AddAuthorizationBuilder()
//     .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// TODO: Add authentication and authorisation middleware
// app.UseAuthentication();
// app.UseAuthorization();

// ----- Exercise 2: Dev Token Endpoint -----
// TODO: Create POST "/auth/token" that accepts a LoginRequest and returns a JWT
//       Use System.IdentityModel.Tokens.Jwt to create the token

// ----- Exercise 3: Protected Endpoint -----
// TODO: Create GET "/secure" with .RequireAuthorization()
//       Return the user's claims from context.User

// ----- Exercise 4: Admin Endpoint -----
// TODO: Create GET "/admin" with .RequireAuthorization("AdminOnly")

app.MapGet("/", () => "Lab 6: Authentication & Authorisation");

app.Run();

// ----- Types -----
// public record LoginRequest(string Username, string Role);
