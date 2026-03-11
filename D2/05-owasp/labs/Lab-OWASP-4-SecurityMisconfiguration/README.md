# Lab 4: Security Misconfiguration (OWASP A05:2021)

## Objective

Identify and fix common **Security Misconfiguration** issues including information leakage through error messages, missing security headers, and CORS misconfiguration.

---

## Background

**Security Misconfiguration** is the **#5 risk** in the OWASP Top 10 (2021). It covers a broad range of issues:

- **Verbose error messages** exposing stack traces and internals
- **Missing security headers** (Content-Security-Policy, X-Content-Type-Options, etc.)
- **Overly permissive CORS** allowing any origin
- **Default credentials** or **unnecessary features** enabled
- **Missing TLS** or weak cipher configurations

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Information Leakage Through Error Details

The starter project exposes detailed exception information including stack traces in production responses.

### Tasks

1. Run the starter project and call `GET /error-test` — observe the detailed stack trace in the response.
2. Register `ProblemDetails` services: `builder.Services.AddProblemDetails()`.
3. Add `app.UseExceptionHandler()` to handle exceptions globally.
4. Add `app.UseStatusCodePages()` for consistent status code responses.
5. Verify that exceptions now return a generic `ProblemDetails` response without stack traces.

---

## Exercise 2 – Add Security Headers

Missing security headers leave the application vulnerable to clickjacking, MIME sniffing, and other attacks.

### Tasks

1. Create a middleware that adds the following security headers to every response:

   | Header | Value | Purpose |
   |--------|-------|---------|
   | `X-Content-Type-Options` | `nosniff` | Prevent MIME type sniffing |
   | `X-Frame-Options` | `DENY` | Prevent clickjacking |
   | `X-XSS-Protection` | `0` | Disable built-in XSS filter (CSP is preferred) |
   | `Referrer-Policy` | `strict-origin-when-cross-origin` | Control referrer information |
   | `Content-Security-Policy` | `default-src 'self'` | Restrict resource loading |
   | `Permissions-Policy` | `camera=(), microphone=(), geolocation=()` | Restrict browser features |

2. Register the middleware early in the pipeline.
3. Verify headers are present in the response using browser dev tools or `curl -I`.

---

## Exercise 3 – Fix CORS Misconfiguration

The starter has CORS configured to allow **all origins** — a security risk in production.

### Tasks

1. Examine the current CORS configuration: `builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()))`.
2. Replace with a restrictive policy that only allows specific origins:
   ```csharp
   builder.Services.AddCors(options =>
   {
       options.AddDefaultPolicy(policy =>
       {
           policy.WithOrigins("https://trusted-app.example.com")
                 .WithMethods("GET", "POST")
                 .WithHeaders("Content-Type", "Authorization");
       });
   });
   ```
3. Add a named policy `"Development"` that is more permissive for local development.
4. Use the appropriate policy based on the environment.

---

## Exercise 4 – Remove Debug Endpoints in Production

### Tasks

1. Identify the `/debug/config` endpoint that exposes application configuration.
2. Wrap it in an environment check so it's only available in Development:
   ```csharp
   if (app.Environment.IsDevelopment())
   {
       app.MapGet("/debug/config", ...);
   }
   ```
3. Ensure Swagger is also only available in Development or Staging.
4. Test by running with `ASPNETCORE_ENVIRONMENT=Production`.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- Never expose **stack traces** or **internal details** in production error responses
- Add **security headers** to every response
- Configure **CORS restrictively** — never allow all origins in production
- **Remove debug endpoints** and diagnostic tools in production
- Use **environment-specific configuration** for security settings
