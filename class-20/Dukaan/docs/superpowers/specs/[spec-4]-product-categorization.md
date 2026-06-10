# Design Spec: Product Categorization and Hierarchy

**Date:** 2026-05-04
**Topic:** Product Categories, Many-to-Many Relationships, and Hierarchy
**Status:** Complete

## 1. Overview
This feature introduces a system for merchants to organize their products into categories. It supports hierarchical categories (sub-categories) and allows a single product to be assigned to multiple categories or none at all.

## 2. Goals
- Merchants can create, edit, and delete categories.
- Categories can be nested (e.g., "Electronics" > "Smartphones").
- Products can belong to multiple categories.
- Ensure strict multi-tenant isolation for all category data.

## 3. Data Model

### 3.1 Category Entity
- `Id`: Guid (Primary Key)
- `TenantId`: Guid (Multi-tenant isolation)
- `Name`: String (Required)
- `Description`: String (Optional)
- `ParentCategoryId`: Guid? (Self-reference for hierarchy)
- `IsActive`: Boolean (Soft delete/Deactivation)

### 3.2 CategorizedProduct (Pivot Entity)
- `ProductId`: Guid (Part of composite PK)
- `CategoryId`: Guid (Part of composite PK)
- `TenantId`: Guid (Multi-tenant isolation)
- *Navigation Properties:* `Product`, `Category`

### 3.3 Product Entity Update
- Add navigation property: `public virtual ICollection<CategorizedProduct> ProductCategories { get; set; }`

## 4. Architecture & Logic

### 4.1 Multi-Tenancy
- Both `Category` and `CategorizedProduct` will implement `ITenantEntity`.
- `ApplicationDbContext` will automatically apply global query filters based on the `TenantId`.
- The `TenantInterceptor` will ensure `TenantId` is set automatically during creation.

### 4.2 Hierarchy Logic
- A category is a "Top-level" category if `ParentCategoryId` is null.
- When deleting a category, we should decide on the strategy (e.g., prevent deletion if it has sub-categories, or cascade delete). For the MVP, we will prevent deletion of categories that have active products or sub-categories.

### 4.3 Many-to-Many logic
- When a product is created or updated, the service layer will:
    1. Validate that all provided `CategoryIds` belong to the current `TenantId`.
    2. Sync the `CategorizedProduct` table (remove old links, add new ones).

## 5. API Design

### 5.1 Category API (`/api/categories`)
- `GET /api/categories`: List categories (support optional nesting or flat list).
- `POST /api/categories`: Create category (accept `ParentCategoryId`).
- `PUT /api/categories/{id}`: Update category.
- `DELETE /api/categories/{id}`: Delete category.

### 5.2 Product API Updates
- `POST /api/products` and `PUT /api/products/{id}` will now accept an optional `List<Guid> CategoryIds`.
- `GET /api/products` will include the category names/IDs in the response.

## 6. Success Criteria
- A merchant can create a "Clothing" category and a "Shirts" sub-category.
- A merchant can assign a "Cotton Shirt" to both "Clothing" and "Shirts".
- Merchant B cannot see or use Merchant A's categories.
- Products can exist without any category assignment.
