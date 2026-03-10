// ============================================================
// Lab 5: Code Quality & Consistency (SOLUTION)
// All issues have been fixed.
// ============================================================

var service = new UserService();

service.AddUser("Alice", "alice@example.com");
service.AddUser("Bob", null);
service.AddUser(null, "test@test.com");

// Correctly handling nullable return
var user = service.FindByName("Alice");
if (user is not null)
{
    Console.WriteLine($"Found: {user.Name} ({user.Email})");
}

// Correctly handling the case where user is not found
var missing = service.FindByName("Charlie");
if (missing is not null)
{
    Console.WriteLine($"Found: {missing.Name}");
}
else
{
    Console.WriteLine("User 'Charlie' not found.");
}

// Display all users
foreach (var u in service.GetAllUsers())
{
    Console.WriteLine($"  {u.Name} - {u.Email}");
}

// ---------- Fixed code below ----------

public class UserService
{
    // FIXED: Private field uses _camelCase and is readonly
    private readonly List<User> _users = [];

    // FIXED: Parameters are nullable since callers may pass null
    public void AddUser(string? name, string? email)
    {
        var user = new User { Name = name ?? "Unknown", Email = email ?? "no-email" };
        _users.Add(user);
    }

    // FIXED: Return type is nullable since Find can return null
    public User? FindByName(string name)
    {
        return _users.Find(u => u.Name == name);
    }

    public List<User> GetAllUsers() => [.. _users];
}

public class User
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
