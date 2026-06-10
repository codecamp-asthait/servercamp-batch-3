# [plan-8] JWT Authentication Implementation Plan

Status: Completed

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement JWT authentication with ASP.NET Core Identity and Swagger integration.

**Architecture:** N-Tier with `AuthService` handling token logic, `AuthController` handling requests via DTOs, and JWT Bearer security configured for Swagger.

**Tech Stack:** ASP.NET Core Identity, JWT Bearer Authentication, Swashbuckle

---

### Task 1: Add NuGet Packages

**Files:**
- Modify: `dukaan.Host/dukaan.Host.csproj`

- [x] **Step 1: Add authentication packages**

Run:
```bash
dotnet add dukaan.Host/dukaan.Host.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
```

- [x] **Step 2: Commit**

```bash
git add dukaan.Host/dukaan.Host.csproj
git commit -m "feat: add jwt bearer package"
```

### Task 2: Implement Auth DTOs

**Files:**
- Create: `dukaan.Application/DTOs/AuthDTOs.cs`

- [x] **Step 1: Write Auth DTOs**

```csharp
namespace dukaan.Application.DTOs;

public record LoginRequestDTO(string Email, string Password);
public record RegisterRequestDTO(string Email, string Password, string StoreName); // Extend as needed
public record AuthResponseDTO(string Token, string Email);
```

- [x] **Step 2: Commit**

```bash
git add dukaan.Application/DTOs/AuthDTOs.cs
git commit -m "feat: add auth dtos"
```

### Task 3: Implement Auth Service

**Files:**
- Create: `dukaan.Application/Services/IAuthService.cs`
- Create: `dukaan.Application/Services/AuthService.cs`

- [x] **Step 1: Implement IAuthService**

```csharp
using dukaan.Application.DTOs;

namespace dukaan.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request);
}
```

- [x] **Step 2: Implement AuthService (token logic with TenantId claim)**

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dukaan.Application.DTOs;
using dukaan.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace dukaan.Application.Services;

public class AuthService(UserManager<Merchant> userManager, IConfiguration config) : IAuthService
{
    public async Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new("tenant_id", user.TenantId.ToString()) // Multi-tenant claim
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthResponseDTO(tokenHandler.WriteToken(token), user.Email!);
    }
}
```

- [x] **Step 3: Commit**

```bash
git add dukaan.Application/Services/*
git commit -m "feat: implement auth service with jwt"
```

### Task 4: Configure Authentication and Swagger

**Files:**
- Modify: `dukaan.Host/appsettings.json`
- Modify: `dukaan.Host/Program.cs`

- [x] **Step 1: Add JWT settings to appsettings.json**

```json
{
  "Jwt": {
    "Key": "ThisIsASecretKeyMustBeAtLeast16CharactersLong"
  }
}
```

- [x] **Step 2: Register IAuthService and configure Auth/Swagger in Program.cs**

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using dukaan.Application.Services;

// ... Auth Service Registration
builder.Services.AddScoped<IAuthService, AuthService>();

// ... Auth config
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// ... Swagger Auth config
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});
```

- [x] **Step 3: Commit**

```bash
git add dukaan.Host/appsettings.json dukaan.Host/Program.cs
git commit -m "feat: configure jwt settings and register auth service"
```

### Task 5: Auth Controller

**Files:**
- Create: `dukaan.Host/Controllers/AuthController.cs`

- [x] **Step 1: Implement Auth Controller**

```csharp
using dukaan.Application.DTOs;
using dukaan.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace dukaan.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginRequestDTO request)
    {
        var response = await authService.LoginAsync(request);
        return response == null ? Unauthorized() : Ok(response);
    }
}
```

- [x] **Step 2: Commit**

```bash
git add dukaan.Host/Controllers/AuthController.cs
git commit -m "feat: add auth controller"
```
