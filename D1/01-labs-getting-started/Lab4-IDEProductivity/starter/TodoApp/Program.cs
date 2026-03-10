// ============================================================
// Lab 4: IDE Productivity – TodoApp
// Use this project to practice IDE features:
//   debugging, navigation, refactoring, test running.
// ============================================================

using TodoApp;

Console.WriteLine("=== Todo App ===");
Console.WriteLine();

var service = new TodoService();

service.AddTodo("Learn .NET CLI basics");
service.AddTodo("Understand build lifecycle");
service.AddTodo("Explore publishing options");
service.AddTodo("Master IDE productivity");

service.CompleteTodo(1);
service.CompleteTodo(2);

Console.WriteLine("All todos:");
foreach (var todo in service.GetAllTodos())
{
    string status = todo.IsCompleted ? "✓" : " ";
    Console.WriteLine($"  [{status}] {todo.Id}. {todo.Title}");
}

Console.WriteLine();
Console.WriteLine($"Pending: {service.GetPendingTodos().Count}");
Console.WriteLine($"Completed: {service.GetCompletedTodos().Count}");
