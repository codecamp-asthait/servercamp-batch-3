using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.DataProtection;

/*
 * --- CLASS 11: MANUAL AUTHENTICATION & MULTI-SCHEME FLOW ---
 * 
 * In this class, we are NOT using the built-in '.AddAuthentication()' or '.UseAuthentication()' middleware.
 * Instead, we are building our own middleware to understand exactly how ASP.NET Core handles:
 * 1. Identity Resolution (Who are you?)
 * 2. Multi-Scheme Support (Cookies vs JWT)
 * 3. Data Protection (Encrypting/Decrypting Cookies)
 * 4. Token Validation (Verifying JWT signatures)
 */

var key = "THIS_IS_SUPER_SECRET_KEY_1234567890";
var keyBytes = Encoding.UTF8.GetBytes(key);

var builder = WebApplication.CreateBuilder(args);

// Data Protection is required to securely encrypt and decrypt our manual cookies.
builder.Services.AddDataProtection();

var app = builder.Build();

// This dictionary simulates a database of "Route Metadata".
// It tells our manual middleware which authentication schemes are allowed for each path.
var endpoints = new Dictionary<string, Metadata>
{
    ["/secure-with-jwt"] = new Metadata
    {
        // This endpoint accepts EITHER a JWT or a Cookie
        AuthSchemes = ["Bearer", "Cookies"],
    },
    ["/secure-with-cookie"] = new Metadata
    {
        // This endpoint ONLY accepts Cookies
        AuthSchemes = ["Cookies"]
    },
};

// --- CUSTOM AUTHENTICATION MIDDLEWARE ---
// This takes the place of 'app.UseAuthentication()'
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    
    // Skip authentication for login endpoints
    if (path is "/login-with-jwt" or "/login-with-cookie")
    {
        await next();
        return;
    }

    // If the path isn't in our "Secure List", just move on
    if (!endpoints.TryGetValue(path!, out var metadata))
    {
        await next();
        return;
    }

    var schemes = metadata.AuthSchemes;

    // Initialize an empty User (ClaimsPrincipal).
    context.User = new ClaimsPrincipal();

    // IMPORTANT CONCEPT: Multi-Identity Principal
    // In ASP.NET Core, a single 'User' (ClaimsPrincipal) can have MULTIPLE 'Identities' (ClaimsIdentity).
    // For example, a user might be authenticated via both a Cookie AND a JWT simultaneously.
    // Each successful scheme validation below will add a new Identity to the same User object.
    foreach (var scheme in schemes)
    {
        // --- SCHEME 1: MANUAL COOKIE HANDLING ---
        if (scheme == "Cookies")
        {
            var idp = context.RequestServices.GetRequiredService<IDataProtectionProvider>();
            var protector = idp.CreateProtector("codecamp-protector");

            // Look for our custom 'codecamp' cookie
            var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("codecamp"));
            if (string.IsNullOrEmpty(authCookie)) continue;

            try
            {
                // 1. Decrypt the cookie payload
                var protectedPayload = authCookie.Split("=").Last();
                var payload = protector.Unprotect(protectedPayload);
                
                // 2. Parse the payload (formatted as "type:value,type:value")
                var values = payload.Split(",");
                var claims = new List<Claim>();
                foreach (var value in values)
                {
                    var parts = value.Split(":");
                    claims.Add(new Claim(parts[0], parts[1]));
                }

                // 3. Create a Cookie Identity and add it to the User
                var claimsIdentity = new ClaimsIdentity(claims, "cookie");
                context.User.AddIdentity(claimsIdentity);
            }
            catch
            {
                // If cookie is tampered with or invalid, ignore it for this scheme
            }
        }

        // --- SCHEME 2: MANUAL JWT HANDLING ---
        if (scheme == "Bearer")
        {
            var authHeader = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) continue;

            var token = authHeader.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            
            // Define rules for verifying the token signature
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                // 1. Verify signature and expiration
                var principal = handler.ValidateToken(token, parameters, out _);
                if (principal == null) continue;

                // 2. Extract claims from the valid token
                var username = principal.Identity?.Name;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;

                var claims = new List<Claim>
                {
                    new("username", username),
                    new("role", role)
                };
                
                // 3. Create a JWT Identity and add it to the User
                var claimsIdentity = new ClaimsIdentity(claims, "jwt");
                context.User.AddIdentity(claimsIdentity);
            }
            catch
            {
                // If token is invalid, ignore it for this scheme
            }
        }
    }

    // --- FINAL CHECK (Manual Authorization) ---
    // If after checking all allowed schemes, the User still has no "Identities",
    // it means they are not authenticated.
    if (!context.User.Identities.Any())
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized: No valid authentication provided.");
        return;
    }

    await next();
});

// --- SECURE ENDPOINTS ---

app.MapGet("/secure-with-jwt", (HttpContext ctx) =>
{
    // Accessing information from the principal constructed in our middleware
    var username = ctx.User.FindFirst("username")?.Value;
    var role = ctx.User.FindFirst("role")?.Value;

    return Results.Ok(new
    {
        message = "Access Granted! (JWT validated manually)",
        user = username,
        role
    });
});

app.MapGet("/secure-with-cookie", (HttpContext ctx) =>
{
    // Here we use the .Claims collection directly to show the student another way to access data
    var claims = ctx.User.Claims;
    var result = new Dictionary<string, string>
    {
        ["message"] = "Access Granted! (Cookie decrypted manually)"
    };

    foreach (var claim in claims)
    {
        if (!result.ContainsKey(claim.Type))
            result.Add(claim.Type, claim.Value);
    }

    return Results.Ok(result);
});

// --- LOGIN ENDPOINTS (Issuance) ---

app.MapGet("/login-with-jwt", (string userName, string password) =>
{
    if (userName != "shafayet" || password != "password")
        return Results.Unauthorized();

    // 1. Define the user's data
    var claims = new[]
    {
        new Claim(ClaimTypes.Name, userName),
        new Claim(ClaimTypes.Role, "admin")
    };

    // 2. Describe the token (who, when, how to sign)
    var tokenDescriptor = new SecurityTokenDescriptor()
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(30),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256Signature
        )
    };

    // 3. Generate the token string
    var handler = new JwtSecurityTokenHandler();
    var token = handler.CreateToken(tokenDescriptor);
    var jwt = handler.WriteToken(token);

    return Results.Ok(new { token = jwt });
});

app.MapGet("/login-with-cookie", (
    string userName,
    string password,
    HttpContext context,
    IDataProtectionProvider idp) =>
{
    if (userName != "shafayet" || password != "password")
        return Results.Unauthorized();

    // 1. Create a "Protector" with a specific purpose/key
    var protector = idp.CreateProtector("codecamp-protector");
    
    // 2. Encrypt our custom claims string
    var encryptedData = protector.Protect($"username:{userName},role:admin");
    
    // 3. Set the cookie in the response header
    context.Response.Headers["set-cookie"] = $"codecamp={encryptedData}; HttpOnly; Path=/";
    
    return Results.Ok("Logged in! Cookie issued.");
});

app.Run();

public class Metadata
{
    public List<string> AuthSchemes { get; set; } = new();
}