using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// --- Custom Authentication Middleware ---
// This manual implementation shows what happens 'under the hood' of app.UseAuthentication().
app.Use(async (context, next) =>
{
    // 1. Try to find the 'codecamp' cookie in the request headers.
    var authCookie = context.Request.Headers.Cookie.FirstOrDefault(c => c.StartsWith("codecamp"));

    // 2. If no cookie is found, we return 401 Unauthorized.
    if (authCookie == null || authCookie.Length <= 0)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized: No authentication cookie found.");
        return;
    }

    // 3. Parse the cookie payload. Format: "codecamp=username:Shafayet"
    var payload = authCookie.Split("=").Last(); // Get "username:Shafayet"
    var parts = payload.Split(":"); // Split into ["username", "Shafayet"]
    var key = parts[0];
    var value = parts[1];

    // 4. Create Claims and an Identity based on the cookie data.
    // A Claim is a piece of information about the user (like username or role).
    var claims = new List<Claim>
    {
        new(key, value)
    };

    // ClaimsIdentity: Represents the user's identity (like a digital passport).
    // IMPORTANT: The second parameter (CookieAuthenticationDefaults.AuthenticationScheme) is the 'AuthenticationType'.
    // If this is null or empty, the identity's 'IsAuthenticated' property will be FALSE, 
    // even if it contains claims. Specifying the scheme marks the identity as authenticated.
    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    // 5. Set the User property of the current HttpContext. 
    // This makes the user 'authenticated' for the rest of the pipeline.
    context.User = new ClaimsPrincipal(claimsIdentity);

    // 6. Call next() to allow the request to proceed to the next middleware or endpoint.
    await next();
});

/// <summary>
/// A protected endpoint that uses the user identity populated by our manual middleware.
/// </summary>
app.MapGet("/cookie-authorized", (HttpContext context) =>
{
    // Retrieve the username claim from the authenticated user.
    var value = context.User.FindFirst("username");

    return Results.Ok(value?.Value);
});

/// <summary>
/// Login endpoint that manually sets an authentication cookie.
/// </summary>
app.MapGet("/login", async (string userName, string password, HttpContext context) =>
{
    // Simple credential check.
    if (userName != "shafayet" && password != "password") return Results.Unauthorized();

    // Manually create the cookie string.
    var secret = $"username:{userName}";

    // Set the 'set-cookie' header so the browser stores the identity.
    // In a real app, this should be encrypted and marked as Secure/HttpOnly.
    context.Response.Headers["set-cookie"] = $"codecamp={secret}";

    return Results.Ok("Login successful! Cookie has been set.");
});

app.Run();

// --- Extra: Abstracting Authentication logic (for advanced study) ---
// These classes demonstrate how one might start building a more flexible system 
// using common Design Patterns like the Factory Pattern and Strategy Pattern.

/* 
   WHY USE THIS?
   In a real-world project, you might need to support MULTIPLE ways to log in 
   (e.g., Cookies for web users, Bearer Tokens for mobile apps, or API Keys for 3rd parties).
   
   Instead of writing huge 'if-else' blocks inside your endpoints, you can use 
   the Factory Pattern to "ask" for the right service and the Strategy Pattern 
   to define "how" each one works.
*/

/// <summary>
/// A Simple Factory that decides which authentication service to use based on a string 'scheme'.
/// This helps decouple the calling code from the specific implementations of authentication.
/// </summary>
public class AuthFactory
{
    // Real-world usage: app.MapPost("/login", async (string scheme) => { 
    //    await AuthFactory.SignInAsync(scheme); 
    // });
    public static async Task SignInAsync(string scheme)
    {
        // Based on the 'scheme' (strategy), we instantiate and use the appropriate service.
        // Both services implement IIAuthService, so we can treat them interchangeably.
        if (scheme == "cookie") await new CookieAuthServicee().SignInAsync();
        if (scheme == "bearer") await new BearerAuthServicee().SignInAsync();
    }
}

/// <summary>
/// This interface defines the 'Contract' for any authentication service.
/// Any class that implements this MUST provide a SignInAsync method.
/// This allows us to use 'Polymorphism'—calling the same method name on different objects.
/// </summary>
public interface IIAuthService
{
    public Task SignInAsync();
}

/// <summary>
/// Implementation of authentication using Cookies.
/// Encapsulates the logic for setting 'set-cookie' headers.
/// </summary>
public class CookieAuthServicee : IIAuthService
{
    public Task SignInAsync()
    {
        Console.WriteLine("Executing Strategy: Sign in with Cookie");
        // Logic to set cookie headers would go here...
        return Task.CompletedTask;
    }
}

/// <summary>
/// Implementation of authentication using Bearer Tokens (JWT).
/// Encapsulates the logic for generating and returning a JSON Web Token.
/// </summary>
public class BearerAuthServicee : IIAuthService
{
    public Task SignInAsync()
    {
        Console.WriteLine("Executing Strategy: Sign in with Bearer Token");
        // Logic to generate JWT would go here...
        return Task.CompletedTask;
    }
}