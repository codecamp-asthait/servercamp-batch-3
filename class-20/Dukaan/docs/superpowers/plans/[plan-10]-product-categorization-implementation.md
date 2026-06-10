# [plan-10] Product Categorization Implementation Plan

Status: Completed

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement a hierarchical product categorization system with many-to-many relationships and paginated responses.

**Architecture:** Use a `Category` entity with self-referencing hierarchy and an explicit `CategorizedProduct` pivot entity for multi-tenant many-to-many mapping. Service methods and controllers will use the project's central pagination system.

**Tech Stack:** .NET 10, EF Core (PostgreSQL), FluentValidation

---

### Task 1: Create Category and Pivot Entities

**Files:**
- Create: `dukaan.Domain/Entities/Category.cs`
- Create: `dukaan.Domain/Entities/CategorizedProduct.cs`
- Modify: `dukaan.Domain/Entities/Product.cs`

- [x] **Step 1: Create Category Entity**
```csharp
using dukaan.Domain.Interfaces;

namespace dukaan.Domain.Entities;

public class Category : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<CategorizedProduct> ProductLinks { get; set; } = new List<CategorizedProduct>();
}
```

- [x] **Step 2: Create CategorizedProduct Pivot Entity**
```csharp
using dukaan.Domain.Interfaces;

namespace dukaan.Domain.Entities;

public class CategorizedProduct : ITenantEntity
{
    public Guid ProductId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid TenantId { get; set; }

    public virtual Product Product { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
}
```

- [x] **Step 3: Update Product Entity**
```csharp
// In dukaan.Domain/Entities/Product.cs
public virtual ICollection<CategorizedProduct> ProductCategories { get; set; } = new List<CategorizedProduct>();
```

- [x] **Step 4: Commit**
```bash
git add dukaan.Domain/Entities/ && git commit -m "feat: add category and categorizedproduct entities"
```

---

### Task 2: Configure DB Context and Repositories

**Files:**
- Modify: `dukaan.Infrastructure/Data/ApplicationDbContext.cs`
- Modify: `dukaan.Domain/Interfaces/IRepository.cs`
- Modify: `dukaan.Infrastructure/Data/Repositories/Repository.cs`

- [x] **Step 1: Add DbSets and Configuration to ApplicationDbContext**
```csharp
public DbSet<Category> Categories { get; set; }
public DbSet<CategorizedProduct> CategorizedProducts { get; set; }

protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder); // Ensure multi-tenant filters are applied first
    
    builder.Entity<CategorizedProduct>()
        .HasKey(cp => new { cp.ProductId, cp.CategoryId });
        
    builder.Entity<Category>()
        .HasOne(c => c.ParentCategory)
        .WithMany(c => c.SubCategories)
        .HasForeignKey(c => c.ParentCategoryId)
        .OnDelete(DeleteBehavior.Restrict);
}
```

- [x] **Step 2: Add Overloaded GetPagedAsync to IRepository**
```csharp
// dukaan.Domain/Interfaces/IRepository.cs
Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
    Expression<Func<T, bool>> predicate, 
    int pageNumber, 
    int pageSize, 
    bool trackChanges = false);
```

- [x] **Step 3: Implement Overloaded GetPagedAsync in Repository**
```csharp
// dukaan.Infrastructure/Data/Repositories/Repository.cs
public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
    Expression<Func<T, bool>> predicate, 
    int pageNumber, 
    int pageSize, 
    bool trackChanges = false)
{
    var query = _dbSet.Where(predicate);
    var count = await query.CountAsync();
    var items = trackChanges 
        ? await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
        : await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    
    return (items, count);
}
```

- [x] **Step 4: Create and Apply Migration**
```bash
dotnet ef migrations add AddProductCategories --project dukaan.Infrastructure --startup-project dukaan.Host
dotnet ef database update --project dukaan.Infrastructure --startup-project dukaan.Host
```

- [x] **Step 5: Commit**
```bash
git add dukaan.Domain/ dukaan.Infrastructure/ && git commit -m "feat: configure repository and db for categories"
```

---

### Task 3: Category DTOs and Service

