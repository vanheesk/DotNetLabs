using System.Net;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// Exercise 5: Configure HttpClient safely
builder.Services.AddHttpClient("safe", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
    client.MaxResponseContentBufferSize = 1_048_576; // 1 MB max response
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AllowAutoRedirect = false // Prevent redirect-based SSRF bypasses
});

builder.Services.AddHttpClient(); // Default client for vulnerable demo

// Load allowed domains from configuration
var allowedDomains = builder.Configuration.GetSection("AllowedDomains").Get<string[]>() ?? [];

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Simulated internal endpoint
app.MapGet("/admin/secrets", () => new
{
    dbConnectionString = "Server=prod-db;Database=main;User=admin;Password=P@ssw0rd!",
    apiKey = "sk-super-secret-api-key-12345",
    internalNote = "This should never be exposed via the public API"
})
.WithName("AdminSecrets")
.WithSummary("Internal secrets (SSRF target for testing)")
.WithTags("Internal");

// =====================================================
// Vulnerable endpoint (for comparison)
// =====================================================

app.MapGet("/api/fetch/vulnerable", async (string url, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetStringAsync(url);
        return Results.Ok(new { url, content = response });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("FetchUrlVulnerable")
.WithSummary("VULNERABLE — fetches any URL without validation")
.WithTags("Vulnerable");

// =====================================================
// EXERCISE 2-5: Safe fetch endpoint
// =====================================================

app.MapGet("/api/fetch", async (string url, IHttpClientFactory httpClientFactory, ILogger<Program> logger) =>
{
    // Exercise 2: Validate URL scheme
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
    {
        return Results.BadRequest(new { error = "Invalid URL format" });
    }

    if (uri.Scheme != Uri.UriSchemeHttps)
    {
        logger.LogWarning("SSRF blocked — disallowed scheme: {Scheme} in URL: {Url}", uri.Scheme, url);
        return Results.BadRequest(new { error = $"Only HTTPS URLs are allowed. Got: {uri.Scheme}" });
    }

    // Exercise 4: Check domain allowlist
    if (!allowedDomains.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
    {
        logger.LogWarning("SSRF blocked — domain not in allowlist: {Host}", uri.Host);
        return Results.BadRequest(new { error = $"Domain '{uri.Host}' is not in the allowlist. Allowed: {string.Join(", ", allowedDomains)}" });
    }

    // Exercise 3: Resolve and validate IP addresses
    try
    {
        var addresses = await Dns.GetHostAddressesAsync(uri.Host);
        foreach (var ip in addresses)
        {
            if (IsPrivateOrReservedIp(ip))
            {
                logger.LogWarning("SSRF blocked — private/reserved IP {IP} for host {Host}", ip, uri.Host);
                return Results.BadRequest(new { error = $"URL resolves to a private/reserved IP address ({ip})" });
            }
        }
    }
    catch (SocketException)
    {
        return Results.BadRequest(new { error = $"Could not resolve hostname: {uri.Host}" });
    }

    // Exercise 5: Use the safely configured HttpClient
    try
    {
        var client = httpClientFactory.CreateClient("safe");
        var response = await client.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            return Results.BadRequest(new { error = $"Remote server returned {response.StatusCode}" });
        }

        // Check for redirects (since we disabled auto-redirect)
        if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
        {
            logger.LogWarning("SSRF blocked — redirect response from {Url}", url);
            return Results.BadRequest(new { error = "Redirects are not allowed" });
        }

        var content = await response.Content.ReadAsStringAsync();
        return Results.Ok(new { url, contentLength = content.Length, content });
    }
    catch (TaskCanceledException)
    {
        return Results.BadRequest(new { error = "Request timed out" });
    }
    catch (HttpRequestException ex)
    {
        return Results.BadRequest(new { error = $"Request failed: {ex.Message}" });
    }
})
.WithName("FetchUrlSafe")
.WithSummary("SAFE — validates scheme, domain allowlist, and resolved IP")
.WithTags("Secure");

app.MapGet("/", () => "Lab OWASP-API-5: Server-Side Request Forgery (API7:2023)")
    .ExcludeFromDescription();

app.Run();

// ----- Helper: Check for private/reserved IP addresses -----
static bool IsPrivateOrReservedIp(IPAddress ip)
{
    // IPv6 loopback
    if (IPAddress.IsLoopback(ip))
        return true;

    byte[] bytes = ip.GetAddressBytes();

    if (ip.AddressFamily == AddressFamily.InterNetwork)
    {
        // Loopback: 127.0.0.0/8
        if (bytes[0] == 127) return true;

        // Private: 10.0.0.0/8
        if (bytes[0] == 10) return true;

        // Private: 172.16.0.0/12
        if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true;

        // Private: 192.168.0.0/16
        if (bytes[0] == 192 && bytes[1] == 168) return true;

        // Link-local / Cloud metadata: 169.254.0.0/16
        if (bytes[0] == 169 && bytes[1] == 254) return true;

        // Broadcast: 255.255.255.255
        if (bytes.All(b => b == 255)) return true;

        // 0.0.0.0/8
        if (bytes[0] == 0) return true;
    }

    return false;
}
