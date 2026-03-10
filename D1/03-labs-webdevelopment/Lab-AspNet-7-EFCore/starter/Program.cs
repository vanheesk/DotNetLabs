using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO Exercise 1: Register TodoDb with SQLite
// builder.Services.AddDbContext<TodoDb>(options =>
//     options.UseSqlite("Data Source=todos.db"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// TODO Exercise 2: Add CRUD endpoints for Todo
// GET    /todos
// GET    /todos/{id}
// POST   /todos
// PUT    /todos/{id}
// DELETE /todos/{id}

// TODO Exercise 3: Use MapGroup("/todos") and add ?complete filter

app.MapGet("/", () => "Lab 7: Data Access with EF Core");

app.Run();

// ----- Models -----

// TODO Exercise 1: Define Todo entity
// public class Todo
// {
//     public int Id { get; set; }
//     public required string Title { get; set; }
//     public bool IsComplete { get; set; }
//     public DateTime? DueDate { get; set; }
// }

// TODO Exercise 1: Define TodoDb DbContext
// public class TodoDb : DbContext
// {
//     public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
//     public DbSet<Todo> Todos => Set<Todo>();
// }
