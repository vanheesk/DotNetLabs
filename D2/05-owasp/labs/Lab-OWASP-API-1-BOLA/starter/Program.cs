using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

// ----- In-memory data -----
var records = new List<MedicalRecord>
{
    new(1, "patient-1", "Annual checkup — all clear", "2025-01-15"),
    new(2, "patient-1", "Blood work — cholesterol elevated", "2025-03-20"),
    new(3, "patient-2", "X-ray — fractured wrist", "2025-02-10"),
    new(4, "patient-2", "Follow-up — healing well", "2025-04-05"),
    new(5, "patient-2", "Prescription renewal — medication A", "2025-05-01"),
};

// ----- Token endpoint -----
app.MapPost("/auth/token", (TokenRequest request) =>
{
    var claims = new[]
    {
        new Claim("sub", request.UserId),
        new Claim("name", request.UserId),
        new Claim(ClaimTypes.Role, request.Role ?? "patient")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
})
.WithName("GetToken")
.WithSummary("Get a JWT token (use patient-1, patient-2, or doctor-1)")
.WithTags("Auth");

// =====================================================
// EXERCISE 1 & 2: Patient records — BOLA vulnerability
// =====================================================

// ⚠️ VULNERABLE: No ownership check — any authenticated user can access any patient's records
app.MapGet("/api/patients/{patientId}/records", (string patientId) =>
{
    // TODO (Exercise 2): Add ownership check
    // Get the current user from HttpContext
    // Compare with patientId
    // Allow if owner OR if user has "doctor" role

    var patientRecords = records.Where(r => r.PatientId == patientId).ToList();
    if (!patientRecords.Any())
        return Results.NotFound();

    return Results.Ok(patientRecords);
})
.RequireAuthorization()
.WithName("GetPatientRecords")
.WithSummary("Get all records for a patient (VULNERABLE — no ownership check)")
.WithTags("Records");

app.MapGet("/api/patients/{patientId}/records/{recordId:int}", (string patientId, int recordId) =>
{
    // TODO (Exercise 2): Add ownership check

    var record = records.FirstOrDefault(r => r.PatientId == patientId && r.Id == recordId);
    return record is not null ? Results.Ok(record) : Results.NotFound();
})
.RequireAuthorization()
.WithName("GetPatientRecord")
.WithSummary("Get a specific record (VULNERABLE — no ownership check)")
.WithTags("Records");

app.MapGet("/", () => "Lab OWASP-API-1: Broken Object Level Authorization (API1:2023)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record MedicalRecord(int Id, string PatientId, string Description, string Date);
public record TokenRequest(string UserId, string? Role);
