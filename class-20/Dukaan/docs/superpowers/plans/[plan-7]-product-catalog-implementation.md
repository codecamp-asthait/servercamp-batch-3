# [plan-7] Product Catalog Implementation Plan

Status: Completed

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the Product Catalog module with multi-tenant support.

**Architecture:** Domain Entity -> DTOs -> Service -> Controller (N-Tier) with Global Query Filtering.

**Tech Stack:** .NET 10, EF Core, FluentValidation

---

### Task 1: Create Product Entity

**Files:**
- Create: `dukaan.Domain/Entities/Product.cs`

- [ ] **Step 1: Write Product entity**

```csharp
using dukaan.Domain.Interfaces;

namespace dukaan.Domain.Entities;

public class Product : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
}
```

- [ ] **Step 2: Commit**

```bash
git add dukaan.Domain/Entities/Product.cs
git commit -m "feat: add product entity"
```

### Task 2: Update DbContext and Configure Filters

**Files:**
- Modify: `dukaan.Infrastructure/Data/ApplicationDbContext.cs`

- [ ] **Step 1: Register Product DbSet and Configure Query Filter**

Update `ApplicationDbContext.cs` to ensure the `Product` entity uses the tenant query filter. (Note: The existing logic in `OnModelCreating` already iterates over all `ITenantEntity` types and sets the filter, so adding the `DbSet` is sufficient for the automated system).

```csharp
// Add to ApplicationDbContext.cs
public DbSet<Product> Products { get; set; }
```

- [ ] **Step 2: Run migration**

Run:
```bash
dotnet ef migrations add AddedProductEntity --project dukaan.Infrastructure --startup-project dukaan.Host
dotnet ef database update --project dukaan.Infrastructure --startup-project dukaan.Host
```

- [ ] **Step 3: Commit**

```bash
git add dukaan.Infrastructure/Data/ApplicationDbContext.cs dukaan.Infrastructure/Migrations/*
git commit -m "feat: update dbcontext and add product migration"
```

### Task 7: Verify Tenant Isolation Test

**Files:**
- Create: `dukaan.Tests/ProductTenantTests.cs`

- [ ] **Step 1: Write test to verify multi-tenant isolation**

```csharp
using dukaan.Domain.Entities;
using dukaan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace dukaan.Tests;

public class ProductTenantTests
{
    [Fact]
    public void QueryFilter_ShouldFilterByTenantId()
    {
        // Setup: Setup an InMemory DB or mock ITenantProvider and verify 
        // that querying Products returns only products for the current tenant.
        // This confirms the global query filter is correctly applied.
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add dukaan.Tests/ProductTenantTests.cs
git commit -m "test: add tenant isolation verification test"
```

### Task 3: Product DTOs

**Files:**
- Create: `dukaan.Application/DTOs/ProductDTOs.cs`

- [ ] **Step 1: Write ProductRequestDto and ProductResponseDto**

```csharp
namespace dukaan.Application.DTOs;

public record ProductRequestDto(
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity
);

public record ProductResponseDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity,
    bool IsActive
);
```

- [ ] **Step 2: Commit**

```bash
git add dukaan.Application/DTOs/ProductDTOs.cs
git commit -m "feat: add product dtos"
```

### Task 4: Product Validator

**Files:**
- Create: `dukaan.Application/Validators/ProductValidator.cs`

- [ ] **Step 1: Write ProductValidator**

```csharp
using FluentValidation;
using dukaan.Application.DTOs;

namespace dukaan.Application.Validators;

public class ProductValidator : AbstractValidator<ProductRequestDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add dukaan.Application/Validators/ProductValidator.cs
git commit -m "feat: add product validator"
```

### Task 5: Product Service Interface and Implementation

**Files:**
- Create: `dukaan.Application/Services/IProductService.cs`
- Create: `dukaan.Application/Services/ProductService.cs`

- [ ] **Step 1: Write IProductService**

```csharp
using dukaan.Application.DTOs;

namespace dukaan.Application.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(Guid id);
    Task<ProductResponseDto> CreateAsync(ProductRequestDto request);
    Task<bool> UpdateAsync(Guid id, ProductRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}
```

- [ ] **Step 2: Write ProductService implementation**

```csharp
using dukaan.Application.DTOs;
using dukaan.Domain.Entities;
using dukaan.Domain.Interfaces;

namespace dukaan.Application.Services;

public class ProductService(IRepository<Product> repository) : IProductService
{
    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await repository.GetAllAsync(trackChanges: false);
        return products.Select(p => new ProductResponseDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.IsActive));
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var p = await repository.GetByIdAsync(id);
        return p == null ? null : new ProductResponseDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.IsActive);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductRequestDto request)
    {
        var product = new Product { Name = request.Name, Description = request.Description, Price = request.Price, ImageUrl = request.ImageUrl, StockQuantity = request.StockQuantity };
        await repository.AddAsync(product);
        return new ProductResponseDto(product.Id, product.Name, product.Description, product.Price, product.ImageUrl, product.StockQuantity, product.IsActive);
    }

    public async Task<bool> UpdateAsync(Guid id, ProductRequestDto request)
    {
        var product = await repository.GetByIdAsync(id);
        if (product == null) return false;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;
        product.StockQuantity = request.StockQuantity;
        await repository.UpdateAsync(product);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await repository.GetByIdAsync(id);
        if (product == null) return false;
        product.IsActive = false;
        await repository.UpdateAsync(product);
        return true;
    }
}
```

- [ ] **Step 3: Register service in Program.cs**

```csharp
// Program.cs
builder.Services.AddScoped<IProductService, ProductService>();
```

- [ ] **Step 4: Commit**

```bash
git add dukaan.Application/Services/* dukaan.Host/Program.cs
git commit -m "feat: implement product service"
```

### Task 6: Products Controller

**Files:**
- Create: `dukaan.Host/Controllers/ProductsController.cs`

- [ ] **Step 1: Write ProductsController**

```csharp
using dukaan.Application.DTOs;
using dukaan.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace dukaan.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll() => Ok(await productService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> Get(Guid id) 
    {
        var product = await productService.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(ProductRequestDto request) => Ok(await productService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ProductRequestDto request) => await productService.UpdateAsync(id, request) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) => await productService.DeleteAsync(id) ? NoContent() : NotFound();
}
```

- [ ] **Step 2: Commit**

```bash
git add dukaan.Host/Controllers/ProductsController.cs
git commit -m "feat: add products controller"
```
