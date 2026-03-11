# Lab 6: Broken Object Level Authorization — BOLA (OWASP API1:2023)

## Objective

Understand **Broken Object Level Authorization (BOLA)**, the **#1 API security risk**. Learn to detect and fix API endpoints that allow users to access objects they don't own by manipulating object IDs.

---

## Background

**BOLA** (also known as **IDOR** — Insecure Direct Object Reference) is the most common API vulnerability. APIs tend to expose endpoints that handle object identifiers, creating a wide attack surface. An attacker simply changes an ID in the request to access another user's data.

### Why APIs Are Especially Vulnerable

- APIs expose structured object IDs (auto-increment, UUIDs)
- APIs often lack server-side authorisation checks on individual objects
- API traffic is easier to manipulate than web UIs

---

## Prerequisites

- .NET 10 SDK installed
- Familiarity with Minimal APIs and JWT authentication

```bash
cd starter
dotnet run
```

---

## Exercise 1 – Identify BOLA Vulnerability

The starter API exposes medical records by patient ID with no ownership checks.

### Tasks

1. Run the starter project and navigate to `/swagger`.
2. Obtain a token for patient `patient-1` via `POST /auth/token`.
3. Access `GET /api/patients/patient-1/records` — your own records return correctly.
4. Access `GET /api/patients/patient-2/records` — **you can see another patient's medical records!**
5. Try `GET /api/patients/patient-2/records/3` — you can access individual records too.
6. Understand why this is critical for healthcare, banking, and other sensitive data.

---

## Exercise 2 – Fix with Object-Level Authorization

### Tasks

1. Modify the patient records endpoints to verify the authenticated user owns the requested resource.
2. Extract the `sub` claim from the JWT token.
3. Compare with the `patientId` route parameter.
4. Return `403 Forbidden` for unauthorized access.
5. Verify that `patient-1` can only access their own records.

---

## Exercise 3 – Admin Override

Some users (like doctors) legitimately need to access other patients' records.

### Tasks

1. Add a `doctor` role to the authorization system.
2. Create a policy that allows doctors to bypass the ownership check.
3. Modify the endpoint to check: is the user the owner OR has the doctor role?
4. Test that doctors can access any patient's records, but patients can only access their own.

---

## Exercise 4 – Use GUIDs Instead of Sequential IDs

Sequential integer IDs make BOLA easier to exploit. GUIDs are harder to guess.

### Tasks

1. Replace integer record IDs with GUIDs.
2. Discuss why this is **not a security fix** — it's defence in depth.
3. Understand that BOLA must be fixed with authorisation, not obscurity.

---

## Wrapping Up

```bash
dotnet run
```

Compare your implementation with the `solution` folder. Key takeaways:

- **Every object access** must verify the requester has permission
- Object-level authorisation must be checked on the **server side**
- Using GUIDs adds obscurity but is **not a substitute** for authorisation
- Consider **role-based overrides** for legitimate cross-user access
