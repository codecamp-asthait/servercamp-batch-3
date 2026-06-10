# Design Spec: Public Storefront

**Date:** 2026-05-25
**Topic:** Public-facing storefront endpoints with slug-based tenant resolution
**Status:** Complete

## 1. Overview
The storefront is the public-facing side of Dukaan. A customer visits a merchant's store via a slug identifier (e.g., `my-awesome-store`) and browses that merchant's active products and categories — without authentication.

## 2. Goals
- Customers can browse a merchant's active products and categories without logging in.
- Tenant is resolved from the `x-tenant-slug` request header (consistent with customer registration).
- Existing tenant isolation (global query filters) applies automatically — no new isolation logic needed.
- Merchants' private management endpoints are unaffected.

## 3. Tenant Resolution

### Current State
`TenantProvider` resolves `TenantId` from the JWT `tenant_id` claim only. Storefront requests carry no JWT.

### Change
- Add `SetTenantId(Guid)` to `ITenantProvider`.
- `TenantProvider` stores the value in `HttpContext.Items["TenantId"]` and falls back to it when no JWT claim is present.
- `StorefrontController` reads `x-tenant-slug` header → calls `ITenantService.GetTenantIdFromSlug()` → calls `tenantProvider.SetTenantId()` → delegates to existing services.

This pattern is already established in `CustomersController` for slug resolution.

## 4. Data Considerations

### Active-only products
The storefront must only return products where `IsActive = true`. Currently `GetAllAsync` returns all products. A new `GetActiveAsync` method will be added to `IProductService`.

### Categories
Storefront returns active categories only (`IsActive = true`). Existing `GetAllAsync` on `ICategoryService` already filters active categories — no change needed.

## 5. API Design

### `StorefrontController` — Route: `/api/storefront`
All endpoints are public (no `[Authorize]`). All require `x-tenant-slug` header.

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/storefront/products` | Paginated active products for the store |
| `GET` | `/api/storefront/products/{id}` | Single active product |
| `GET` | `/api/storefront/categories` | All active categories for the store |
| `GET` | `/api/storefront/categories/{categoryId}/products` | Paginated active products in a category |

### Request Header
```
x-tenant-slug: my-awesome-store
```

### Error Responses
- `400 Bad Request` — missing `x-tenant-slug` header
- `404 Not Found` — slug does not match any tenant

## 6. Response Examples

### `GET /api/storefront/products`
```json
{
  "items": [
    {
      "id": "a1b2c3d4-...",
      "name": "Cotton Shirt",
      "description": "Comfortable everyday shirt",
      "price": 499.00,
      "imageUrl": "https://cdn.example.com/shirt.jpg",
      "stockQuantity": 50,
      "isActive": true,
      "categoryIds": ["e5f6g7h8-...", "i9j0k1l2-..."]
    }
  ],
  "totalCount": 42,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### `GET /api/storefront/products/{id}`
```json
{
  "id": "a1b2c3d4-...",
  "name": "Cotton Shirt",
  "description": "Comfortable everyday shirt",
  "price": 499.00,
  "imageUrl": "https://cdn.example.com/shirt.jpg",
  "stockQuantity": 50,
  "isActive": true,
  "categoryIds": ["e5f6g7h8-...", "i9j0k1l2-..."]
}
```

### `GET /api/storefront/categories`
```json
[
  {
    "id": "e5f6g7h8-...",
    "name": "Clothing",
    "description": null,
    "parentCategoryId": null,
    "subCategories": [
      {
        "id": "i9j0k1l2-...",
        "name": "Shirts",
        "description": null,
        "parentCategoryId": "e5f6g7h8-...",
        "subCategories": []
      }
    ]
  }
]
```

### `GET /api/storefront/categories/{categoryId}/products`
Same shape as `GET /api/storefront/products` — a `PagedResponse` of products filtered to that category.

### Error Responses
```json
// 400 — missing header
{ "message": "Store not found." }

// 404 — slug not matched
{ "message": "Store not found." }
```

## 7. Changes Summary

| Layer | Change |
|-------|--------|
| `dukaan.Domain` | Add `SetTenantId(Guid)` to `ITenantProvider` |
| `dukaan.Infrastructure` | Implement `SetTenantId` in `TenantProvider` using `HttpContext.Items` |
| `dukaan.Application` | Add `GetActiveAsync(PaginationRequest)` to `IProductService` / `ProductService` |
| `dukaan.Host` | Add `StorefrontController` |

## 8. Success Criteria
- A customer can list active products for a store using only the slug header.
- A customer cannot see inactive (soft-deleted) products.
- Merchant A's products are never visible on Merchant B's storefront.
- Existing merchant endpoints continue to work unchanged.
- No authentication required for any storefront endpoint.
