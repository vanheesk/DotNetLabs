# Lab 10: Server-Side Request Forgery — SSRF (OWASP API7:2023)

## Objective

Understand **Server-Side Request Forgery (SSRF)** vulnerabilities where an API can be tricked into making requests to unintended destinations. Learn to mitigate with URL validation, allowlists, and network segmentation.

---

## Background

**SSRF** occurs when an API fetches a remote resource based on user-supplied input without proper validation. This is the **#7 API security risk** (2023) and also the **#10 web application risk** (OWASP 2021).

### Attack Scenarios

- Access **internal services** (metadata endpoints, admin panels) through the API
- **Port scan** internal networks
- Access **cloud metadata** APIs (e.g., `http://169.254.169.254/` on AWS/Azure/GCP)
- Read **local files** via `file://` protocol
- **Bypass firewalls** by using the server as a proxy

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs and `HttpClient`

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Identify SSRF Vulnerability

The starter has a `/api/fetch` endpoint that proxies HTTP requests based on user input without validation.

### Tasks

1. Run the starter project and navigate to `/swagger`.
2. Test with a legitimate URL:
   ```
   GET /api/fetch?url=https://httpbin.org/get
   ```
3. Test with an internal address — attempting to access cloud metadata:
   ```
   GET /api/fetch?url=http://169.254.169.254/latest/meta-data/
   ```
4. Test with localhost to access internal services:
   ```
   GET /api/fetch?url=http://localhost:5000/admin/secrets
   ```
5. Test with the file protocol:
   ```
   GET /api/fetch?url=file:///etc/passwd
   ```
6. Understand how each of these could be exploited in a real environment.

---

## Exercise 2 – Validate URL Scheme

### Tasks

1. Only allow `https://` URLs — reject `http://`, `file://`, `ftp://`, etc.
2. Parse the URL using `Uri.TryCreate()` and check the scheme.
3. Return `400 Bad Request` for disallowed schemes.
4. Test that `file://` and `http://` are rejected.

---

## Exercise 3 – Block Internal and Reserved IP Ranges

### Tasks

1. Resolve the hostname to an IP address using `Dns.GetHostAddresses()`.
2. Block requests to:
   - Loopback addresses (`127.0.0.0/8`, `::1`)
   - Private networks (`10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`)
   - Link-local addresses (`169.254.0.0/16` — cloud metadata)
3. Return `400 Bad Request` with a message explaining the URL is blocked.
4. Test with `http://127.0.0.1`, `http://10.0.0.1`, `http://169.254.169.254`.

---

## Exercise 4 – URL Allowlist

For maximum security, only allow requests to known, trusted domains.

### Tasks

1. Define an allowlist of trusted domains (e.g., `["httpbin.org", "api.github.com"]`).
2. Validate that the requested URL's host is in the allowlist.
3. Store the allowlist in configuration (`appsettings.json`).
4. Return `400 Bad Request` for domains not in the allowlist.
5. Test with allowed and disallowed domains.

---

## Exercise 5 – Configure HttpClient Safely

### Tasks

1. Set a **timeout** on `HttpClient` to prevent slow responses from tying up resources.
2. Disable **automatic redirects** to prevent redirect-based SSRF bypasses.
3. Limit **response size** to prevent memory exhaustion.
4. Use `IHttpClientFactory` for proper lifecycle management.

> **Hint:**
> ```csharp
> builder.Services.AddHttpClient("safe", client =>
> {
>     client.Timeout = TimeSpan.FromSeconds(5);
>     client.MaxResponseContentBufferSize = 1_048_576; // 1 MB
> }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
> {
>     AllowAutoRedirect = false
> });
> ```

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- **Never proxy raw user-supplied URLs** without validation
- **Allowlists** are stronger than blocklists — prefer them when possible
- **Validate at multiple layers**: scheme, hostname, resolved IP, and allowlist
- **Block cloud metadata endpoints** (`169.254.169.254`) explicitly
- **Configure HttpClient securely** — timeouts, no auto-redirect, size limits
- In production, use **network segmentation** as an additional defence
