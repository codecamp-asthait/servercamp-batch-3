# Class 12: Advanced Authorization, Identity, and Database Integration

This class explores the implementation of robust security in ASP.NET Core, transitioning from simple checks to granular, policy-based authorization backed by a persistent database.

## Commit-by-Commit Explanation

### 1. Granular Policy-Based Authorization & JWT Security
**Commit:** `Class-12: implement granular claim, role, and policy-based authorization and JWT security`
- **The Goal:** Master the "Rules of Access" by moving beyond simple authentication to complex authorization logic.
- **Key Concepts:**
    - **Custom Claims:** Using organizational metadata (e.g., `org: ait`) to categorize users.
    - **Named Policies:** Creating reusable authorization rules (like `admin-policy`) that combine multiple requirements (Claims + Roles).
    - **Inline Requirements:** Applying security directly to endpoints using `.RequireAuthorization(policy => ...)` for one-off access rules.
    - **JWT Configuration:** Setting up the infrastructure for secure, stateless communication using JSON Web Tokens.

### 2. Database-Backed Identity & Persistent Security
**Commit:** `feat(class-12): integrate PostgreSQL and database-backed Identity management`
- **The Goal:** Bridge the gap between application logic and persistent storage using ASP.NET Core Identity and Entity Framework Core.
- **Key Concepts:**
    - **PostgreSQL Integration:** Connecting the security system to a real database using `Npgsql`.
    - **ApplicationDbContext:** Understanding how `IdentityDbContext` manages the underlying tables for Users, Roles, and Claims.
    - **Identity Management:** Using `UserManager` to handle complex tasks like password hashing, user registration, and credential verification against the database.
    - **Educational Configuration:** Intentionally simplifying password requirements to focus on the core flow of registration and login during the learning process.

---

## Summary for Students
By studying these commits, you will learn how to:
1. **Define** who can access what using flexible Policies and Claims.
2. **Store** and manage user identities securely in a persistent database.
3. **Verify** real-world credentials using industry-standard Identity frameworks.
4. **Issue** secure tokens (JWT) that encapsulate these permissions for use in modern web architectures.
