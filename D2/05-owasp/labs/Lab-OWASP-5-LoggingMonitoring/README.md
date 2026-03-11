# Lab 5: Security Logging & Monitoring Failures (OWASP A09:2021)

## Objective

Implement proper **security logging and monitoring** to detect, alert on, and respond to security events. Learn to create audit trails, log security-relevant events, and detect suspicious patterns.

---

## Background

**Security Logging and Monitoring Failures** is the **#9 risk** in the OWASP Top 10 (2021). Without proper logging, breaches go undetected. On average, it takes **over 200 days** to detect a breach. Logging helps with:

- **Detection** — identifying attacks as they happen
- **Investigation** — understanding what happened after a breach
- **Compliance** — meeting regulatory requirements (GDPR, PCI-DSS)
- **Alerting** — triggering responses to suspicious activity

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Add Structured Logging

The starter project has no meaningful logging. Security events happen silently.

### Tasks

1. Examine the starter project — notice there's no logging for security events.
2. Add structured logging for:
   - Successful logins: `logger.LogInformation("User {Username} logged in successfully from {IP}", ...)`
   - Failed logins: `logger.LogWarning("Failed login attempt for {Username} from {IP}", ...)`
   - Access denied: `logger.LogWarning("Access denied for {Username} to {Resource}", ...)`
3. Use `ILogger<T>` or `ILoggerFactory` to create loggers.
4. Include contextual information: username, IP address, timestamp, resource.

> **Hint:** Access the client IP via `HttpContext.Connection.RemoteIpAddress`

---

## Exercise 2 – Create an Audit Trail

### Tasks

1. Create an `AuditService` that records security events to an in-memory store.
2. Register it as a singleton service.
3. Record the following events:
   - `LOGIN_SUCCESS` — user, IP, timestamp
   - `LOGIN_FAILED` — attempted username, IP, timestamp
   - `ACCESS_DENIED` — user, resource, timestamp
   - `DATA_ACCESS` — user, resource, timestamp (for sensitive data endpoints)
4. Create a `GET /admin/audit` endpoint to view the audit trail (admin only).

---

## Exercise 3 – Detect Suspicious Activity

### Tasks

1. Create a `SuspiciousActivityDetector` service.
2. Track failed login attempts per IP address.
3. If more than 5 failed attempts within 5 minutes from the same IP, log a **critical** alert:
   ```csharp
   logger.LogCritical("Possible brute force attack from {IP} — {Count} failed attempts", ip, count);
   ```
4. Add the detection to the login endpoint.
5. Optionally return `Results.Problem("Too many failed attempts", statusCode: 429)` to throttle the attacker.

---

## Exercise 4 – Log Sensitive Data Access

### Tasks

1. Create middleware that logs all requests to endpoints tagged with `"Sensitive"`.
2. Log the HTTP method, path, user, and response status code.
3. Apply the `"Sensitive"` tag to endpoints that return personal or financial data.
4. Verify the logging output shows each access.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- Log **all security events** — logins, failures, access changes
- Use **structured logging** with contextual data (not just strings)
- Create **audit trails** for compliance and investigation
- **Detect patterns** like brute force attacks in real time
- **Never log sensitive data** like passwords, tokens, or credit card numbers
