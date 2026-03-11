var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ----- In-memory store -----
var nextId = 1;
var users = new List<UserEntity>();

// =====================================================
// EXERCISE 1: Mass Assignment vulnerability
// =====================================================

// ⚠️ VULNERABLE: Binds directly to the domain model — attacker can set Role, Salary, etc.
app.MapPost("/api/users", (UserEntity user) =>
{
    user.Id = nextId++;
    user.CreatedAt = DateTime.UtcNow;
    if (string.IsNullOrEmpty(user.Role)) user.Role = "user";
    users.Add(user);

    return Results.Created($"/api/users/{user.Id}", user);
})
.WithName("CreateUser")
.WithSummary("Create a user (VULNERABLE — mass assignment)")
.WithTags("Users");

app.MapPut("/api/users/{id:int}", (int id, UserEntity update) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    // ⚠️ VULNERABLE: Updates ALL fields from client input
    user.Name = update.Name;
    user.Email = update.Email;
    user.Role = update.Role;         // ⚠️ Attacker can escalate to admin!
    user.Salary = update.Salary;     // ⚠️ Attacker can change salary!

    return Results.Ok(user);
})
.WithName("UpdateUser")
.WithSummary("Update a user (VULNERABLE — mass assignment)")
.WithTags("Users");

// =====================================================
// EXERCISE 3: Excessive Data Exposure
// =====================================================

// ⚠️ VULNERABLE: Returns full domain model including sensitive fields
app.MapGet("/api/users", () => users)
    .WithName("GetUsers")
    .WithSummary("Get all users (VULNERABLE — exposes sensitive fields)")
    .WithTags("Users");

app.MapGet("/api/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
})
.WithName("GetUser")
.WithSummary("Get a user (VULNERABLE — exposes sensitive fields)")
.WithTags("Users");

// TODO (Exercise 2): Create input DTOs — CreateUserRequest, UpdateUserRequest
// TODO (Exercise 2): Create new endpoints that use DTOs instead of domain model

// TODO (Exercise 3): Create response DTOs — UserResponse, UserAdminResponse
// TODO (Exercise 3): Return appropriate DTO instead of full entity

app.MapGet("/", () => "Lab OWASP-API-3: Mass Assignment & Data Exposure (API3:2023)")
    .ExcludeFromDescription();

app.Run();

// ----- Domain Model (internal) -----
public class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "user";
    public decimal Salary { get; set; }
    public string? PasswordHash { get; set; }
    public string? InternalNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}
