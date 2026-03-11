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
// EXERCISE 2: Safe endpoints using DTOs
// =====================================================

// ✅ SAFE: Input DTO only accepts allowed fields
app.MapPost("/api/users", (CreateUserRequest request) =>
{
    var user = new UserEntity
    {
        Id = nextId++,
        Name = request.Name,
        Email = request.Email,
        Role = "user",           // Set by server, not by client
        Salary = 0,              // Set by server, not by client
        CreatedAt = DateTime.UtcNow
    };
    users.Add(user);

    return Results.Created($"/api/users/{user.Id}", ToResponse(user));
})
.WithName("CreateUser")
.WithSummary("Create a user (SAFE — input DTO)")
.Produces<UserResponse>(201)
.WithTags("Users");

// ✅ SAFE: Update DTO only allows Name and Email changes
app.MapPut("/api/users/{id:int}", (int id, UpdateUserRequest request) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    // Only map allowed fields
    user.Name = request.Name;
    user.Email = request.Email;
    // Role, Salary, InternalNotes are NOT updated from input

    return Results.Ok(ToResponse(user));
})
.WithName("UpdateUser")
.WithSummary("Update a user (SAFE — input DTO)")
.Produces<UserResponse>()
.WithTags("Users");

// =====================================================
// EXERCISE 3: Safe responses using output DTOs
// =====================================================

// ✅ SAFE: Returns only public fields
app.MapGet("/api/users", () => users.Select(ToResponse))
    .WithName("GetUsers")
    .WithSummary("Get all users (SAFE — response DTO)")
    .Produces<IEnumerable<UserResponse>>()
    .WithTags("Users");

app.MapGet("/api/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    return Results.Ok(ToResponse(user));
})
.WithName("GetUser")
.WithSummary("Get a user (SAFE — response DTO)")
.Produces<UserResponse>()
.WithTags("Users");

// ✅ Admin endpoint returns more details
app.MapGet("/api/admin/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    return Results.Ok(ToAdminResponse(user));
})
.WithName("GetUserAdmin")
.WithSummary("Get user details for admin (includes salary, notes)")
.Produces<UserAdminResponse>()
.WithTags("Admin");

// =====================================================
// Vulnerable endpoints (for comparison)
// =====================================================

app.MapPost("/api/users-vulnerable", (UserEntity user) =>
{
    user.Id = nextId++;
    user.CreatedAt = DateTime.UtcNow;
    if (string.IsNullOrEmpty(user.Role)) user.Role = "user";
    users.Add(user);
    return Results.Created($"/api/users/{user.Id}", user); // Returns full entity
})
.WithName("CreateUserVulnerable")
.WithSummary("VULNERABLE — mass assignment + excessive exposure")
.WithTags("Vulnerable");

app.MapGet("/", () => "Lab OWASP-API-3: Mass Assignment & Data Exposure (API3:2023)")
    .ExcludeFromDescription();

app.Run();

// ----- Mapping helpers -----
static UserResponse ToResponse(UserEntity u) =>
    new(u.Id, u.Name, u.Email, u.Role);

static UserAdminResponse ToAdminResponse(UserEntity u) =>
    new(u.Id, u.Name, u.Email, u.Role, u.Salary, u.InternalNotes, u.CreatedAt);

// ----- Input DTOs (Exercise 2) -----
public record CreateUserRequest(string Name, string Email);
public record UpdateUserRequest(string Name, string Email);

// ----- Output DTOs (Exercise 3) -----
public record UserResponse(int Id, string Name, string Email, string Role);
public record UserAdminResponse(int Id, string Name, string Email, string Role, decimal Salary, string? InternalNotes, DateTime CreatedAt);

// ----- Domain Model (internal — never exposed directly) -----
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
