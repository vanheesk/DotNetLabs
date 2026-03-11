var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// Exercise 2: Audit service
builder.Services.AddSingleton<AuditService>();

// Exercise 3: Suspicious activity detector
builder.Services.AddSingleton<SuspiciousActivityDetector>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();

// Exercise 4: Middleware to log sensitive data access
app.Use(async (context, next) =>
{
    await next();

    var endpoint = context.GetEndpoint();
    if (endpoint?.Metadata.GetMetadata<SensitiveAttribute>() is not null)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var user = context.User.Identity?.Name ?? "anonymous";
        logger.LogInformation(
            "SENSITIVE_ACCESS: {Method} {Path} by {User} — Status: {StatusCode}",
            context.Request.Method, context.Request.Path, user, context.Response.StatusCode);

        var auditService = context.RequestServices.GetRequiredService<AuditService>();
        auditService.Record("DATA_ACCESS", user, context.Request.Path,
            context.Connection.RemoteIpAddress?.ToString());
    }
});

// ----- In-memory user store -----
var users = new List<UserAccount>
{
    new("alice", "password123", "user"),
    new("bob", "secret456", "user"),
    new("admin1", "adminpass", "admin")
};

// =====================================================
// EXERCISE 1 & 3: Login with security logging and brute force detection
// =====================================================

app.MapPost("/auth/login", (
    LoginRequest request,
    HttpContext context,
    ILogger<Program> logger,
    AuditService auditService,
    SuspiciousActivityDetector detector) =>
{
    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var user = users.FirstOrDefault(u => u.Username == request.Username);

    if (user is null || user.Password != request.Password)
    {
        // Exercise 1: Log failed attempt
        logger.LogWarning("Failed login attempt for {Username} from {IP} at {Timestamp}",
            request.Username, ip, DateTime.UtcNow);

        // Exercise 2: Record in audit trail
        auditService.Record("LOGIN_FAILED", request.Username, "/auth/login", ip);

        // Exercise 3: Check for brute force
        if (detector.RecordFailedAttempt(ip))
        {
            return Results.Problem("Too many failed attempts. Please try again later.", statusCode: 429);
        }

        return Results.Unauthorized();
    }

    // Exercise 1: Log successful login
    logger.LogInformation("User {Username} logged in successfully from {IP} at {Timestamp}",
        user.Username, ip, DateTime.UtcNow);

    // Exercise 2: Record in audit trail
    auditService.Record("LOGIN_SUCCESS", user.Username, "/auth/login", ip);

    return Results.Ok(new { message = "Login successful", user = user.Username });
})
.WithName("Login")
.WithSummary("Login with security logging and brute force detection")
.WithTags("Auth");

// =====================================================
// Sensitive data endpoint with logging
// =====================================================

app.MapGet("/users/{username}/profile", (string username) =>
{
    var user = users.FirstOrDefault(u => u.Username == username);
    if (user is null)
        return Results.NotFound();

    return Results.Ok(new { user.Username, user.Role, Email = $"{user.Username}@example.com" });
})
.WithMetadata(new SensitiveAttribute())
.WithName("GetProfile")
.WithSummary("Get user profile (logged as sensitive access)")
.WithTags("Users", "Sensitive");

// =====================================================
// EXERCISE 2: Audit trail endpoint
// =====================================================

app.MapGet("/admin/audit", (AuditService auditService) => auditService.GetEntries())
    .WithName("GetAuditTrail")
    .WithSummary("View the security audit trail")
    .WithTags("Admin");

app.MapGet("/", () => "Lab OWASP-5: Security Logging & Monitoring (A09:2021)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record UserAccount(string Username, string Password, string Role);
public record LoginRequest(string Username, string Password);

// ----- Marker attribute for sensitive endpoints -----
[AttributeUsage(AttributeTargets.Method)]
public class SensitiveAttribute : Attribute { }

// ----- Exercise 2: Audit Service -----
public class AuditService
{
    private readonly List<AuditEntry> _entries = [];
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger) => _logger = logger;

    public void Record(string eventType, string? user, string? resource, string? ip)
    {
        var entry = new AuditEntry(DateTime.UtcNow, eventType, user, resource, ip);
        _entries.Add(entry);
        _logger.LogInformation("AUDIT: {EventType} by {User} on {Resource} from {IP}",
            eventType, user ?? "anonymous", resource ?? "-", ip ?? "unknown");
    }

    public IReadOnlyList<AuditEntry> GetEntries() => _entries.AsReadOnly();
}

public record AuditEntry(DateTime Timestamp, string EventType, string? User, string? Resource, string? IpAddress);

// ----- Exercise 3: Suspicious Activity Detector -----
public class SuspiciousActivityDetector
{
    private readonly Dictionary<string, List<DateTime>> _failedAttempts = [];
    private readonly ILogger<SuspiciousActivityDetector> _logger;
    private const int MaxAttempts = 5;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(5);

    public SuspiciousActivityDetector(ILogger<SuspiciousActivityDetector> logger) => _logger = logger;

    public bool RecordFailedAttempt(string ip)
    {
        if (!_failedAttempts.ContainsKey(ip))
            _failedAttempts[ip] = [];

        var attempts = _failedAttempts[ip];
        attempts.Add(DateTime.UtcNow);

        // Remove old attempts outside the window
        attempts.RemoveAll(t => t < DateTime.UtcNow - Window);

        if (attempts.Count >= MaxAttempts)
        {
            _logger.LogCritical(
                "ALERT: Possible brute force attack from {IP} — {Count} failed attempts in {Window} minutes",
                ip, attempts.Count, Window.TotalMinutes);
            return true;
        }

        return false;
    }
}
