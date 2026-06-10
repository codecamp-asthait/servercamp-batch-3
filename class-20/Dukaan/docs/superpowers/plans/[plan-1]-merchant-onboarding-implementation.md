# Merchant Onboarding Implementation Plan

Status: Completed

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the foundation for multi-tenancy and the merchant registration flow using ASP.NET Core Identity, PostgreSQL, and a Generic Repository pattern.

**Architecture:** N-Tier architecture with a shared database (Shared Table) approach. Data access is abstracted through a Generic Repository. Multi-tenancy is handled via EF Core Global Query Filters.

**Tech Stack:** .NET 10, Entity Framework Core, PostgreSQL (Npgsql), ASP.NET Core Identity, JWT Bearer Authentication.

---

### Task 1: Core Models & Interface Setup

**Files:**
- Create: `Models/Entities/Tenant.cs`
- Create: `Models/Entities/Merchant.cs`
- Create: `Models/Entities/StoreBranding.cs`
- Create: `Models/Interfaces/ITenantEntity.cs`

- [ ] **Step 1: Create `ITenantEntity`**
```csharp
namespace dukaan.Models.Interfaces;
public interface ITenantEntity { Guid TenantId { get; set; } }
```

- [ ] **Step 2: Create Entities**

`Models/Entities/Tenant.cs`:
```csharp
namespace dukaan.Models.Entities;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StoreName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Country { get; set; } = "India";
    public string Currency { get; set; } = "INR";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

`Models/Entities/Merchant.cs`:
```csharp
using Microsoft.AspNetCore.Identity;
using dukaan.Models.Interfaces;

namespace dukaan.Models.Entities;

public class Merchant : IdentityUser, ITenantEntity
{
    public Guid TenantId { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
```

`Models/Entities/StoreBranding.cs`:
```csharp
using dukaan.Models.Interfaces;

namespace dukaan.Models.Entities;

public class StoreBranding : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = "https://placehold.co/200x200?text=Logo";
    public string ThemeColor { get; set; } = "#000000";
}
```

- [ ] **Step 3: Commit**
```bash
git add Models/ && git commit -m "feat: core entities and ITenantEntity"
```

---

### Task 2: Data Infrastructure & Generic Repository

**Files:**
- Create: `Data/ApplicationDbContext.cs`
- Create: `Data/Repositories/IRepository.cs`
- Create: `Data/Repositories/Repository.cs`
- Create: `Infrastructure/ITenantProvider.cs`
- Create: `Infrastructure/TenantProvider.cs`

- [ ] **Step 1: Create `IRepository<T>`**
```csharp
using System.Linq.Expressions;
namespace dukaan.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, bool trackChanges = false);
    Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task SaveChangesAsync();
}
```

- [ ] **Step 2: Create `Repository<T>` Implementation**
```csharp
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace dukaan.Data.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, bool trackChanges = false) =>
        trackChanges ? await _dbSet.FindAsync(id) : await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);

    public async Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false) =>
        trackChanges ? await _dbSet.ToListAsync() : await _dbSet.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false) =>
        trackChanges ? await _dbSet.Where(predicate).ToListAsync() : await _dbSet.Where(predicate).AsNoTracking().ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Remove(T entity) => _dbSet.Remove(entity);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
```

- [ ] **Step 3: Setup `ApplicationDbContext` with PostgreSQL & Filters**
```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using dukaan.Models.Entities;
using dukaan.Models.Interfaces;
using dukaan.Infrastructure;

namespace dukaan.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider) 
    : IdentityDbContext<Merchant>(options)
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<StoreBranding> StoreBrandings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType).HasQueryFilter(CreateFilterExpression(entityType.ClrType));
            }
        }
        builder.Entity<Tenant>().HasIndex(t => t.Slug).IsUnique();
    }

    private LambdaExpression CreateFilterExpression(Type type)
    {
        var parameter = Expression.Parameter(type, "e");
        var property = Expression.Property(parameter, "TenantId");
        var tenantIdValue = Expression.Call(Expression.Constant(tenantProvider), typeof(ITenantProvider).GetMethod("GetTenantId")!);
        return Expression.Lambda(Expression.Equal(property, tenantIdValue), parameter);
    }
}
```

- [ ] **Step 4: Commit**
```bash
git add Data/ Infrastructure/ && git commit -m "feat: repository pattern and postgres context"
```

---

### Task 3: Merchant Onboarding Service

**Files:**
- Create: `Services/ITenantService.cs`
- Create: `Services/TenantService.cs`

- [ ] **Step 1: Implement `TenantService` using Repositories**

```csharp
using dukaan.Data.Repositories;
using dukaan.Models.DTOs;
using dukaan.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace dukaan.Services;

public class TenantService(
    IRepository<Tenant> tenantRepository,
    IRepository<StoreBranding> brandingRepository,
    UserManager<Merchant> userManager) : ITenantService
{
    public async Task<bool> IsSlugAvailable(string slug)
    {
        var existing = await tenantRepository.FindAsync(t => t.Slug == slug.ToLower());
        return !existing.Any();
    }

    public async Task<RegisterResponse> RegisterMerchant(RegisterRequest request)
    {
        // Registration is a cross-repo operation. In a real app, we'd use a Unit of Work.
        // For the MVP, we'll implement the creation logic sequentially.
        
        var tenant = new Tenant
        {
            StoreName = request.StoreName,
            Slug = request.Slug.ToLower(),
            Category = request.Category,
            Country = request.Country
        };

        await tenantRepository.AddAsync(tenant);
        await tenantRepository.SaveChangesAsync();

        var merchant = new Merchant
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            TenantId = tenant.Id
        };

        var result = await userManager.CreateAsync(merchant, request.Password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var branding = new StoreBranding
        {
            TenantId = tenant.Id,
            StoreName = request.StoreName
        };

        await brandingRepository.AddAsync(branding);
        await brandingRepository.SaveChangesAsync();

        // TODO: Implement JWT Token generation in Task 5 (Future)
        return new RegisterResponse("dummy-token", tenant.Id, tenant.StoreName);
    }
}
```

- [ ] **Step 2: Commit**
```bash
git add Services/ && git commit -m "feat: onboarding service with repository pattern"
```

---

### Task 4: API Wiring & PostgreSQL Config

**Files:**
- Modify: `Program.cs`
- Modify: `appsettings.json`

- [ ] **Step 1: Update `Program.cs` for Npgsql & Repositories**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

- [ ] **Step 2: Commit**
```bash
git add Program.cs appsettings.json && git commit -m "chore: wire up postgres and generic repository"
```
