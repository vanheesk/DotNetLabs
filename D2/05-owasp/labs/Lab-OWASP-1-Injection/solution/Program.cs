using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// Register the shared SQLite connection as a singleton so data persists
builder.Services.AddSingleton(_ =>
{
    var connection = new SqliteConnection("Data Source=:memory:");
    connection.Open();

    using var cmd = connection.CreateCommand();
    cmd.CommandText = """
        CREATE TABLE Products (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Price REAL NOT NULL,
            Category TEXT
        );
        INSERT INTO Products (Name, Price, Category) VALUES ('Laptop', 999.99, 'Electronics');
        INSERT INTO Products (Name, Price, Category) VALUES ('Desk Chair', 349.00, 'Furniture');
        INSERT INTO Products (Name, Price, Category) VALUES ('Keyboard', 79.99, 'Electronics');
        INSERT INTO Products (Name, Price, Category) VALUES ('Monitor', 449.00, 'Electronics');
        INSERT INTO Products (Name, Price, Category) VALUES ('Notebook', 4.99, 'Stationery');
    """;
    cmd.ExecuteNonQuery();

    return connection;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();

// =====================================================
// EXERCISE 1: Vulnerable endpoint (for demonstration)
// =====================================================

// ⚠️ VULNERABLE — String concatenation allows SQL injection (DO NOT USE IN PRODUCTION)
app.MapGet("/products/search/vulnerable", (string name, SqliteConnection db) =>
{
    var sql = $"SELECT * FROM Products WHERE Name LIKE '%{name}%'";

    using var command = db.CreateCommand();
    command.CommandText = sql;

    var products = new List<Product>();
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        products.Add(new Product(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetDouble(2),
            reader.IsDBNull(3) ? null : reader.GetString(3)));
    }

    return Results.Ok(products);
})
.WithName("SearchProductsVulnerable")
.WithSummary("Search products — VULNERABLE to SQL injection")
.WithTags("Vulnerable");

// =====================================================
// EXERCISE 2: Fixed endpoint using parameterized queries
// =====================================================

app.MapGet("/products/search", (string name, SqliteConnection db) =>
{
    using var command = db.CreateCommand();
    // ✅ SAFE: Parameterized query — the database treats @name as a literal value
    command.CommandText = "SELECT * FROM Products WHERE Name LIKE '%' || @name || '%'";
    command.Parameters.AddWithValue("@name", name);

    var products = new List<Product>();
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        products.Add(new Product(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetDouble(2),
            reader.IsDBNull(3) ? null : reader.GetString(3)));
    }

    return Results.Ok(products);
})
.AddEndpointFilter<SearchInputValidationFilter>()
.WithName("SearchProductsSafe")
.WithSummary("Search products — SAFE with parameterized queries + validation")
.WithTags("Secure");

// =====================================================
// EXERCISE 3: XSS Prevention
// =====================================================

// ⚠️ VULNERABLE — Reflects user input in HTML without encoding
app.MapGet("/greet/vulnerable", (string name) =>
    Results.Content($"<html><body><h1>Hello, {name}!</h1></body></html>", "text/html"))
.WithName("GreetVulnerable")
.WithSummary("Greet — VULNERABLE to XSS")
.WithTags("Vulnerable");

// ✅ SAFE — HTML-encodes user input before rendering
app.MapGet("/greet", (string name) =>
{
    var encoded = System.Net.WebUtility.HtmlEncode(name);
    return Results.Content(
        $"<html><body><h1>Hello, {encoded}!</h1></body></html>",
        "text/html");
})
.WithName("GreetSafe")
.WithSummary("Greet — SAFE with HTML encoding")
.WithTags("Secure");

// =====================================================
// Supporting endpoints
// =====================================================

app.MapGet("/products", (SqliteConnection db) =>
{
    using var command = db.CreateCommand();
    command.CommandText = "SELECT * FROM Products";

    var products = new List<Product>();
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        products.Add(new Product(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetDouble(2),
            reader.IsDBNull(3) ? null : reader.GetString(3)));
    }

    return Results.Ok(products);
})
.WithName("GetAllProducts")
.WithSummary("Get all products")
.WithTags("Products");

app.MapGet("/", () => "Lab OWASP-1: Injection (A03:2021)")
    .ExcludeFromDescription();

app.Run();

// ----- Types -----
public record Product(int Id, string Name, double Price, string? Category);

// ----- Exercise 4: Reusable Input Validation Filter -----
public class SearchInputValidationFilter : IEndpointFilter
{
    private static readonly char[] SuspiciousChars = ['\'', ';'];

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var searchTerm = context.GetArgument<string>(0);
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            errors["name"] = ["Search term is required"];
        }
        else if (searchTerm.Length > 100)
        {
            errors["name"] = ["Search term must be 100 characters or less"];
        }
        else if (searchTerm.Any(c => SuspiciousChars.Contains(c)) || searchTerm.Contains("--"))
        {
            errors["name"] = ["Search term contains invalid characters"];
        }

        if (errors.Count > 0)
            return Results.ValidationProblem(errors);

        return await next(context);
    }
}
