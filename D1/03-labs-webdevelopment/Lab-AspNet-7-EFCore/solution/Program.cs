using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----- Exercise 1: Register TodoDb with SQLite -----
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlite("Data Source=todos.db"));

var app = builder.Build();

// Ensure the database is created (for demo purposes)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

// ----- Exercise 3: Use MapGroup -----
var todos = app.MapGroup("/todos").WithTags("Todos");

// ----- Exercise 2 & 3: CRUD Endpoints -----

// GET /todos – with optional ?complete filter
todos.MapGet("/", async (TodoDb db, bool? complete) =>
{
    IQueryable<Todo> query = db.Todos;

    if (complete.HasValue)
        query = query.Where(t => t.IsComplete == complete.Value);

    return Results.Ok(await query.ToListAsync());
}).WithName("GetTodos")
  .WithSummary("List all todos, optionally filtered by completion status");

// GET /todos/{id}
todos.MapGet("/{id:int}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id) is Todo todo
        ? Results.Ok(todo)
        : Results.NotFound())
    .WithName("GetTodoById");

// POST /todos
todos.MapPost("/", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
}).WithName("CreateTodo");

// PUT /todos/{id}
todos.MapPut("/{id:int}", async (int id, Todo input, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = input.Title;
    todo.IsComplete = input.IsComplete;
    todo.DueDate = input.DueDate;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("UpdateTodo");

// DELETE /todos/{id}
todos.MapDelete("/{id:int}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
}).WithName("DeleteTodo");

app.MapGet("/", () => "Lab 7: Data Access with EF Core");

app.Run();

// ----- Models -----

public class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? DueDate { get; set; }
}

// ----- DbContext -----

public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}
