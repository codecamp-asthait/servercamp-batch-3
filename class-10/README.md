# Class 10: Authentication & Authorization Exploration

This project provides two different ways to understand Authentication and Authorization in ASP.NET Core. You can explore the git history to see the progression from standard built-in middleware to a manual, low-level implementation.

## Two Ways to Explore

You can **checkout** different commits to see how the code changes between a standard implementation and a manual one.

### 1. Standard Cookie Authentication (The "Easy" Way)
To see the standard implementation using built-in ASP.NET Core middleware, run:
```bash
git checkout 36eac57
```
*   **Key features**: Uses `builder.Services.AddAuthentication()`, `AddCookie()`, and `SignInAsync`.
*   **Best for**: Understanding how you will actually build apps in production.

### 2. Manual "Under-the-Hood" Implementation (The "Learning" Way)
To see how authentication works internally (parsing cookies and creating identities manually), run:
```bash
git checkout 4afb59d
```
*   **Key features**: Custom middleware, manual header parsing, and explicit `ClaimsPrincipal` construction.
*   **Best for**: Understanding the core mechanics and design patterns (Factory/Strategy).

### How to return to the latest version
After exploring, you can return to the latest state of the project (including this README) by running:
```bash
git checkout dotnet-class-10
```

---

## Key Learning Objectives

1.  **Claims-Based Identity**: Understanding `Claim` (the fact), `ClaimsIdentity` (the ID card), and `ClaimsPrincipal` (the holder).
2.  **Middleware Pipeline**: Seeing how the order of middleware affects the request lifecycle.
3.  **Design Patterns**: The manual implementation includes an **Extra** section demonstrating the **Factory Pattern** and **Strategy Pattern** for supporting multiple authentication schemes (like switching between Cookies and JWT Bearer Tokens).

## How to Test
Use the `class-10.http` file to run test requests for:
1.  Accessing protected resources without login (401 Unauthorized).
2.  Logging in to receive an authentication cookie.
3.  Accessing protected resources with the cookie (200 OK).

## Running the Project
1.  Navigate to the `class-10` directory.
2.  Run `dotnet run`.
