# [Spec-1]-[1.4]-Product Catalog Design

## Overview
This specification defines the Product Catalog module for the Dukaan platform, enabling merchants to manage their product listings within their tenant scope, adhering to N-Tier architecture standards.

## Requirements
- Support CRUD operations for products (Create, Read, Update, Delete) using DTOs.
- Ensure multi-tenant isolation using `TenantId` and Global Query Filters.
- Support basic inventory tracking (`StockQuantity`).
- Implement soft-delete functionality using `IsActive`.
- Validate product data using FluentValidation (e.g., non-negative price, non-empty name).

## Data Model (Entities & DTOs)
- **Entity:** `Product` (includes `Id`, `TenantId`, `Name`, `Description`, `Price`, `ImageUrl`, `StockQuantity`, `IsActive`).
- **DTOs:**
    - `ProductRequestDto` (for POST/PUT operations)
    - `ProductResponseDto` (for GET operations)

## API Endpoints (Controllers)
- Controllers accept `ProductRequestDto` and return `ProductResponseDto`.
- Controllers delegate all business logic to `IProductService`.

## Architecture & Logic (Service & Data Layer)
- **Service Layer:** `IProductService` implementation will contain business logic and exclusively use the Central Generic `IRepository<Product>` for data access.
- **Data Access:** Generic Repository handles EF Core operations, respecting tenant scope via `ITenantProvider`.
- **Security:** Scoped to the current tenant via `ITenantProvider`.
