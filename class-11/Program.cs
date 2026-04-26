using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

/*
 * --- CLASS 11: MULTI-SCHEME WITH DEFAULT AUTHENTICATION ---
 * 
 * CONCEPT: Default Authentication Scheme
 * When we have multiple schemes (Cookie, JWT, Google, etc.), we can set one as the "Default".
 * 
 * WHY? 
 * This simplifies our code. Any route that uses .RequireAuthorization() without 
 * specifying a scheme will automatically use this Default.
 * 
 * In this example, we set "Cookies" as the Default.
 */

var key = "secret-key-secret-key-secret-key-secret-key-secret-key-secret-key";

var builder = WebApplication.CreateBuilder(args);

// CONFIGURATION: Registering Multiple Authentication Schemes
// We pass the Default Scheme name into AddAuthentication()
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie() 
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization(); 

// --- LOGIN ENDPOINTS ---

app.MapGet("/login-with-cookie", async (string userName, string password, HttpContext context) =>
{
    if (userName != "shafayet" || password != "password") return Results.Unauthorized();
    var claims = new List<Claim> { new(ClaimTypes.Name, userName) };
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    return Results.Ok("Logged in with Cookie!");
});

app.MapGet("/login-with-jwt", (string userName, string password) =>
{
    if (userName != "shafayet" || password != "password") return Results.Unauthorized();
    var claims = new List<Claim> { new(ClaimTypes.Name, userName), new(ClaimTypes.Role, "admin") };
    var tokenDescriptor = new SecurityTokenDescriptor() {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(30),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
    };
    var handler = new JwtSecurityTokenHandler();
    return Results.Ok(new { token = handler.WriteToken(handler.CreateToken(tokenDescriptor)) });
});

// --- SECURE ROUTE VERSIONS ---

// VERSION 1: The Basic Secure Route
// BEHAVIOR CHANGE: Because we set a DefaultScheme (Cookies), this route 
// will now automatically require a valid Cookie.
app.MapGet("/secure", () => "Basic Secure Route (Defaults to Cookie)").RequireAuthorization();

// VERSION 2: Cookie ONLY (Redundant now, but explicitly stated)
app.MapGet("/secure-cookie", () => "Access via Cookie ONLY")
    .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme });

// VERSION 3: JWT ONLY
// Even though Cookie is default, we can OVERRIDE it by explicitly asking for JWT.
app.MapGet("/secure-jwt", () => "Access via JWT ONLY")
    .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme });

// VERSION 4: BOTH (Multi-Scheme)
// We still list both if we want to accept EITHER. The default only applies 
// if we don't list any schemes.
app.MapGet("/secure-both", (HttpContext context) => {
    var type = context.User.Identity?.AuthenticationType;
    return $"Access via {type}! Both schemes are supported here.";
}).RequireAuthorization(new AuthorizeAttribute { 
    AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}" 
});

app.Run();
