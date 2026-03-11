# Lab 4: Security Misconfiguration (OWASP A05:2021) — Instructor Guide

## Teaching Notes

Security Misconfiguration is extremely common because it requires attention to many small details. Students often focus on writing secure code but forget about configuring the infrastructure securely.

### Key Points to Emphasise

1. **Stack traces in production** are an information goldmine for attackers
2. **Security headers are free protection** — they take minutes to add
3. **CORS `AllowAnyOrigin()` is almost never correct** in production
4. **Debug endpoints must be removed** or gated by environment
5. **Defence in depth** — each header protects against a different attack vector

### Demo Flow

1. Show an error response with full stack trace — explain what an attacker learns
2. Add ProblemDetails and show the sanitised response
3. Show missing headers in browser dev tools
4. Add headers middleware and verify
5. Demo CORS misconfiguration with a cross-origin request

---

## Exercise 1 – Solution

```csharp
builder.Services.AddProblemDetails();

app.UseExceptionHandler();
app.UseStatusCodePages();
```

Before: The raw exception with stack trace is returned.
After: A standardised ProblemDetails response with no internal details.

---

## Exercise 2 – Solution

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "0");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    await next();
});
```

> **Teaching note:** Explain each header's purpose. Show that `X-XSS-Protection: 0` is correct — the built-in browser XSS filter has been deprecated and can itself be exploited. CSP is the modern replacement.

---

## Exercise 3 – Solution

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://trusted-app.example.com")
              .WithMethods("GET", "POST")
              .WithHeaders("Content-Type", "Authorization");
    });

    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// In the pipeline:
if (app.Environment.IsDevelopment())
    app.UseCors("Development");
else
    app.UseCors();
```

---

## Exercise 4 – Solution

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/debug/config", (IConfiguration config) =>
        config.AsEnumerable().Select(kvp => new { kvp.Key, kvp.Value }))
        .WithTags("Debug");
}
```

---

## Common Student Issues

1. **Headers added after response starts** — security headers middleware must be early in the pipeline
2. **Forgetting `app.UseCors()` call** — CORS services alone don't apply the policy
3. **CORS vs security headers confusion** — CORS controls cross-origin requests, security headers protect the browser
4. **Testing CORS** — students need to understand it's enforced by the browser, not the server
