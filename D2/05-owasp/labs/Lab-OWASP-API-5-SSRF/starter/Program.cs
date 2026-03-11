var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Simulated internal endpoint that should NOT be accessible externally
app.MapGet("/admin/secrets", () => new
{
    dbConnectionString = "Server=prod-db;Database=main;User=admin;Password=P@ssw0rd!",
    apiKey = "sk-super-secret-api-key-12345",
    internalNote = "This should never be exposed via the public API"
})
.WithName("AdminSecrets")
.WithSummary("Internal secrets (should not be reachable via SSRF)")
.WithTags("Internal");

// =====================================================
// EXERCISE 1: SSRF Vulnerability
// =====================================================

// ⚠️ VULNERABLE: Fetches any URL provided by the user
app.MapGet("/api/fetch", async (string url, IHttpClientFactory httpClientFactory) =>
{
    // No validation of the URL at all!
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
.WithName("FetchUrl")
.WithSummary("Fetch a URL (VULNERABLE — no validation)")
.WithTags("Proxy");

// TODO (Exercise 2): Create /api/fetch/safe that validates URL scheme
// TODO (Exercise 3): Add IP address validation to block private/reserved ranges
// TODO (Exercise 4): Add domain allowlist validation
// TODO (Exercise 5): Configure HttpClient with timeout, no auto-redirect, size limits

app.MapGet("/", () => "Lab OWASP-API-5: Server-Side Request Forgery (API7:2023)")
    .ExcludeFromDescription();

app.Run();
