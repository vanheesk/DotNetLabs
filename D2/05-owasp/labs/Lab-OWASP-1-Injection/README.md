# Lab 1: Injection (OWASP A03:2021)

## Objective

Understand **SQL Injection** and **Cross-Site Scripting (XSS)** vulnerabilities, see them in action, and learn how to mitigate them using parameterized queries, input validation, and output encoding.

---

## Background

**Injection** is one of the most common and dangerous web application vulnerabilities. It occurs when untrusted data is sent to an interpreter as part of a command or query. The attacker's hostile data can trick the interpreter into executing unintended commands or accessing data without authorisation.

### Common Types
- **SQL Injection** — Manipulating SQL queries through user input
- **Cross-Site Scripting (XSS)** — Injecting client-side scripts into web pages
- **Command Injection** — Executing OS commands through user input

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Identify SQL Injection Vulnerability

The starter project has a `/products/search` endpoint that builds SQL queries using **string concatenation** — a classic SQL injection vulnerability.

### Tasks

1. Run the starter project and browse to `/swagger`.
2. Try the `GET /products/search?name=Laptop` endpoint — it works normally.
3. Now try a SQL injection attack:
   ```
   GET /products/search?name=' OR '1'='1
   ```
4. Observe that **all products** are returned — the injection succeeded.
5. Try another attack that attempts to extract data:
   ```
   GET /products/search?name=' UNION SELECT sql,2,3 FROM sqlite_master--
   ```
6. Examine the vulnerable code in `Program.cs` and understand why string concatenation is dangerous.

---

## Exercise 2 – Fix with Parameterized Queries

### Tasks

1. Replace the vulnerable string-concatenated SQL in the `/products/search` endpoint with a **parameterized query**.
2. Use `@name` as a parameter placeholder and add it via `command.Parameters.AddWithValue()`.
3. Test the same SQL injection attempts from Exercise 1 — they should now return no results instead of leaking data.
4. Verify that legitimate searches still work correctly.

> **Hint:** The parameterized query should look like:
> ```sql
> SELECT * FROM Products WHERE Name LIKE '%' || @name || '%'
> ```

---

## Exercise 3 – Prevent XSS with Output Encoding

The starter project has a `/greet` endpoint that reflects user input directly in an HTML response — a **Reflected XSS** vulnerability.

### Tasks

1. Test the endpoint with normal input:
   ```
   GET /greet?name=Alice
   ```
2. Test with a malicious XSS payload:
   ```
   GET /greet?name=<script>alert('XSS')</script>
   ```
3. Observe that the script tag is rendered in the HTML response (vulnerable).
4. Fix the endpoint by encoding the user input using `System.Net.WebUtility.HtmlEncode()` before including it in the HTML response.
5. Verify the XSS payload is now safely encoded in the output.

---

## Exercise 4 – Input Validation with Endpoint Filters

Rather than only encoding output, it's best to also **validate and sanitise input** before processing.

### Tasks

1. Create an endpoint filter that validates search input:
   - Reject empty or whitespace-only input
   - Reject input longer than 100 characters
   - Reject input containing SQL-suspicious characters like `'`, `--`, `;`
2. Apply the filter to the `/products/search` endpoint.
3. Return `Results.ValidationProblem(...)` with descriptive error messages.
4. Test that malicious inputs are rejected at the filter level.

---

## Exercise 5 – Use Entity Framework Core (Bonus)

Using an ORM like EF Core inherently prevents SQL injection because it uses parameterized queries internally.

### Tasks

1. Add the `Microsoft.EntityFrameworkCore.Sqlite` NuGet package.
2. Create a `ProductDbContext` with a `DbSet<Product>`.
3. Replace the raw SQL endpoint with an EF Core LINQ query:
   ```csharp
   db.Products.Where(p => p.Name.Contains(searchTerm)).ToList()
   ```
4. Verify that SQL injection is no longer possible.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- **Never** concatenate user input into SQL queries
- Always use **parameterized queries** or an ORM
- **Encode** output to prevent XSS
- **Validate** input at the boundary
- Defence in depth: combine multiple mitigation strategies
