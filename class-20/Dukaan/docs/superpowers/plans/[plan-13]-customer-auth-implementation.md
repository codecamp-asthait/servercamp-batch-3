# Customer Authentication Flow Implementation Plan

**Status:** Complete
**Spec:** `[spec-6]-customer-auth-design.md`

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [x]`) syntax for tracking.

**Goal:** Implement customer registration and login scoped to a tenant, resolved via `X-Tenant-Slug` header. Extends `TenantProvider` with cached header fallback. No new JWT scheme — uses `CustomerOnly` authorization policy.

---

### Task 1: Extend TenantProvider with Header Fallback

**Files:**
- Modify: `dukaan.Infrastructure/Services/TenantProvider.cs`
- Modify: `dukaan.Host/Program.cs` (register `IMemoryCache`)

- [x] **Step 1: Add cached slug resolution to TenantProvider**

  Inject `IDistributedCache` and `ApplicationDbContext` alongside `IHttpContextAccessor`. Add header fallback after JWT claim check:

  ```csharp
  using Microsoft.AspNetCore.Http;
  using Microsoft.Extensions.Caching.Distributed;
  using Microsoft.EntityFrameworkCore;
  using System.Text;
  using dukaan.Domain.Interfaces;
  using dukaan.Infrastructure.Data;

  namespace dukaan.Infrastructure.Services;

  public class TenantProvider(
      IHttpContextAccessor httpContextAccessor,
      ApplicationDbContext context,
      IDistributedCache cache) : ITenantProvider
  {
      public Guid? GetTenantId()
      {
          var tenantIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
          if (Guid.TryParse(tenantIdClaim, out var tenantId)) return tenantId;

          var slug = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Slug"].FirstOrDefault();
          if (slug is null) return null;

          var cacheKey = $"tenant_slug:{slug}";
          var cached = cache.GetString(cacheKey);
          if (cached is not null) return Guid.Parse(cached);

          var id = context.Tenants
              .Where(t => t.Slug == slug)
              .Select(t => (Guid?)t.Id)
              .FirstOrDefault();

          if (id is not null)
              cache.SetString(cacheKey, id.ToString()!, new DistributedCacheEntryOptions
              {
                  SlidingExpiration = TimeSpan.FromMinutes(5)
              });

          return id;
      }
  }
  ```

- [x] **Step 2: Register IDistributedCache (in-memory provider) in Program.cs**

  ```csharp
  builder.Services.AddDistributedMemoryCache(); // swap to AddStackExchangeRedisCache() when ready
  ```

- [x] **Step 3: Build and verify no errors**
  ```bash
  dotnet build dukaan.sln -q
  ```

- [x] **Step 4: Commit**
  ```bash
  git add dukaan.Infrastructure/Services/TenantProvider.cs dukaan.Host/Program.cs
  git commit -m "feat: extend TenantProvider with X-Tenant-Slug header fallback and caching"
  ```

---

### Task 2: DTOs

**Files:**
- Create: `dukaan.Application/DTOs/CustomerDTOs.cs`

- [x] **Step 1: Create CustomerDTOs**

  ```csharp
  namespace dukaan.Application.DTOs;

  public record CustomerRegisterRequest(
      string Email,
      string Password,
      string FirstName,
      string LastName,
      string Phone
  );

  public record CustomerLoginRequest(string Email, string Password);

  public record CustomerAuthResponse(string Token, string Email, Guid CustomerId);
  ```

- [x] **Step 2: Commit**
  ```bash
  git add dukaan.Application/DTOs/CustomerDTOs.cs
  git commit -m "feat: add customer auth DTOs"
  ```

---

### Task 3: ICustomerService Interface

**Files:**
- Create: `dukaan.Application/Services/ICustomerService.cs`

- [x] **Step 1: Create ICustomerService**

  ```csharp
  using dukaan.Application.DTOs;

  namespace dukaan.Application.Services;

  public interface ICustomerService
  {
      Task<CustomerAuthResponse> RegisterAsync(CustomerRegisterRequest request, Guid tenantId);
      Task<CustomerAuthResponse?> LoginAsync(CustomerLoginRequest request, Guid tenantId);
  }
  ```

- [x] **Step 2: Commit**
  ```bash
  git add dukaan.Application/Services/ICustomerService.cs
  git commit -m "feat: add ICustomerService interface"
  ```

---

### Task 4: CustomerService Implementation

**Files:**
- Create: `dukaan.Infrastructure/Identity/Services/CustomerService.cs`

- [x] **Step 1: Implement CustomerService**

  ```csharp
  using System.Text;
  using System.Security.Claims;
  using dukaan.Application.DTOs;
  using dukaan.Application.Services;
  using dukaan.Domain.Entities;
  using dukaan.Domain.Interfaces;
  using dukaan.Infrastructure.Identity.Models;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.Extensions.Configuration;
  using Microsoft.IdentityModel.Tokens;
  using System.IdentityModel.Tokens.Jwt;

  namespace dukaan.Infrastructure.Identity.Services;

  public class CustomerService(
      UserManager<ApplicationUser> userManager,
      IRepository<Customer> customerRepository,
      IConfiguration config) : ICustomerService
  {
      public async Task<CustomerAuthResponse> RegisterAsync(CustomerRegisterRequest request, Guid tenantId)
      {
          var existing = await userManager.FindByEmailAsync(request.Email);
          if (existing != null && existing.TenantId == tenantId)
              throw new InvalidOperationException("Email already registered in this store.");

          await customerRepository.BeginTransactionAsync();
          try
          {
              var user = new ApplicationUser
              {
                  UserName = request.Email,
                  Email = request.Email,
                  PhoneNumber = request.Phone,
                  TenantId = tenantId,
                  UserType = UserType.Customer
              };

              var result = await userManager.CreateAsync(user, request.Password);
              if (!result.Succeeded)
                  throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

              var customer = new Customer
              {
                  ApplicationUserId = user.Id,
                  TenantId = tenantId,
                  FirstName = request.FirstName,
                  LastName = request.LastName,
                  Phone = request.Phone
              };

              await customerRepository.AddAsync(customer);
              await customerRepository.SaveChangesAsync();
              await customerRepository.CommitTransactionAsync();

              return new CustomerAuthResponse(GenerateToken(user), user.Email!, customer.Id);
          }
          catch
          {
              await customerRepository.RollbackTransactionAsync();
              throw;
          }
      }

      public async Task<CustomerAuthResponse?> LoginAsync(CustomerLoginRequest request, Guid tenantId)
      {
          var user = await userManager.FindByEmailAsync(request.Email);
          if (user == null || user.TenantId != tenantId || user.UserType != UserType.Customer)
              return null;

          if (!await userManager.CheckPasswordAsync(user, request.Password))
              return null;

          var customer = customerRepository.FindAsync(c => c.ApplicationUserId == user.Id).Result.FirstOrDefault();
          return new CustomerAuthResponse(GenerateToken(user), user.Email!, customer?.Id ?? Guid.Empty);
      }

      private string GenerateToken(ApplicationUser user)
      {
          var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
          var claims = new List<Claim>
          {
              new(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new(ClaimTypes.Email, user.Email!),
              new("user_type", user.UserType.ToString()),
              new("tenant_id", user.TenantId.ToString())
          };
          var tokenDescriptor = new SecurityTokenDescriptor
          {
              Subject = new ClaimsIdentity(claims),
              Expires = DateTime.UtcNow.AddDays(30),
              SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
          };
          var handler = new JwtSecurityTokenHandler();
          return handler.WriteToken(handler.CreateToken(tokenDescriptor));
      }
  }
  ```

  > Note: Customer tokens use 30-day expiry (vs 7-day for merchants) — adjust if needed.

- [x] **Step 2: Commit**
  ```bash
  git add dukaan.Infrastructure/Identity/Services/CustomerService.cs
  git commit -m "feat: implement CustomerService with register and login"
  ```

---

### Task 5: CustomersController

**Files:**
- Create: `dukaan.Host/Controllers/CustomersController.cs`

- [x] **Step 1: Implement CustomersController**

  ```csharp
  using dukaan.Application.DTOs;
  using dukaan.Application.Services;
  using dukaan.Domain.Interfaces;
  using Microsoft.AspNetCore.Mvc;

  namespace dukaan.Host.Controllers;

  [ApiController]
  [Route("api/[controller]")]
  public class CustomersController(ICustomerService customerService, ITenantProvider tenantProvider) : ControllerBase
  {
      [HttpPost("register")]
      public async Task<ActionResult<CustomerAuthResponse>> Register(CustomerRegisterRequest request)
      {
          var tenantId = tenantProvider.GetTenantId();
          if (tenantId is null) return NotFound("Store not found.");

          try
          {
              var response = await customerService.RegisterAsync(request, tenantId.Value);
              return CreatedAtAction(null, response);
          }
          catch (InvalidOperationException ex) when (ex.Message.Contains("already registered"))
          {
              return Conflict(ex.Message);
          }
      }

      [HttpPost("login")]
      public async Task<ActionResult<CustomerAuthResponse>> Login(CustomerLoginRequest request)
      {
          var tenantId = tenantProvider.GetTenantId();
          if (tenantId is null) return NotFound("Store not found.");

          var response = await customerService.LoginAsync(request, tenantId.Value);
          return response is null ? Unauthorized() : Ok(response);
      }
  }
  ```

- [x] **Step 2: Commit**
  ```bash
  git add dukaan.Host/Controllers/CustomersController.cs
  git commit -m "feat: add CustomersController with register and login endpoints"
  ```

---

### Task 6: Wire Up in Program.cs

**Files:**
- Modify: `dukaan.Host/Program.cs`

- [x] **Step 1: Register CustomerService and CustomerOnly policy**

  Add after existing service registrations:
  ```csharp
  builder.Services.AddScoped<ICustomerService, CustomerService>();
  ```

  Update `AddAuthorization`:
  ```csharp
  builder.Services.AddAuthorization(options =>
  {
      options.AddPolicy("CustomerOnly", policy =>
          policy.RequireClaim("user_type", "Customer"));
  });
  ```

- [x] **Step 2: Build and verify**
  ```bash
  dotnet build dukaan.sln -q
  ```

- [x] **Step 3: Commit**
  ```bash
  git add dukaan.Host/Program.cs
  git commit -m "feat: register CustomerService and add CustomerOnly authorization policy"
  ```

---

### Task 7: Integration Tests

**Files:**
- Modify: `dukaan.Tests.Integration/` (existing test project)

- [x] **Step 1: Write integration tests**

  Cover:
  - `POST /api/Customers/register` with valid `X-Tenant-Slug` → `201` + JWT
  - Same email, different tenant → `201`
  - Same email, same tenant → `409`
  - `POST /api/Customers/login` with correct credentials → `200` + JWT with `user_type=Customer`
  - Login with wrong password → `401`
  - Login without `X-Tenant-Slug` header → `404`
  - Merchant token on `[Authorize(Policy = "CustomerOnly")]` endpoint → `403`

- [x] **Step 2: Run tests**
  ```bash
  dotnet test dukaan.Tests.Integration -q
  ```

- [x] **Step 3: Commit**
  ```bash
  git add dukaan.Tests.Integration/
  git commit -m "test: add customer auth integration tests"
  ```
