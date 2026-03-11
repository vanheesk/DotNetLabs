# Lab 10: Server-Side Request Forgery — SSRF (OWASP API7:2023) — Instructor Guide

## Teaching Notes

SSRF has become increasingly important with cloud adoption. The 2019 Capital One breach was a classic SSRF attack through the cloud metadata endpoint. This is now a separate entry in both OWASP Top 10 lists.

### Key Points to Emphasise

1. **SSRF is a gateway to internal networks** — the API server becomes a proxy
2. **Cloud metadata endpoints are high-value targets** — credentials, access tokens
3. **Allowlists > blocklists** — blocklists can be bypassed with encoding, DNS rebinding, etc.
4. **DNS rebinding** is a real threat — validate the resolved IP, not just the hostname
5. **Defense in depth** — combine URL validation with network segmentation

### Demo Flow

1. Show the vulnerable proxy endpoint
2. Attempt to access localhost endpoints — show internal data leaking
3. Show the cloud metadata risk (explain, don't actually access cloud)
4. Fix with scheme validation, IP blocking, and allowlisting
5. Configure HttpClient for additional safety

---

## Exercise 2 – Solution

```csharp
static bool IsSchemeAllowed(Uri uri)
{
    return uri.Scheme == Uri.UriSchemeHttps;
}
```

---

## Exercise 3 – Solution

```csharp
static bool IsPrivateOrReservedIp(System.Net.IPAddress ip)
{
    byte[] bytes = ip.GetAddressBytes();

    // IPv4 checks
    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    {
        // Loopback: 127.0.0.0/8
        if (bytes[0] == 127) return true;

        // Private: 10.0.0.0/8
        if (bytes[0] == 10) return true;

        // Private: 172.16.0.0/12
        if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true;

        // Private: 192.168.0.0/16
        if (bytes[0] == 192 && bytes[1] == 168) return true;

        // Link-local / cloud metadata: 169.254.0.0/16
        if (bytes[0] == 169 && bytes[1] == 254) return true;
    }

    // IPv6 loopback
    if (System.Net.IPAddress.IsLoopback(ip)) return true;

    return false;
}
```

---

## Exercise 4 – Solution

```csharp
// In appsettings.json:
// { "AllowedDomains": ["httpbin.org", "api.github.com"] }

var allowedDomains = builder.Configuration.GetSection("AllowedDomains").Get<string[]>() ?? [];

// In the endpoint:
if (!allowedDomains.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
{
    return Results.BadRequest($"Domain '{uri.Host}' is not in the allowlist");
}
```

---

## Exercise 5 – Solution

```csharp
builder.Services.AddHttpClient("safe", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
    client.MaxResponseContentBufferSize = 1_048_576;
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AllowAutoRedirect = false // Prevent redirect-based SSRF bypasses
});
```

> **Teaching note:** Explain that auto-redirect can bypass URL validation. An attacker might host a URL that redirects to `http://169.254.169.254/`, bypassing the initial validation.

---

## Common Student Issues

1. **Not resolving DNS before checking** — hostname might resolve to a private IP
2. **Forgetting IPv6** — `::1` is loopback, `::ffff:127.0.0.1` is mapped IPv4
3. **Not disabling redirects** — attackers chain redirects to bypass validation
4. **Using blocklists instead of allowlists** — many bypass techniques exist
5. **Forgetting to set timeouts** — vulnerable to slow-loris style attacks
