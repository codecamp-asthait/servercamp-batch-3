# [plan-9] Pagination and Structured Responses Implementation Plan

Status: Completed

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement a centralized pagination system for requests and responses, and update the Product Catalog to use it.

**Architecture:** Create generic `PaginationRequest` and `PagedResponse<T>` models, implement a Repository-level pagination helper, and update the Product module.

**Tech Stack:** .NET 10, EF Core, FluentValidation

---

### Task 1: Create Pagination Models

**Files:**
- Create: `dukaan.Application/Common/Models/Pagination.cs`

- [x] **Step 1: Write PaginationRequest and PagedResponse models**

```csharp
namespace dukaan.Application.Common.Models;

public record PaginationRequest(int PageNumber = 1, int PageSize = 10);

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

- [x] **Step 2: Commit**

```bash
git add dukaan.Application/Common/Models/Pagination.cs
git commit -m "feat: add central pagination models"
```

### Task 2: Update Repository to support Pagination

**Files:**
- Modify: `dukaan.Domain/Interfaces/IRepository.cs`
- Modify: `dukaan.Infrastructure/Data/Repositories/Repository.cs`

- [x] **Step 1: Add GetPagedAsync to IRepository**

```csharp
// dukaan.Domain/Interfaces/IRepository.cs
// Add to interface:
Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, bool trackChanges = false);
```

- [x] **Step 2: Implement GetPagedAsync in Repository**

```csharp
// dukaan.Infrastructure/Data/Repositories/Repository.cs
public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, bool trackChanges = false)
{
    var count = await _dbSet.CountAsync();
    var items = trackChanges 
        ? await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
        : await _dbSet.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    
    return (items, count);
}
```

- [x] **Step 3: Commit**

```bash
git add dukaan.Domain/Interfaces/IRepository.cs dukaan.Infrastructure/Data/Repositories/Repository.cs
git commit -m "feat: add pagination support to generic repository"
```

### Task 3: Pagination Validator

**Files:**
- Create: `dukaan.Application/Validators/PaginationValidator.cs`

- [x] **Step 1: Write PaginationValidator**

```csharp
using FluentValidation;
using dukaan.Application.Common.Models;

namespace dukaan.Application.Validators;

public class PaginationValidator : AbstractValidator<PaginationRequest>
{
    public PaginationValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
```

- [x] **Step 2: Commit**

```bash
git add dukaan.Application/Validators/PaginationValidator.cs
git commit -m "feat: add pagination validator"
```

### Task 4: Update Product Service for Pagination

**Files:**
- Modify: `dukaan.Application/Services/IProductService.cs`
- Modify: `dukaan.Application/Services/ProductService.cs`

- [x] **Step 1: Update IProductService.GetAllAsync**

```csharp
using dukaan.Application.Common.Models;
// ...
Task<PagedResponse<ProductResponseDto>> GetAllAsync(PaginationRequest request);
```

- [x] **Step 2: Update ProductService implementation**

```csharp
// dukaan.Application/Services/ProductService.cs
public async Task<PagedResponse<ProductResponseDto>> GetAllAsync(PaginationRequest request)
{
    var (items, totalCount) = await repository.GetPagedAsync(request.PageNumber, request.PageSize, trackChanges: false);
    
    var dtos = items.Select(p => new ProductResponseDto(
        p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.IsActive));

    return new PagedResponse<ProductResponseDto>(dtos, totalCount, request.PageNumber, request.PageSize);
}
```

- [x] **Step 3: Commit**

```bash
git add dukaan.Application/Services/IProductService.cs dukaan.Application/Services/ProductService.cs
git commit -m "feat: update product service to support pagination"
```

### Task 5: Update Products Controller

**Files:**
- Modify: `dukaan.Host/Controllers/ProductsController.cs`

- [x] **Step 1: Update GetAll endpoint**

```csharp
[HttpGet]
public async Task<ActionResult<PagedResponse<ProductResponseDto>>> GetAll([FromQuery] PaginationRequest request) 
    => Ok(await productService.GetAllAsync(request));
```

- [x] **Step 2: Commit**

```bash
git add dukaan.Host/Controllers/ProductsController.cs
git commit -m "feat: update products controller to use paginated response"
```

### Task 6: Verify Pagination with Integration Test

**Files:**
- Modify: `dukaan.Tests/ProductTenantTests.cs` (or create new)

- [x] **Step 1: Add pagination test case**

```csharp
[Fact]
public async Task GetAll_ShouldReturnPaginatedResults()
{
    // Arrange: Add 15 products for the same tenant
    // Act: Request page 1 with size 10, then page 2 with size 10
    // Assert: Page 1 has 10 items, Page 2 has 5 items. TotalCount is 15.
}
```

- [x] **Step 2: Run tests**

Run: `dotnet test`

- [x] **Step 3: Commit**

```bash
git add dukaan.Tests/ProductTenantTests.cs
git commit -m "test: add pagination verification test"
```
