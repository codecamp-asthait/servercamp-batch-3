using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// 1. Initialize the WebApplication builder.
var builder = WebApplication.CreateBuilder(args);

// Configure the Database Context (ApplicationDbContext).
// We are using PostgreSQL as our database provider.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Define the connection string (Host, Port, DB Name, User, Password).
    options.UseNpgsql("Host=localhost;Port=5432;Database=identity_db;Username=postgres;Password=password");
});

// Configure ASP.NET Core Identity.
// Identity provides APIs for user management (create, delete, roles, etc.).
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Educational: Weakening password requirements to make testing easier for students.
    // In production, these should be set to 'true' and higher lengths for security.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
})
    // Tell Identity to use our ApplicationDbContext to store user data.
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // Add default token providers (used for features like password reset or email confirmation).
    .AddDefaultTokenProviders();


// Define a secret key used for signing and verifying JWT tokens.// In a real application, this should be stored securely (e.g., Environment Variables or Key Vault).
var key = "a-very-long-and-secure-secret-key-at-least-32-chars"u8.ToArray();

// 2. Configure Authentication services.
// We are using JWT (JSON Web Token) Bearer authentication.
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        // Define how the application should validate the incoming token.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Ensure the token was issued by a trusted server.
            ValidateAudience = true, // Ensure the token is intended for this application.
            ValidateLifetime = true, // Ensure the token has not expired.
            ValidateIssuerSigningKey = true, // Ensure the token signature matches our secret key.
            ValidIssuer = "learning-identity-framework",
            ValidAudience = "learning-identity-framework",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// 3. Configure Authorization services.
// We define 'Policies' which are sets of requirements that a user must meet to access certain resources.
builder.Services.AddAuthorization(options =>
{
    // Define a policy named "admin-policy".
    // To satisfy this policy, a user must have a claim "org" with the value "ait" AND have the "admin" role.
    options.AddPolicy("admin-policy", policy =>
    {
        policy.RequireClaim("org", "ait");
        policy.RequireRole("admin");
    });
});

var app = builder.Build();

// 4. Register Authentication and Authorization middleware.
// Order matters: Authentication must come BEFORE Authorization.
// UseAuthentication: Identifies who the user is based on the token.
app.UseAuthentication();
// UseAuthorization: Checks if the identified user has permission to access the resource.
app.UseAuthorization();

// Endpoint to register a new user using ASP.NET Core Identity.
app.MapPost("/register", async (
    string email,
    string password,
    UserManager<IdentityUser> userManager
) =>
{
    // Create a new IdentityUser object.
    var user = new IdentityUser
    {
        UserName = email,
        Email = email
    };

    // Use UserManager to create the user in the database and hash the password.
    var result = await userManager.CreateAsync(user, password);

    // Return errors if creation fails (e.g., duplicate email), otherwise return success.
    return !result.Succeeded ? Results.BadRequest(result.Errors) : Results.Ok("User Created");
});

// Endpoint to login and receive a JWT token.
// Now verifies credentials against the database.
app.MapGet("/login", async (
    string email,
    string password,
    UserManager<IdentityUser> userManager) =>
{
    // Basic validation.
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        return Results.Unauthorized();

    // 1. Find the user by their email in the database.
    var user = await userManager.FindByEmailAsync(email);
    if (user is null) return Results.Unauthorized();

    // 2. Verify if the provided password matches the hashed password in the database.
    var isPasswordValid = await userManager.CheckPasswordAsync(user, password);
    if (!isPasswordValid) return Results.Unauthorized();


    // Educational Logic: Assign an organization claim based on the email domain.
    var organization = email switch
    {
        var e when e.EndsWith("@ait.com") => "ait",
        var e when e.EndsWith("@optimizely.com") => "optimizely",
        var e when e.EndsWith("@fieldnation.com") => "fieldnation",
        _ => "unknown" // Added a default case for safety
    };

    // Educational Logic: Assign a role based on a specific email.
    var role = email switch
    {
        var e when e.EndsWith("shafayet@ait.com") => "admin",
        _ => "user"
    };

    // 5. Generate the JWT Token.
    // The TokenDescriptor describes what goes into the token (Claims, Expiration, Signing Key).
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        // Claims are pieces of information about the user (e.g., Email, Role, Organization).
        Subject = new ClaimsIdentity([
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("org", organization),
            new Claim(ClaimTypes.Role, role),
        ]),

        Expires = DateTime.UtcNow.AddMinutes(30), // Token will expire in 30 minutes.
        Issuer = "learning-identity-framework",
        Audience = "learning-identity-framework",

        // Sign the token using our secret key and the HMAC SHA256 algorithm.
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        )
    };

    // Create and serialize the token into a string format.
    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var jwtToken = tokenHandler.WriteToken(token);

    // Return the token to the client.
    return Results.Ok(new { token = jwtToken });
});

// Endpoint protected by an inline authorization policy.
// User MUST have the "org" claim with value "ait".
app.MapGet("/ait-resources", () => { return Results.Ok("You accessed AIT resources"); })
    .RequireAuthorization(policy => { policy.RequireClaim("org", "ait"); });

// Endpoint protected by a named policy ("admin-policy").
// This policy was defined in the services configuration above.
app.MapGet("/ait-admin-resources", () => Results.Ok("You accessed AIT resources"))
    .RequireAuthorization("admin-policy");

// Another endpoint using the same named "admin-policy".
app.MapGet("/ait-partial-ceo-resources", () => Results.Ok("You accessed AIT resources"))
    .RequireAuthorization("admin-policy");

// Endpoint protected by an inline policy for the "optimizely" organization.
app.MapGet("/optimizely-resources", () => Results.Ok("You accessed Optimizely resources"))
    .RequireAuthorization(policy => { policy.RequireClaim("org", "optimizely"); });

// Endpoint protected by an inline policy for the "fieldnation" organization.
app.MapGet("/fieldnation-resources", () => Results.Ok("You accessed FieldNation resources"))
    .RequireAuthorization(policy => { policy.RequireClaim("org", "fieldnation"); });

app.Run();