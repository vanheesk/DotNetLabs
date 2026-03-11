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
// EXERCISE 1 & 2: SQL Injection
// =====================================================

// VULNERABLE ENDPOINT — uses string concatenation (DO NOT USE IN PRODUCTION)
// Exercise 1: Test with SQL injection payloads
// Exercise 2: Fix this by using parameterized queries
app.MapGet("/products/search", (string name, SqliteConnection db) =>
{
    // ⚠️ VULNERABLE: String concatenation allows SQL injection
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
.WithName("SearchProducts")
.WithSummary("Search products by name (VULNERABLE)")
.WithTags("Injection");

// TODO (Exercise 2): Create a new endpoint /products/search/safe that uses parameterized queries
// Hint: Use command.Parameters.AddWithValue("@name", searchTerm)
// The SQL should be: SELECT * FROM Products WHERE Name LIKE '%' || @name || '%'

// =====================================================
// EXERCISE 3: XSS Prevention
// =====================================================

// VULNERABLE ENDPOINT — reflects user input in HTML without encoding
app.MapGet("/greet", (string name) =>
    Results.Content($"<html><body><h1>Hello, {name}!</h1></body></html>", "text/html"))
.WithName("Greet")
.WithSummary("Greet a user (VULNERABLE to XSS)")
.WithTags("XSS");

// TODO (Exercise 3): Create a new endpoint /greet/safe that HTML-encodes the name
// Hint: Use System.Net.WebUtility.HtmlEncode(name)

// =====================================================
// EXERCISE 4: Input Validation Filter
// =====================================================

// TODO (Exercise 4): Create a SearchInputValidationFilter : IEndpointFilter
// that validates:
//   - name is not empty/whitespace
//   - name is <= 100 characters
//   - name does not contain suspicious characters like ', --, ;
// Apply it to the /products/search/safe endpoint

// =====================================================
// Default route
// =====================================================
app.MapGet("/", () => "Lab OWASP-1: Injection (A03:2021)")
    .ExcludeFromDescription();

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

app.Run();

// ----- Types -----
public record Product(int Id, string Name, double Price, string? Category);