**Files:**
- Create: `dukaan.Application/DTOs/CategoryDTOs.cs`
- Create: `dukaan.Application/Services/ICategoryService.cs`
- Create: `dukaan.Application/Services/CategoryService.cs`

- [x] **Step 1: Define Category DTOs**
```csharp
namespace dukaan.Application.DTOs;
public record CategoryRequestDto(string Name, string? Description, Guid? ParentCategoryId);
public record CategoryResponseDto(Guid Id, string Name, string? Description, Guid? ParentCategoryId, List<CategoryResponseDto> SubCategories);
```

- [x] **Step 2: Implement CategoryService with Hierarchy and Safety Checks**
```csharp
public class CategoryService(IRepository<Category> repository) : ICategoryService
{
    public async Task<PagedResponse<CategoryResponseDto>> GetAllAsync(PaginationRequest request)
    {
        // Only paginate Root categories to maintain tree structure integrity
        var (items, totalCount) = await repository.GetPagedAsync(c => c.ParentCategoryId == null, request.PageNumber, request.PageSize);
        return new PagedResponse<CategoryResponseDto>(items.Select(MapToDto), totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parent = await repository.GetByIdAsync(request.ParentCategoryId.Value);
            if (parent == null) throw new Exception("Parent category not found.");
        }

        var category = new Category { Name = request.Name, Description = request.Description, ParentCategoryId = request.ParentCategoryId };
        await repository.AddAsync(category);
        await repository.SaveChangesAsync();
        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await repository.GetByIdAsync(id, trackChanges: true);
        if (category == null) return false;

        // Spec requirement: Prevent deletion if it has sub-categories or products
        if (category.SubCategories.Any(sc => sc.IsActive)) throw new Exception("Cannot delete category with active sub-categories.");
        if (category.ProductLinks.Any()) throw new Exception("Cannot delete category assigned to products.");

        category.IsActive = false;
        await repository.SaveChangesAsync();
        return true;
    }

    private CategoryResponseDto MapToDto(Category c) => new(c.Id, c.Name, c.Description, c.ParentCategoryId, c.SubCategories.Where(sc => sc.IsActive).Select(MapToDto).ToList());
}
```

- [x] **Step 3: Commit**
```bash
git add dukaan.Application/ && git commit -m "feat: implement category service with safety checks"
```

---

### Task 4: Update Product Service for Many-to-Many Sync

**Files:**
- Modify: `dukaan.Application/DTOs/ProductDTOs.cs`
- Modify: `dukaan.Application/Services/ProductService.cs`

- [x] **Step 1: Update Product DTOs**
```csharp
public record ProductRequestDto(string Name, string? Description, decimal Price, string? ImageUrl, int StockQuantity, List<Guid>? CategoryIds);
public record ProductResponseDto(Guid Id, string Name, string? Description, decimal Price, string? ImageUrl, int StockQuantity, bool IsActive, List<Guid> CategoryIds);
```

- [x] **Step 2: Update ProductService Sync Logic**
```csharp
// In ProductService.UpdateAsync:
product.ProductCategories.Clear();
if (request.CategoryIds != null)
{
    foreach (var catId in request.CategoryIds)
    {
        product.ProductCategories.Add(new CategorizedProduct { CategoryId = catId, ProductId = product.Id });
    }
}
// Note: Ensure ProductCategories is Included when fetching the product for update.
```

- [x] **Step 3: Commit**
```bash
git add dukaan.Application/ && git commit -m "feat: update product service for category syncing"
```

---

### Task 5: API Controllers and Verification

**Files:**
- Create: `dukaan.Host/Controllers/CategoriesController.cs`
- Modify: `dukaan.Host/Controllers/ProductsController.cs`
- Create: `dukaan.Tests/CategoryTests.cs`

- [x] **Step 1: Implement CategoriesController**
- [x] **Step 2: Update ProductsController**
- [x] **Step 3: Write and Run Integration Tests**
```bash
dotnet test
```
- [x] **Step 4: Commit**
```bash
git add dukaan.Host/ dukaan.Tests/ && git commit -m "feat: finalize category api and tests"
```
