# Specification: Unified Identity System (Merchant & Customer)

**Status:** Complete
**Date:** 2026-05-06

## 1. Problem Statement
Currently, `Merchant` inherits from `IdentityUser`, making it the sole entity capable of authentication. This architecture prevents other user types (like Customers) from having their own accounts and profile-specific data without creating a parallel identity system.

## 2. Proposed Solution
Transition to a **Profile-Based Identity Architecture**. This involves:
- Introducing a single `ApplicationUser` class for all authentication.
- Decoupling business logic into separate `Merchant` and `Customer` profile entities.
- Maintaining store-specific isolation via `TenantId` on the `ApplicationUser`.

## 3. Architecture Changes

### 3.1. Identity Layer (`dukaan.Infrastructure`)
- **New Entity:** `ApplicationUser : IdentityUser<Guid>, ITenantEntity`
  - Properties: `TenantId`, `UserType` (Enum: Merchant, Customer), `RegisteredAt`.
- **Remove:** `Merchant` inheriting from `IdentityUser`.

### 3.2. Domain Layer (`dukaan.Domain`)
- **Refactor:** `Merchant` entity to be a standard domain entity (not an Identity user).
  - Link to `ApplicationUser` via `ApplicationUserId`.
- **New Entity:** `Customer` entity.
  - Link to `ApplicationUser` via `ApplicationUserId`.
  - Includes `TenantId` for multi-tenant isolation.

### 3.3. Data Access (`dukaan.Infrastructure`)
- **DbContext:** Update `ApplicationDbContext` to inherit from `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`.
- **Fluent API:** 
  - Configure 1:1 relationships between `ApplicationUser` and its profiles.
  - Update Global Query Filters to include `ApplicationUser` (scoped by `TenantId`).

## 4. Logical Flow

### 4.1. Registration (Merchant)
1. `TenantService.RegisterMerchant` creates a `Tenant`.
2. Creates an `ApplicationUser` with `UserType.Merchant`.
3. Creates a `Merchant` profile record linked to the `ApplicationUser`.
4. Wraps all operations in a database transaction.

### 4.2. Registration (Customer) - Future Proofing
1. `CustomerService` (to be created) will create an `ApplicationUser` with `UserType.Customer`.
2. Creates a `Customer` profile record.

### 4.3. Authentication
1. `AuthService` validates credentials against `ApplicationUser`.
2. JWT claims will include `user_type` and `tenant_id`.

## 5. Impact on Existing Code
- **Breaking Changes:**
  - `UserManager<Merchant>` must be replaced with `UserManager<ApplicationUser>`.
  - All references to `Merchant` as a user (e.g., in `ClaimsPrincipal`) must be updated to `ApplicationUser`.
- **Migrations:** A new migration will be required to rename/restructure the `AspNetUsers` table and create the new `Merchants` (domain) table.

## 6. Testing Strategy
- **Unit Tests:** Verify `AuthService` handles `ApplicationUser` correctly.
- **Integration Tests:** 
  - Verify merchant registration still works and populates both `ApplicationUser` and `Merchant` tables.
  - Verify `TenantId` isolation on `ApplicationUser`.
