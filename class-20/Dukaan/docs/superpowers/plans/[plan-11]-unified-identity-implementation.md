# Unified Identity System Implementation Plan

Status: Complete

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Transition to a profile-based identity architecture using the Infrastructure Join pattern. This maintains a clean Domain layer (no Identity dependencies) and utilizes single-query LINQ joins in the Infrastructure layer for performance.

**Architecture:**
- **Domain Layer:** Pure POCO entities (`Merchant`, `Customer`). Identity is referenced via `Guid ApplicationUserId` (Soft Link).
- **Infrastructure Layer:** `ApplicationUser` (Identity implementation) acts as the auth source.
- **Join Strategy:** LINQ Joins in Infrastructure services/repositories perform the SQL JOINs.

---

### Task 1: Domain Profiles (Soft Links)

**Files:**
- Create: `dukaan.Domain/Entities/Merchant.cs`
- Create: `dukaan.Domain/Entities/Customer.cs`
- Create: `dukaan.Domain/Entities/CustomerAddress.cs`

- [x] **Step 1: Create Merchant Profile**
```csharp
using dukaan.Domain.Interfaces;

namespace dukaan.Domain.Entities;

public class Merchant : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; } // Link to Identity User
    public Guid TenantId { get; set; }
}
```

- [x] **Step 2: Create Customer Profile**
```csharp
using dukaan.Domain.Interfaces;

namespace dukaan.Domain.Entities;

public class Customer : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
```

- [x] **Step 3: Create CustomerAddress**
```csharp
using dukaan.Domain.Interfaces;

namespace dukaan.Domain.Entities;

public class CustomerAddress : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public Guid TenantId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
```

- [x] **Step 4: Commit**
```bash
git add dukaan.Domain/Entities/ && git commit -m "feat: add decoupled domain profiles"
```

---

### Task 2: Infrastructure Identity

**Files:**
- Create: `dukaan.Infrastructure/Identity/Models/ApplicationUser.cs`
- Create: `dukaan.Infrastructure/Identity/Models/UserType.cs`

- [x] **Step 1: Create ApplicationUser and UserType**
- [x] **Step 2: Commit**

---

### Task 3: Database Context & Migration

**Files:**
- Modify: `dukaan.Infrastructure/Data/ApplicationDbContext.cs`

- [x] **Step 1: Update ApplicationDbContext**
    - Ensure `ApplicationUser` is configured in `IdentityDbContext`.
    - Map `Merchant`, `Customer`, `CustomerAddress` as `DbSet`.
- [x] **Step 2: Add Migration**
```bash
dotnet ef migrations add TransitionToInfrastructureJoins --project dukaan.Infrastructure --startup-project dukaan.Host
```

---

### Task 4: Service Layer & Joins

- [x] **Step 1: Update AuthService (Login)**
- [x] **Step 2: Update TenantService (Register)**
- [x] **Step 3: Implement Join Logic**
    - Add `MerchantProfileDto(MerchantId, TenantId, Email, Phone)` to `RegisterDTOs.cs`.
    - Add `GetMerchantProfile(Guid userId)` to `ITenantService`.
    - Implement in `TenantService` using LINQ join: `from merchant in context.Merchants join user in context.Users on merchant.ApplicationUserId equals user.Id where user.Id == userId`.
    - Inject `ApplicationDbContext` directly into `TenantService` for the join query.
    - Expose via `GET /api/Tenants/profile` (requires `[Authorize]`, reads `userId` from `NameIdentifier` claim).
