using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ----- Exercise 1: Register IClock -----
// TODO: Define IClock interface and SystemClock implementation below
// TODO: Register: builder.Services.AddSingleton<IClock, SystemClock>();

// ----- Exercise 2: Lifetimes -----
// TODO: Register RequestInfo as transient and scoped (see instructions)

// ----- Exercise 3: Options Pattern -----
// TODO: Create an AppSettings class and bind it:
// builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));

var app = builder.Build();

// ----- Exercise 1: Time endpoint -----
// TODO: Map GET "/time" that injects IClock and returns the current time

// ----- Exercise 2: Lifetime demo endpoint -----
// TODO: Map GET "/lifetime" that resolves services to show transient vs scoped behaviour

// ----- Exercise 3: Settings endpoint -----
// TODO: Map GET "/settings" that injects IOptions<AppSettings> and returns the value

// ----- Exercise 5: Live-reload endpoint -----
// TODO: Map GET "/settings/live" that injects IOptionsMonitor<AppSettings>

app.MapGet("/", () => "Lab 4: Dependency Injection & Options Pattern");

app.Run();

// ----- Type definitions -----

// TODO: Define IClock interface
// public interface IClock { DateTime Now { get; } }

// TODO: Define SystemClock
// public class SystemClock : IClock { public DateTime Now => DateTime.UtcNow; }

// TODO: Define AppSettings class
// public class AppSettings
// {
//     public string Title { get; set; } = "";
//     public int MaxPageSize { get; set; }
//     public bool EnableFeatureX { get; set; }
// }
