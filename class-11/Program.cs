using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

/*
 * --- CLASS 11: JWT AUTHENTICATION BASICS ---
 * 
 * CONCEPT 1: Authentication (Who are you?)
 * We use the '/login-with-jwt' endpoint to verify credentials and issue a "Passport" (JWT).
 * 
 * CONCEPT 2: Authorization (What can you do?)
 * We use the 'secure' endpoint to check the "Passport" and see if the user has the right permissions (Claims/Roles).
 * 
 * CONCEPT 3: JWT (JSON Web Token)
 * A self-contained way to securely transmit information between parties as a JSON object.
 * It consists of Header, Payload (Claims), and Signature.
 */

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// This key is used to sign and verify our tokens. 
// In production, this should be stored securely (e.g., Environment Variables or Key Vault).
var key = "secret-key-secret-key-secret-key-12345";

// ENDPOINT: Secure Resource
// This simulates a page that only logged-in users can see.
app.MapGet("secure", (HttpContext context) =>
{
    // 1. Get the Authorization header from the request
    var authHeader = context.Request.Headers.Authorization.ToString();

    // 2. Check if the header exists and follows the 'Bearer <token>' pattern
    if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) 
    {
        return Results.Unauthorized();
    }

    // 3. Extract the actual token string
    var token = authHeader.Replace("Bearer ", ""); 

    var handler = new JwtSecurityTokenHandler();
    
    // 4. Define how we want to validate the token
    var parameters = new TokenValidationParameters
    {
        ValidateIssuer = false,      // For this demo, we don't check who issued it
        ValidateAudience = false,    // For this demo, we don't check who it's for
        ValidateLifetime = true,      // IMPORTANT: Check if the token has expired
        ValidateIssuerSigningKey = true, // IMPORTANT: Check if the signature matches our secret key
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };

    try 
    {
        // 5. Validate the token. If it's tampered with or expired, this will fail.
        var principal = handler.ValidateToken(token, parameters, out _);
        
        // 6. Extract user identity information (Claims) from the validated token
        var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;

        return Results.Ok(new
        {
            message = "Access Granted! Your token is valid.",
            userName,
            role
        });
    }
    catch (Exception)
    {
        // If validation fails (e.g., token was modified by a hacker), return Unauthorized
        return Results.Unauthorized();
    }
});

// ENDPOINT: Login / Token Issuance
// Users provide credentials here to receive a JWT.
app.MapGet("/login-with-jwt", (string userName, string password) =>
{
    // 1. Authenticate the user (Hardcoded for demonstration)
    if (userName != "shafayet" && password != "password")
        return Results.Unauthorized();

    // 2. Create "Claims" - pieces of information about the user
    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, userName),
        new(ClaimTypes.Role, "admin") // Giving the user an 'admin' role
    };

    // 3. Describe what the token should contain
    var tokenDescriptor = new SecurityTokenDescriptor()
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(30), // Token expires in 30 minutes
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), // Sign with our secret key
            SecurityAlgorithms.HmacSha256Signature // Use HMAC SHA256 algorithm
        )
    };

    // 4. Generate and write the token to a string
    var handler = new JwtSecurityTokenHandler();
    var token = handler.CreateToken(tokenDescriptor);
    var jwt = handler.WriteToken(token);

    // 5. Return the token to the client
    return Results.Ok(new { token = jwt });
});


app.Run();