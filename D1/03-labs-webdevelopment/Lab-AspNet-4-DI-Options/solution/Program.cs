using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ----- Exercise 1: Register IClock -----
builder.Services.AddSingleton<IClock, SystemClock>();

// ----- Exercise 2: Lifetimes -----
builder.Services.AddTransient<TransientId>();
builder.Services.AddScoped<ScopedId>();

// ----- Exercise 3: Options Pattern -----
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));

var app = builder.Build();

// ----- Exercise 1: Time endpoint -----
app.MapGet("/time", (IClock clock) => new { utc = clock.Now });

// ----- Exercise 2: Lifetime demo -----
app.MapGet("/lifetime", (TransientId t1, TransientId t2, ScopedId s1, ScopedId s2) =>
    new
    {
        transient1 = t1.Id,
        transient2 = t2.Id,
        transientMatch = t1.Id == t2.Id,       // false — different instances
        scoped1 = s1.Id,
        scoped2 = s2.Id,
        scopedMatch = s1.Id == s2.Id            // true — same instance per request
    });

// ----- Exercise 3: Settings endpoint -----
app.MapGet("/settings", (IOptions<AppSettings> options) => options.Value);

// ----- Exercise 5: Live-reload endpoint -----
app.MapGet("/settings/live", (IOptionsMonitor<AppSettings> monitor) => monitor.CurrentValue);

app.MapGet("/", () => "Lab 4: Dependency Injection & Options Pattern");

app.Run();

// ----- Type definitions -----

public interface IClock
{
    DateTime Now { get; }
}

public class SystemClock : IClock
{
    public DateTime Now => DateTime.UtcNow;
}

public class TransientId
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class ScopedId
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class AppSettings
{
    public string Title { get; set; } = "";
    public int MaxPageSize { get; set; }
    public bool EnableFeatureX { get; set; }
}
