# Specification: Customer Authentication Flow

**Status:** Complete
**Date:** 2026-05-19

## 1. Problem Statement

Customers need to register and log in within the context of a specific tenant (store). The existing `AuthService` and `MerchantService` only handle merchant flows. Customer registration must be scoped to a tenant, and customer login must resolve the correct tenant from context rather than relying on a pre-existing JWT.

## 2. Proposed Solution

Extend the existing profile-based identity architecture to support customer registration and login. Customers share the same `ApplicationUser` identity table as merchants, differentiated by `UserType.Customer`. Tenant resolution during customer login uses the store `slug` supplied in the request.

### 2.1. On Separate JWT Scheme

A separate JWT bearer scheme with a different secret key is **not recommended**:

- `user_type` is already a claim in every token. Endpoint-level access control is an **authorization** concern, not an authentication one.
- Two schemes introduce two keys to manage, two Swagger definitions, and `[Authorize(AuthenticationSchemes = "...")]` noise throughout controllers.
- The correct approach is a named authorization policy (`CustomerOnly`) that validates the `user_type` claim.

**Decision:** Single JWT scheme. Add a `CustomerOnly` policy.

## 3. Architecture

### 3.1. Registration Flow

`POST /api/Customers/register`

Header: `X-Tenant-Slug: mystore`

Request body:
```json
{
  "email": "customer@example.com",
  "password": "...",
  "firstName": "Jane",
  "lastName": "Doe",
  "phone": "+8801..."
}
```

Steps:
1. Resolve `Tenant` by `X-Tenant-Slug` header via `TenantProvider` — return `404` if not found.
2. Check `ApplicationUser` does not already exist with that email **within the same tenant** (email + tenantId unique check).
3. Create `ApplicationUser` with `UserType.Customer` and `TenantId` set to the resolved tenant.
4. Create `Customer` profile linked via `ApplicationUserId`.
5. Wrap steps 3–4 in a transaction.
6. Return `201` with a JWT (same shape as merchant login response).

### 3.2. Login Flow

`POST /api/Auth/customer/login`

Header: `X-Tenant-Slug: mystore`

Request body:
```json
{
  "email": "customer@example.com",
  "password": "..."
}
```

Steps:
1. Resolve `Tenant` by `X-Tenant-Slug` header via `TenantProvider` — return `404` if not found.
2. Find `ApplicationUser` by email **and** `TenantId`.
3. Validate `UserType == Customer` — return `401` if a merchant tries to use this endpoint.
4. Validate password via `UserManager.CheckPasswordAsync`.
5. Issue JWT with claims: `NameIdentifier`, `Email`, `user_type = Customer`, `tenant_id`.

### 3.3. Authorization Policy

Add a `CustomerOnly` policy in `Program.cs`:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerOnly", policy =>
        policy.RequireClaim("user_type", "Customer"));
});
```

Customer-specific endpoints use `[Authorize(Policy = "CustomerOnly")]`.

## 4. New / Modified Files

| File | Change |
|------|--------|
| `dukaan.Application/DTOs/CustomerDTOs.cs` | New — `CustomerRegisterRequest`, `CustomerLoginRequest` |
| `dukaan.Application/Services/ICustomerService.cs` | New — `RegisterAsync`, `LoginAsync` |
| `dukaan.Infrastructure/Identity/Services/CustomerService.cs` | New — implementation |
| `dukaan.Infrastructure/Services/TenantProvider.cs` | Modify — add `X-Tenant-Slug` header fallback with `IDistributedCache` |
| `dukaan.Host/Controllers/CustomersController.cs` | New — `POST /register`, `POST /login` |
| `dukaan.Host/Program.cs` | Add `CustomerOnly` policy, register `ICustomerService`, register `AddDistributedMemoryCache` |

`AuthService` and `TenantService` are **not modified** — customer auth is a separate service.

## 5. Tenant Resolution

Tenant context is passed via the `X-Tenant-Slug` request header (e.g. `X-Tenant-Slug: mystore`). This keeps the request body clean and allows the frontend to configure the store slug once (e.g. as an env var) rather than embedding it in every request payload.

**Why slug over TenantId:** Slug is the public-facing store identifier, already unique-indexed, and human-readable. TenantId (GUID) is an internal implementation detail.

### 5.1. TenantProvider Extension

`TenantProvider` is extended to resolve tenant from the header as a fallback when no JWT `tenant_id` claim exists:

```csharp
public Guid? GetTenantId()
{
    // Authenticated requests: read from JWT claim
    var tenantIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
    if (Guid.TryParse(tenantIdClaim, out var tenantId)) return tenantId;

    // Unauthenticated requests (customer login/register): read from header
    var slug = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Slug"].FirstOrDefault();
    if (slug is not null)
        return tenantRepository.FindBySlugAsync(slug); // cached lookup

    return null;
}
```

### 5.2. Slug Resolution Caching

The slug → TenantId lookup runs on every unauthenticated request. To avoid a DB hit per request, the result is cached using `IDistributedCache` with a sliding expiration (e.g. 5 minutes). Cache key: `tenant_slug:{slug}`.

`IDistributedCache` is used (not `IMemoryCache`) so the cache provider can be swapped to Redis later with no code changes — only a registration change in `Program.cs` (`AddDistributedMemoryCache()` → `AddStackExchangeRedisCache()`).

## 6. Constraints

- Email uniqueness is **per-tenant**, not global. Two different stores can have the same customer email.
- A customer cannot log in via the merchant login endpoint (`POST /api/Auth/login`) — that endpoint does not validate `UserType`.
- Customer registration does not create a `Tenant`; it joins an existing one.

## 7. Testing Strategy

**Integration Tests:**
- Register customer in tenant A → succeeds.
- Register same email in tenant B → succeeds (different tenant).
- Register same email in tenant A again → `409 Conflict`.
- Login with correct credentials → returns JWT with `user_type = Customer` and correct `tenant_id`.
- Login with merchant credentials on customer endpoint → `401`.
- Access a `[Authorize(Policy = "CustomerOnly")]` endpoint with merchant token → `403`.
