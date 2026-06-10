# Design Spec: Dukaan Initial Version (MVP)

**Date:** 2026-04-17
**Topic:** Initial Multi-tenant Storefront and Merchant Dashboard
**Status:** Complete

## 1. Overview
This project aims to build the foundation for a Dukaan-like e-commerce platform. The initial version focuses on multi-tenant merchant onboarding, store branding, and product management.

## 2. Goals
- Provide a registration flow for new merchants to create their online store.
- Implement a robust multi-tenant architecture using a shared database approach.
- Allow merchants to manage their product catalog and basic store branding.

## 3. Architecture
### 3.1 Tech Stack
- **Backend:** .NET 10 Web API
- **Database:** Entity Framework Core with SQL Server (or SQLite for development)
- **Multi-Tenancy:** Shared Database, Shared Table strategy.

### 3.2 Multi-Tenancy Strategy
- **Tenant Identification:** Tenants will be identified via a custom HTTP header (`X-Tenant-Id`) for the initial API version.
- **Data Isolation:** All tenant-specific tables (Products, Branding, etc.) will include a `TenantId` column.
- **Global Query Filters:** EF Core Global Query Filters will be used to automatically filter data by the current `TenantId`, preventing cross-tenant data leaks.

## 4. Modules & Features

### 4.1 Merchant Onboarding (Tenant Service)
- **Registration Endpoint:** `POST /api/tenants/register`
- **Logic:**
    - Creates a new `Tenant` record.
    - Generates a default `StoreBranding` record with a placeholder logo and the merchant's chosen name.
    - Returns the `TenantId` to the merchant for subsequent requests.

### 4.2 Product Catalog (Product Service)
- **Endpoints:**
    - `POST /api/products`: Create a new product.
    - `GET /api/products`: List all products for the current tenant.
    - `GET /api/products/{id}`: Get product details (scoped to tenant).
    - `PUT /api/products/{id}`: Update product details.
    - `DELETE /api/products/{id}`: Delete a product.
- **Model:** `Name`, `Description`, `Price`, `ImageUrl`, `TenantId`.

### 4.3 Store Branding
- **Endpoints:**
    - `GET /api/branding`: Fetch current store branding.
    - `PUT /api/branding`: Update store branding (name, logo, theme colors).

## 5. Data Model (Draft)
- **Tenant:** `Id (Guid)`, `Name`, `CreatedAt`
- **Product:** `Id (Guid)`, `Name`, `Description`, `Price`, `ImageUrl`, `TenantId`
- **StoreBranding:** `Id (Guid)`, `StoreName`, `LogoUrl`, `ThemeColor`, `TenantId`

## 6. Security & Constraints
- For this initial version, authentication (JWT) is deferred to the next phase. The `X-Tenant-Id` header acts as the primary identifier.
- **Constraint:** A product must always belong to exactly one tenant.

## 7. Future Roadmap
- Public Storefront (Customer-facing site).
- Order Management System.
- Payment Gateway Integrations (Razorpay/Stripe).
- Authentication & Authorization (Merchant logins).
