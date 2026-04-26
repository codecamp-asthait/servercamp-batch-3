# Class 11: Deep Dive into Authentication & Multi-Scheme Flow

This class focuses on understanding how ASP.NET Core identifies users and handles security across different communication methods (Web Browsers vs. API Clients).

## Commit-by-Commit Explanation

### 1. Manual JWT Implementation
**Commit:** `Class-11: implement manual JWT authentication and authorization flow`
- **The Goal:** Strip away the "magic" of built-in middleware to understand how JWTs work at a low level.
- **Key Concepts:**
    - **Extraction:** Manually pulling the `Authorization: Bearer <token>` header from the request.
    - **Validation:** Using `TokenValidationParameters` to verify the cryptographic signature and expiration date.
    - **Principal Construction:** Converting the validated claims into a `ClaimsPrincipal` that the application can use.

### 2. Multi-Scheme with Standard Middleware & Default Schemes
**Commit:** `Class-11: implement Default Authentication Scheme in multi-scheme pipeline`
- **The Goal:** Transitioning to production-ready ASP.NET Core patterns while handling multiple security requirements.
- **Key Concepts:**
    - **Multi-Scheme Registration:** Configuring both `.AddCookie()` and `.AddJwtBearer()` in the same application.
    - **The Default Scheme:** Setting Cookies as the default so that simple `[Authorize]` or `.RequireAuthorization()` calls know exactly which security "rulebook" to check first.
    - **Boilerplate Reduction:** Moving validation logic from the route handlers into the global service configuration.

### 3. Manual Multi-Scheme Middleware
**Commit:** `Class-11: implement manual multi-scheme authentication middleware`
- **The Goal:** Mastering the "Multi-Identity" nature of ASP.NET Core security.
- **Key Concepts:**
    - **Custom Pipeline:** Building a manual `app.Use(...)` middleware that mimics the behavior of the built-in authentication system.
    - **Cookie Decryption:** Using `IDataProtectionProvider` to manually unprotect and read custom security cookies.
    - **Multi-Identity Principal:** Demonstrating that a single `User` (`ClaimsPrincipal`) can hold multiple identities (e.g., one from a Cookie and one from a JWT) at the same time.
    - **Metadata-Driven Security:** Using a custom dictionary to map routes to their required authentication schemes, explaining how the framework resolves route-specific security requirements.

---

## Summary for Students
By following these commits, you will see the evolution from **"How do I do this manually?"** to **"How does the framework do this for me?"** and finally to **"How do I customize the framework for complex scenarios?"**
