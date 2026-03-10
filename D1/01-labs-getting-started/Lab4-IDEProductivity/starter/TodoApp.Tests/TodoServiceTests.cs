using TodoApp;

namespace TodoApp.Tests;

public class TodoServiceTests
{
    [Fact]
    public void AddTodo_ReturnsTodoWithCorrectTitle()
    {
        var service = new TodoService();
        var todo = service.AddTodo("Test task");

        Assert.Equal("Test task", todo.Title);
        Assert.False(todo.IsCompleted);
    }

    [Fact]
    public void AddTodo_AssignsIncrementingIds()
    {
        var service = new TodoService();
        var first = service.AddTodo("First");
        var second = service.AddTodo("Second");

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
    }

    [Fact]
    public void CompleteTodo_MarksAsCompleted()
    {
        var service = new TodoService();
        service.AddTodo("Task to complete");

        var result = service.CompleteTodo(1);

        Assert.True(result);
        Assert.True(service.FindTodo(1)!.IsCompleted);
    }

    [Fact]
    public void CompleteTodo_NonExistentId_ReturnsFalse()
    {
        var service = new TodoService();
        var result = service.CompleteTodo(999);

        Assert.False(result);
    }

    [Fact]
    public void RemoveTodo_RemovesFromList()
    {
        var service = new TodoService();
        service.AddTodo("Task to remove");

        var result = service.RemoveTodo(1);

        Assert.True(result);
        Assert.Empty(service.GetAllTodos());
    }

    [Fact]
    public void GetPendingTodos_ReturnsOnlyIncomplete()
    {
        var service = new TodoService();
        service.AddTodo("Pending");
        service.AddTodo("Done");
        service.CompleteTodo(2);

        var pending = service.GetPendingTodos();

        Assert.Single(pending);
        Assert.Equal("Pending", pending[0].Title);
    }

    [Fact]
    public void GetCompletedTodos_ReturnsOnlyCompleted()
    {
        var service = new TodoService();
        service.AddTodo("Pending");
        service.AddTodo("Done");
        service.CompleteTodo(2);

        var completed = service.GetCompletedTodos();

        Assert.Single(completed);
        Assert.Equal("Done", completed[0].Title);
    }

    [Fact]
    public void FindTodo_ExistingId_ReturnsTodo()
    {
        var service = new TodoService();
        service.AddTodo("Find me");

        var found = service.FindTodo(1);

        Assert.NotNull(found);
        Assert.Equal("Find me", found.Title);
    }

    [Fact]
    public void FindTodo_NonExistentId_ReturnsNull()
    {
        var service = new TodoService();
        var found = service.FindTodo(999);

        Assert.Null(found);
    }
}
