# Design Spec: Merchant Onboarding (Tenant Service)

**Date:** 2026-04-17
**Topic:** Merchant Registration and Multi-tenant Data Persistence
**Parent Spec:** `[spec-1]-dukaan-initial-version-design.md`
**Status:** Complete

## 1. Overview
This sub-spec details the implementation of the merchant onboarding flow. It covers tenant registration and the automatic creation of a default store branding.

## 2. Requirements
- Provide a public endpoint for new merchants to register their store.
- Assign a unique, permanent `TenantId` (Guid) to each new merchant.
- Automatically create a "default branding" record for the merchant (Store Name, placeholder LogoUrl).
- Ensure data is persisted in a relational database using Entity Framework Core.

## 3. Data Models

### 3.1 Merchant (User) Entity
```csharp
// Inherits from Microsoft.AspNetCore.Identity.IdentityUser
public class Merchant : IdentityUser
{
    public Guid TenantId { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
```

### 3.2 Tenant Entity
```csharp
public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StoreName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // e.g., 'mystore'
    public string Category { get; set; } = string.Empty; // e.g., 'Grocery', 'Fashion'
    public string Country { get; set; } = "India";
    public string Currency { get; set; } = "INR";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### 3.3 StoreBranding Entity
```csharp
public class StoreBranding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = "https://placehold.co/200x200?text=Logo";
    public string ThemeColor { get; set; } = "#000000";
}
```

## 4. API Endpoints

### 4.1 Register a New Shop
- **Endpoint:** `POST /api/tenants/register`
- **Request Body (DTO):**
```json
{
  "email": "merchant@example.com",
  "phoneNumber": "+919876543210",
  "password": "Password123!",
  "storeName": "My Awesome Store",
  "slug": "myawesomestore",
  "category": "Grocery",
  "country": "India"
}
```
- **Logic:**
  1. Validate that the `slug` is unique.
  2. Create a new `Tenant` record with category and default currency based on country.
  3. Create a new `Merchant` (User) using ASP.NET Core Identity, linked to the `TenantId`.
  4. Create a new `StoreBranding` record, linked to the `TenantId`.
  5. Generate a JWT token containing `tenant_id` and `sub` (userId) claims.

### 4.2 Check Slug Availability
- **Endpoint:** `GET /api/tenants/check-slug/{slug}`
- **Logic:** Returns `true` if available, `false` otherwise.

## 5. Security & Authentication
- **JWT Claims:**
  - `tenant_id`: The unique ID of the merchant's store. Used by the `TenantProvider` to scope all database queries.
  - `role`: Defaulted to `Admin` for the registering merchant.
- **Slug Validation:** Slugs must be alphanumeric and lowercase.
- **Transactional:** All three records (Tenant, Merchant, Branding) must be created within a single transaction.

## 6. Testing Strategy
- **Unit Tests:** `TenantService` logic for slug validation and entity creation.
- **Integration Tests:** `POST /api/tenants/register` should result in two new database records (Tenant and StoreBranding).
