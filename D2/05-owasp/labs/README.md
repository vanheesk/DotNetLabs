# OWASP Security Labs

This module provides hands-on labs covering the **OWASP Top 10 (2021)** and **OWASP API Security Top 10 (2023)** vulnerabilities using ASP.NET Core and .NET 10 Minimal APIs.

Each lab demonstrates a specific vulnerability, explains why it's dangerous, and guides you through mitigating it.

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with ASP.NET Core Minimal APIs
- Basic understanding of HTTP, REST, and authentication concepts

---

## OWASP Top 10 (2021) Labs

| Lab | OWASP ID | Topic | Focus |
|-----|----------|-------|-------|
| [Lab 1](Lab-OWASP-1-Injection/) | A03:2021 | **Injection** | SQL Injection, XSS, input validation |
| [Lab 2](Lab-OWASP-2-BrokenAccessControl/) | A01:2021 | **Broken Access Control** | IDOR, missing authorization |
| [Lab 3](Lab-OWASP-3-CryptoFailures/) | A02:2021 | **Cryptographic Failures** | Password hashing, data protection |
| [Lab 4](Lab-OWASP-4-SecurityMisconfiguration/) | A05:2021 | **Security Misconfiguration** | Headers, CORS, error leakage |
| [Lab 5](Lab-OWASP-5-LoggingMonitoring/) | A09:2021 | **Logging & Monitoring Failures** | Audit trails, structured logging |

## OWASP API Security Top 10 (2023) Labs

| Lab | OWASP ID | Topic | Focus |
|-----|----------|-------|-------|
| [Lab 6](Lab-OWASP-API-1-BOLA/) | API1:2023 | **Broken Object Level Authorization** | Object ownership validation |
| [Lab 7](Lab-OWASP-API-2-BrokenAuth/) | API2:2023 | **Broken Authentication** | Token security, credential handling |
| [Lab 8](Lab-OWASP-API-3-MassAssignment/) | API3:2023 | **Property Level Authorization** | Mass assignment, excessive data exposure |
| [Lab 9](Lab-OWASP-API-4-RateLimiting/) | API4:2023 | **Unrestricted Resource Consumption** | Rate limiting, throttling |
| [Lab 10](Lab-OWASP-API-5-SSRF/) | API7:2023 | **Server-Side Request Forgery** | URL validation, allowlists |

---

## How to Use

Each lab contains:

- **`README.md`** — Student lab instructions with exercises and tasks
- **`README-instructor.md`** — Instructor guide with full solutions and teaching notes
- **`starter/`** — Starting project with TODOs for students to complete
- **`solution/`** — Completed reference implementation for instructors

```bash
cd Lab-OWASP-X-Name/starter
dotnet run
```

---

## References

- [OWASP Top 10 (2021)](https://owasp.org/Top10/)
- [OWASP API Security Top 10 (2023)](https://owasp.org/API-Security/editions/2023/en/0x11-t10/)
- [ASP.NET Core Security Documentation](https://learn.microsoft.com/aspnet/core/security/)
