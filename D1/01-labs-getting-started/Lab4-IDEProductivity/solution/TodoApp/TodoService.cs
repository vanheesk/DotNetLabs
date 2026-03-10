namespace TodoApp;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TodoService
{
    private readonly List<TodoItem> _todos = [];
    private int _nextId = 1;

    public TodoItem AddTodo(string title)
    {
        var todo = new TodoItem
        {
            Id = _nextId++,
            Title = title
        };
        _todos.Add(todo);
        return todo;
    }

    public bool CompleteTodo(int id)
    {
        var todo = _todos.Find(t => t.Id == id);
        if (todo is null) return false;
        todo.IsCompleted = true;
        return true;
    }

    public bool RemoveTodo(int id)
    {
        var todo = _todos.Find(t => t.Id == id);
        if (todo is null) return false;
        return _todos.Remove(todo);
    }

    public List<TodoItem> GetAllTodos() => [.. _todos];

    public List<TodoItem> GetPendingTodos() =>
        _todos.Where(t => !t.IsCompleted).ToList();

    public List<TodoItem> GetCompletedTodos() =>
        _todos.Where(t => t.IsCompleted).ToList();

    public TodoItem? FindTodo(int id) => _todos.Find(t => t.Id == id);
}
