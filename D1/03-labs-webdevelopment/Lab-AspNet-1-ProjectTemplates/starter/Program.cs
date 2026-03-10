var builder = WebApplication.CreateBuilder(args);

// TODO (Exercise 3): Register Swagger services
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// TODO (Exercise 3): Enable Swagger middleware
// app.UseSwagger();
// app.UseSwaggerUI();

// ----- Exercise 2: Add a Hello endpoint -----
// TODO: Map a GET endpoint at "/hello/{name}" that returns $"Hello, {name}!"

// ----- Exercise 4: Add more endpoints -----
// TODO: Map GET "/time" that returns DateTime.UtcNow

// TODO: Map GET "/random/{min}/{max}" that returns a random number between min and max

// TODO: Map POST "/echo" that accepts a JsonElement body and returns it

app.Run();
