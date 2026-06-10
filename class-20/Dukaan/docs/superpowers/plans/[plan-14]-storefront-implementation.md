# [plan-14] Storefront Implementation Plan

Status: Pending

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement a public-facing storefront for customers to browse a merchant's active products and categories, resolved by `x-tenant-slug` header. Simultaneously lock down merchant write endpoints behind JWT auth.

**Spec:** `docs/superpowers/specs/[spec-8]-storefront-design.md`

**Tech Stack:** .NET 10, EF Core, ASP.NET Core

---

### Task 1: Extend ITenantProvider with SetTenantId

**Files:**
- Modify: `dukaan.Domain/Interfaces/ITenantProvider.cs`
- Modify: `dukaan.Infrastructure/Services/TenantProvider.cs`

- [ ] **Step 1: Add SetTenantId to ITenantProvider**
```csharp
namespace dukaan.Domain.Interfaces;

public interface ITenantProvider
{
    Guid? GetTenantId();
    void SetTenantId(Guid tenantId);
}
```

- [ ] **Step 2: Implement in TenantProvider**

Fall back to `HttpContext.Items["TenantId"]` when no JWT claim is present.

```csharp
public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    private const string ItemsKey = "TenantId";

    public Guid? GetTenantId()
    {
        var claim = httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(claim, out var tenantId)) return tenantId;

        if (httpContextAccessor.HttpContext?.Items[ItemsKey] is Guid itemTenantId)
            return itemTenantId;

        return null;
    }

    public void SetTenantId(Guid tenantId) =>
        httpContextAccessor.HttpContext!.Items[ItemsKey] = tenantId;
}
```

- [ ] **Step 3: Write unit tests**

  - `GetTenantId_ReturnsTenantIdFromJwtClaim`
  - `GetTenantId_FallsBackToHttpContextItems_WhenNoJwtClaim`
  - `SetTenantId_StoresTenantIdInHttpContextItems`

- [ ] **Step 4: Commit**
```bash
git add dukaan.Domain/Interfaces/ITenantProvider.cs dukaan.Infrastructure/Services/TenantProvider.cs dukaan.Tests.Unit/ && git commit -m "feat: add SetTenantId to TenantProvider with HttpContext.Items fallback"
```

---

### Task 2: Add GetActiveAsync to ProductService

**Files:**
- Modify: `dukaan.Application/Services/IProductService.cs`
- Modify: `dukaan.Application/Services/ProductService.cs`

- [ ] **Step 1: Add to IProductService**
```csharp
Task<PagedResponse<ProductResponseDto>> GetActiveAsync(PaginationRequest request);
Task<PagedResponse<ProductResponseDto>> GetActiveByCategoryAsync(Guid categoryId, PaginationRequest request);
```

- [ ] **Step 2: Implement in ProductService**
```csharp
public async Task<PagedResponse<ProductResponseDto>> GetActiveAsync(PaginationRequest request)
{
    var (items, totalCount) = await repository.GetPagedAsync(
        p => p.IsActive,
        request.PageNumber, request.PageSize, trackChanges: false);

    return new PagedResponse<ProductResponseDto>(items.Select(MapToDto), totalCount, request.PageNumber, request.PageSize);
}

public async Task<PagedResponse<ProductResponseDto>> GetActiveByCategoryAsync(Guid categoryId, PaginationRequest request)
{
    var (items, totalCount) = await repository.GetPagedAsync(
        p => p.IsActive && p.ProductCategories.Any(pc => pc.CategoryId == categoryId),
        request.PageNumber, request.PageSize, trackChanges: false,
        p => p.ProductCategories);

    return new PagedResponse<ProductResponseDto>(items.Select(MapToDto), totalCount, request.PageNumber, request.PageSize);
}
```

- [ ] **Step 3: Write unit tests**

  - `GetActiveAsync_ReturnsOnlyActiveProducts`
  - `GetActiveByCategoryAsync_ReturnsOnlyActiveProductsInCategory`

- [ ] **Step 4: Commit**
```bash
git add dukaan.Application/ dukaan.Tests.Unit/ && git commit -m "feat: add GetActiveAsync and GetActiveByCategoryAsync to ProductService"
```

---

### Task 3: Add [Authorize] to Merchant Write Endpoints

**Files:**
- Modify: `dukaan.Host/Controllers/ProductsController.cs`
- Modify: `dukaan.Host/Controllers/CategoriesController.cs`

- [ ] **Step 1: Protect ProductsController write endpoints**

Add `[Authorize]` to `Create`, `Update`, `Delete`, `AttachCategory`, `DetachCategory`.

- [ ] **Step 2: Protect CategoriesController write endpoints**

Add `[Authorize]` to `Create`, `Update`, `Delete`.

- [ ] **Step 3: Commit**
```bash
git add dukaan.Host/Controllers/ && git commit -m "feat: protect merchant write endpoints with [Authorize]"
```

---

### Task 4: Add StorefrontController

**Files:**
- Create: `dukaan.Host/Controllers/StorefrontController.cs`

- [ ] **Step 1: Implement StorefrontController**
```csharp
[ApiController]
[Route("api/storefront")]
public class StorefrontController(
    IProductService productService,
    ICategoryService categoryService,
    ITenantService tenantService,
    ITenantProvider tenantProvider) : ControllerBase
{
    private async Task<bool> ResolveTenant(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return false;
        var tenantId = await tenantService.GetTenantIdFromSlug(slug);
        if (tenantId is null) return false;
        tenantProvider.SetTenantId(tenantId.Value);
        return true;
    }

    [HttpGet("products")]
    public async Task<ActionResult<PagedResponse<ProductResponseDto>>> GetProducts(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await productService.GetActiveAsync(request));
    }

    [HttpGet("products/{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProduct(
        [FromHeader(Name = "x-tenant-slug")] string? slug, Guid id)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        var product = await productService.GetByIdAsync(id);
        return product is null or { IsActive: false } ? NotFound() : Ok(product);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<PagedResponse<CategoryResponseDto>>> GetCategories(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await categoryService.GetAllAsync(request));
    }

    [HttpGet("categories/{categoryId}/products")]
    public async Task<ActionResult<PagedResponse<ProductResponseDto>>> GetProductsByCategory(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid categoryId, [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await productService.GetActiveByCategoryAsync(categoryId, request));
    }
}
```

- [ ] **Step 2: Write integration tests**

  - `GetProducts_WithValidSlug_ReturnsActiveProducts`
  - `GetProducts_WithInvalidSlug_Returns404`
  - `GetProducts_WithMissingSlug_Returns404`
  - `GetProduct_InactiveProduct_Returns404`
  - `GetCategories_WithValidSlug_ReturnsCategories`

- [ ] **Step 3: Commit**
```bash
git add dukaan.Host/Controllers/StorefrontController.cs dukaan.Tests.Integration/ && git commit -m "feat: add StorefrontController with public product and category endpoints"
```

---

### Task 5: Verify

- [ ] **Step 1: Run all tests**
```bash
dotnet test
```

- [ ] **Step 2: Confirm all pass, no regressions**
