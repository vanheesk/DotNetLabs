# Lab 6: Broken Object Level Authorization — BOLA (OWASP API1:2023) — Instructor Guide

## Teaching Notes

BOLA is the **#1 API security risk** for a reason — it's extremely common and trivial to exploit. Most APIs have some form of this vulnerability.

### Key Points to Emphasise

1. **BOLA is the API equivalent of IDOR** — same concept, API-specific context
2. **Every endpoint that takes an object ID needs authorization** — no exceptions
3. **Authentication is not authorization** — having a valid token doesn't mean access to all resources
4. **GUIDs are not security** — they add obscurity but attackers can still enumerate or find them
5. **Automated testing** can detect BOLA — change IDs in API tests and verify 403 responses

### Demo Flow

1. Show the API — login as patient-1, access patient-2's records
2. Show how trivial this is with curl or Postman
3. Fix with ownership check
4. Demonstrate the doctor role override
5. Discuss real-world impact: healthcare breaches, financial data exposure

---

## Exercise 2 – Solution

```csharp
app.MapGet("/api/patients/{patientId}/records", (string patientId, HttpContext context) =>
{
    var currentUser = context.User.FindFirst("sub")?.Value;
    var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

    // Check ownership OR admin/doctor role
    if (currentUser != patientId && userRole != "doctor")
    {
        return Results.Forbid();
    }

    var patientRecords = records.Where(r => r.PatientId == patientId).ToList();
    return Results.Ok(patientRecords);
}).RequireAuthorization();
```

---

## Exercise 3 – Solution

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("PatientOrDoctor", policy =>
    {
        policy.RequireAuthenticatedUser();
    });

// In the endpoint, check programmatically:
if (currentUser != patientId && !context.User.IsInRole("doctor"))
    return Results.Forbid();
```

> **Teaching note:** Explain that policy-based authorization is for function-level access. Object-level authorization often requires programmatic checks because the ownership relationship is dynamic.

---

## Exercise 4 – Solution

```csharp
// Replace integer IDs with GUIDs
var records = new List<MedicalRecord>
{
    new(Guid.NewGuid(), "patient-1", "Annual checkup", "2025-01-15"),
    new(Guid.NewGuid(), "patient-1", "Blood work", "2025-03-20"),
    // ...
};
```

> **Teaching note:** Emphasise that GUIDs make enumeration harder but are NOT a security measure. An attacker who obtains a GUID (from logs, URLs, other responses) can still use it. Authorization checks are mandatory.

---

## Common Student Issues

1. **Forgetting `.RequireAuthorization()`** — the endpoint has no auth at all
2. **Checking authentication but not authorization** — valid token but wrong user
3. **Hard-coding role checks** instead of using claims — less maintainable
4. **Not handling the case where `sub` claim is missing** — defensive programming
