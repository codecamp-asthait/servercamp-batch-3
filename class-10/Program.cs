using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- Authentication Configuration ---
// AddAuthentication registers authentication services.
// CookieAuthenticationDefaults.AuthenticationScheme ("Cookies") is the default scheme.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Name of the cookie that will be stored in the browser
        options.Cookie.Name = "codecamp";

        // Custom behavior for when an unauthorized user tries to access a protected resource.
        // Instead of redirecting to a login page (default for MVC), we return 401 Unauthorized for APIs.
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

// Registers authorization services (required for [Authorize] or .RequireAuthorization())
builder.Services.AddAuthorization();

var app = builder.Build();

// --- Middleware Pipeline ---
// UseAuthentication: Checks if a valid authentication cookie is present in the request.
// If found, it populates the HttpContext.User property.
app.UseAuthentication();

// UseAuthorization: Checks if the authenticated user has permission to access the resource.
app.UseAuthorization();

/// <summary>
/// A protected endpoint that requires the user to be authenticated via a cookie.
/// </summary>
app.MapGet("/cookie-authorized", (HttpContext context) =>
{
    return Results.Ok("You're authenticated by Cookie");
})
.RequireAuthorization(); // This ensures only authenticated users can access this route.

/// <summary>
/// Login endpoint to authenticate a user and issue a cookie.
/// </summary>
/// <param name="userName">The username provided by the user.</param>
/// <param name="password">The password provided by the user.</param>
/// <param name="context">The HttpContext of the current request.</param>
app.MapGet("/login", async (string userName, string password, HttpContext context) =>
{
    // Hardcoded check for demonstration purposes. In a real app, use a database.
    if (userName != "shafayet" && password != "password") return Results.Unauthorized();

    // Claims: Key-value pairs that describe the authenticated user (e.g., name, role, email).
    var claims = new List<Claim>
    {
        new("username", userName),
        new("batch", "codecamp-3")
    };

    // ClaimsIdentity: Represents the user's identity (like a driver's license).
    // It contains the claims and specifies the authentication type.
    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    // ClaimsPrincipal: Represents the security context of the user (like a person holding multiple IDs).
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

    // SignInAsync: Serializes the principal into a cookie and adds it to the response.
    // The browser will automatically send this cookie back in subsequent requests.
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
    
    return Results.Ok("Login successful!");
});

app.Run();