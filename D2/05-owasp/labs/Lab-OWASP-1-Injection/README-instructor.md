# Lab 1: Injection (OWASP A03:2021) — Instructor Guide

## Teaching Notes

This lab demonstrates the **#3 most critical web application risk** according to OWASP 2021. Injection attacks remain prevalent because developers still use string concatenation for building queries.

### Key Points to Emphasise

1. **SQL Injection is still common** — despite being well-known for over 20 years
2. **Parameterized queries are the primary defence** — not input validation alone
3. **ORMs provide built-in protection** — EF Core uses parameterized queries internally
4. **XSS is a form of injection** — injecting into HTML/JavaScript context
5. **Defence in depth** — combine input validation, parameterized queries, and output encoding

### Demo Flow

1. Start with the **vulnerable** endpoint and show how easy SQL injection is
2. Show the actual SQL being constructed (log it) so students see the concatenation
3. Fix with parameterized queries and demonstrate the attack no longer works
4. Move to XSS and show how reflected input can execute scripts
5. Fix with encoding and discuss Content Security Policy headers

---

## Exercise 1 – Solution

The vulnerable code uses string concatenation:

```csharp
// VULNERABLE - DO NOT USE IN PRODUCTION
var sql = $"SELECT * FROM Products WHERE Name LIKE '%{searchTerm}%'";
```

When `searchTerm` is `' OR '1'='1`, the SQL becomes:
```sql
SELECT * FROM Products WHERE Name LIKE '%' OR '1'='1%'
```

This always evaluates to `true`, returning all rows.

---

## Exercise 2 – Solution

The fix uses parameterized queries:

```csharp
command.CommandText = "SELECT * FROM Products WHERE Name LIKE '%' || @name || '%'";
command.Parameters.AddWithValue("@name", searchTerm);
```

The database engine treats `@name` as a **literal value**, not as SQL code. The injected `'` characters are escaped automatically.

---

## Exercise 3 – Solution

**Vulnerable code:**
```csharp
app.MapGet("/greet", (string name) =>
    Results.Content($"<html><body><h1>Hello, {name}!</h1></body></html>", "text/html"));
```

**Fixed code:**
```csharp
app.MapGet("/greet", (string name) =>
{
    var encoded = System.Net.WebUtility.HtmlEncode(name);
    return Results.Content($"<html><body><h1>Hello, {encoded}!</h1></body></html>", "text/html");
});
```

With encoding, `<script>alert('XSS')</script>` becomes `&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;` — harmless text.

---

## Exercise 4 – Solution

```csharp
public class SearchInputValidationFilter : IEndpointFilter
{
    private static readonly char[] SuspiciousChars = ['\'', ';', '-'];

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var searchTerm = context.GetArgument<string>(0);
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(searchTerm))
            errors["name"] = ["Search term is required"];
        else if (searchTerm.Length > 100)
            errors["name"] = ["Search term must be 100 characters or less"];
        else if (searchTerm.Any(c => SuspiciousChars.Contains(c)))
            errors["name"] = ["Search term contains invalid characters"];

        if (errors.Count > 0)
            return Results.ValidationProblem(errors);

        return await next(context);
    }
}
```

> **Teaching note:** Emphasise that input validation is a **secondary** defence. Parameterized queries are the primary mitigation. Input validation adds defence in depth but should never be the sole protection.

---

## Exercise 5 – Solution

```csharp
// Add package: dotnet add package Microsoft.EntityFrameworkCore.Sqlite

public class ProductDbContext : DbContext
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=:memory:");
}

// EF Core query - inherently safe from SQL injection
app.MapGet("/products/search/ef", async (string name, ProductDbContext db) =>
    await db.Products.Where(p => p.Name.Contains(name)).ToListAsync());
```

---

## Common Student Issues

1. **Forgetting to use `@` prefix** for parameter names in SQLite
2. **Not understanding why encoding works** — show the raw HTML output
3. **Thinking input validation alone is sufficient** — demonstrate bypasses
4. **SQLite syntax differences** — `||` for concatenation instead of `+`
