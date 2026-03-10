// ============================================================
// Lab 5: Code Quality & Consistency
// This file has intentional issues — fix them!
// ============================================================

var service = new UserService();

service.AddUser("Alice", "alice@example.com");
service.AddUser("Bob", null);        // Intentional: email could be null
service.AddUser(null, "test@test.com"); // Intentional: name could be null

// This might return null — but we're not handling it
var user = service.FindByName("Alice");
Console.WriteLine($"Found: {user.Name} ({user.Email})");

// This will definitely return null
var missing = service.FindByName("Charlie");
Console.WriteLine($"Missing: {missing.Name}"); // NullReferenceException!

// Display all users
foreach (var u in service.GetAllUsers())
{
    Console.WriteLine($"  {u.Name} - {u.Email}");
}

// ---------- Intentionally messy code below ----------

// Naming violation: class-level field should be _camelCase
public class UserService
{
    // BUG: Should be _users (private field naming convention)
    private List<User> Users = new List<User>();

    // BUG: name parameter should be nullable since callers pass null
    public void AddUser(string name, string email)
    {
        var user = new User { Name = name ?? "Unknown", Email = email ?? "no-email" };
        Users.Add(user);
    }

    // BUG: Return type should be nullable since Find can return null
    public User FindByName(string name)
    {
        return Users.Find(u => u.Name == name);
    }

    public List<User> GetAllUsers()
    {
        return Users;
    }
}

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}
