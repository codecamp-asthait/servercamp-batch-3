using System.Text;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Dukaan.Infrastructure.Services;
using Dukaan.Infrastructure.Data.Model;
using Dukaan.Infrastructure.Data.Services;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Service Registration Section ---
// This is where we register dependencies for the built-in Dependency Injection (DI) container.

// Register the Database Context with PostgreSQL support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register ASP.NET Core Identity for authentication
builder.Services.AddIdentity<Merchant, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Default authentication scheme and JWT authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Register application-specific services and repositories
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Registers the generic repository
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register OpenAPI (Swagger) for API documentation
builder.Services.AddOpenApi();

// Register MVC controllers
builder.Services.AddControllers();

var app = builder.Build();

// --- 2. Middleware Pipeline Section ---
// This defines the order in which HTTP requests are processed.

if (app.Environment.IsDevelopment())
{
    // Enables the interactive Swagger UI in development mode
    app.MapOpenApi();
}

// Redirects HTTP requests to HTTPS
app.UseHttpsRedirection();

// Reads the incoming request, validates the authentication token (like JWT or cookie), and sets the user identity
app.UseAuthentication();

// Maps controller routes (e.g., [Route("api/[controller]")])
app.MapControllers();

// Starts the application
app.Run();