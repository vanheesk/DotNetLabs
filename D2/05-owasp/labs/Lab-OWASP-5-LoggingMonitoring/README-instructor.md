# Lab 5: Security Logging & Monitoring Failures (OWASP A09:2021) — Instructor Guide

## Teaching Notes

This lab addresses a critical gap in most applications. Many developers log errors but forget to log security events. Without security logging, organisations have no visibility into attacks.

### Key Points to Emphasise

1. **Logging ≠ security logging** — application errors vs. security events are different
2. **Structured logging** is essential — log templates with parameters, not string interpolation
3. **Never log secrets** — passwords, tokens, connection strings must be redacted
4. **IP tracking helps** but is not definitive — proxies, VPNs, shared IPs
5. **Real-time detection** turns logs from passive records into active defence

### Demo Flow

1. Show the app running with no security logging — attacks are invisible
2. Add login logging and show how attacks become visible
3. Create the audit trail and demonstrate forensic capability
4. Implement brute force detection and show it triggering

---

## Exercise 1 – Solution

```csharp
app.MapPost("/auth/login", (LoginRequest request, HttpContext context, ILogger<Program> logger) =>
{
    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var user = users.FirstOrDefault(u => u.Username == request.Username);

    if (user is null || user.Password != request.Password)
    {
        logger.LogWarning("Failed login attempt for {Username} from {IP} at {Timestamp}",
            request.Username, ip, DateTime.UtcNow);
        return Results.Unauthorized();
    }

    logger.LogInformation("User {Username} logged in successfully from {IP} at {Timestamp}",
        user.Username, ip, DateTime.UtcNow);
    return Results.Ok(new { message = "Login successful" });
});
```

> **Teaching note:** Emphasise that `LogWarning` with named parameters creates structured log entries that can be queried, unlike `$"Failed: {username}"` which is just a flat string.

---

## Exercise 2 – Solution

```csharp
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
```

---

## Exercise 3 – Solution

```csharp
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
            return true; // Suspicious
        }

        return false;
    }
}
```

---

## Exercise 4 – Solution

```csharp
app.Use(async (context, next) =>
{
    await next();

    var endpoint = context.GetEndpoint();
    var tags = endpoint?.Metadata.GetOrderedMetadata<ITagsMetadata>();
    if (tags?.Any(t => t.Tags.Contains("Sensitive")) == true)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var user = context.User.Identity?.Name ?? "anonymous";
        logger.LogInformation(
            "SENSITIVE_ACCESS: {Method} {Path} by {User} — Status: {StatusCode}",
            context.Request.Method, context.Request.Path, user, context.Response.StatusCode);
    }
});
```

---

## Common Student Issues

1. **Using string interpolation instead of log templates** — `$"User {name}"` vs `"User {Name}"` with parameter
2. **Logging passwords** — remind students to NEVER log credentials
3. **Thread safety of the audit service** — use `ConcurrentBag` or locking in production
4. **IP address is null in development** — `localhost` may show `::1` or `null`
