# Dukaan Service

Core API for the multi-vendor e-commerce platform. Handles merchant onboarding, customer auth, product catalog, cart, addresses, and storefront browsing.

## Responsibility

- Merchant registration and authentication (JWT)
- Customer registration and authentication (JWT, tenant-scoped)
- Product catalog CRUD (merchant) and browsing (customer)
- Hierarchical category management
- Shopping cart with price-change detection
- Customer address management
- Public storefront API (tenant-scoped)
- Multi-tenancy via shared database + EF Core global filters
- Background media resolution (polls Dukaan.Media for product images)

**Does NOT handle:** file uploads (Dukaan.Media), real-time notifications (Dukaan.Notification), payments/orders (not yet implemented).

## Ports

| Context | Port |
|---------|------|
| Docker (external) | 5001 |
| Docker (internal) | 8080 |
| Local dev | 5210 (HTTP), 7172 (HTTPS) |
| Hangfire Dashboard | /hangfire |

## API Endpoints

### Auth (`/api/auth`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/login` | Anonymous | Merchant login, returns JWT |
| POST | `/api/auth/customer/login` | Anonymous + `x-tenant-slug` | Customer login, returns JWT |

### Merchants (`/api/merchants`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/merchants/register` | Anonymous | Register merchant + create Tenant |
| GET | `/api/merchants/profile` | JWT | Get current merchant profile |
| GET | `/api/merchants/check-slug/{slug}` | Anonymous | Check slug availability |

### Customers (`/api/customers`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/customers/register` | Anonymous + `x-tenant-slug` | Register customer within tenant |
| GET | `/api/customers/me` | JWT | Get current customer ID |

### Products (`/api/products`) -- `[Authorize]` at class level

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/products` | JWT | List all products (paged) |
| GET | `/api/products/active` | JWT | List active products |
| GET | `/api/products/{id}` | Anonymous | Get single product |
| POST | `/api/products` | JWT | Create product |
| PUT | `/api/products/{id}` | JWT | Update product |
| DELETE | `/api/products/{id}` | JWT | Delete product |
| GET | `/api/products/category/{categoryId}` | Anonymous | Products by category |
| GET | `/api/products/category/{categoryId}/active` | Anonymous | Active products by category |
| POST | `/api/products/{productId}/categories/{categoryId}` | JWT | Attach product to category |
| DELETE | `/api/products/{productId}/categories/{categoryId}` | JWT | Detach product from category |

### Categories (`/api/categories`) -- `[Authorize]` at class level

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/categories` | JWT | List all categories |
| GET | `/api/categories/{id}` | JWT | Get category by ID |
| POST | `/api/categories` | JWT | Create category |
| PUT | `/api/categories/{id}` | JWT | Update category |
| DELETE | `/api/categories/{id}` | JWT | Delete category |
| GET | `/api/categories/dropdown` | JWT | Lightweight dropdown list |
| GET | `/api/categories/parent/{parentId}` | JWT | Subcategories of parent |

### Cart (`/api/cart`) -- `[Authorize]` + `x-tenant-slug`

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/cart` | Get customer's cart |
| POST | `/api/cart/items` | Add item to cart |
| PUT | `/api/cart/items/{productId}` | Update item quantity |
| DELETE | `/api/cart/items/{productId}` | Remove item |
| DELETE | `/api/cart` | Clear entire cart |

### Addresses (`/api/addresses`) -- `[Authorize]` + `x-tenant-slug`

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/addresses` | Get all addresses |
| GET | `/api/addresses/{id}` | Get address by ID |
| POST | `/api/addresses` | Create address |
| PUT | `/api/addresses/{id}` | Update address |
| DELETE | `/api/addresses/{id}` | Delete address |
| PATCH | `/api/addresses/{id}/set-default` | Set as default for its type |

### Storefront (`/api/storefront`) -- Public + `x-tenant-slug`

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/storefront/products` | Browse active products |
| GET | `/api/storefront/products/{id}` | Get single active product |
| GET | `/api/storefront/categories` | Browse active categories |
| GET | `/api/storefront/categories/{categoryId}/products` | Products in category |

## Database Schema

**Database:** PostgreSQL (via `Npgsql.EntityFrameworkCore.PostgreSQL`)
**ORM:** EF Core 10 with `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`

### Entities

| Entity | Key Fields | Notes |
|--------|-----------|-------|
| **Tenant** | Id, StoreName, Slug, Category, Country, Currency | Root entity, NOT tenant-scoped |
| **ApplicationUser** | Id, Email, UserType (Merchant/Customer/Admin), TenantId | ASP.NET Identity, implements ITenantEntity |
| **Merchant** | Id, ApplicationUserId, TenantId | 1:1 with ApplicationUser |
| **Customer** | Id, ApplicationUserId, TenantId, FirstName, LastName, Phone | 1:1 with ApplicationUser |
| **Product** | Id, TenantId, Name, Description, Price, ImageUrl, PendingMediaId, StockQuantity, IsActive | Many-to-many with Category |
| **Category** | Id, TenantId, Name, Description, IsActive, ParentCategoryId | Self-referencing hierarchy |
| **CategorizedProduct** | ProductId, CategoryId, TenantId | Join entity for Product-Category |
| **Cart** | Id, TenantId, CustomerId | One per customer |
| **CartItem** | Id, TenantId, CartId, ProductId, Quantity, UnitPrice | Price snapshot at add time |
| **Address** | Id, TenantId, CustomerId, Label, Type (Billing/Delivery), Street, City, District, PostalCode, Phone, IsDefault | Multiple per customer |

All entities except Tenant implement `ITenantEntity` with `TenantId` for multi-tenancy.

## Multi-Tenancy

**Strategy:** Shared database, tenant-scoped data isolation via EF Core global query filters.

**Flow:**
1. Request arrives with `x-tenant-slug` header (storefront) or JWT `tenant_id` claim (merchant)
2. Controller resolves slug to TenantId via `GetTenantIdFromSlugQuery`
3. `TenantProvider.SetTenantId(tenantId)` stores it in `HttpContext.Items`
4. EF Core `HasQueryFilter` automatically adds `WHERE TenantId = @tenantId` to all queries
5. `TenantInterceptor` auto-sets `TenantId` on INSERT for new entities

**Key components:**
- `ITenantEntity` interface -- all tenant-scoped entities implement this
- `ITenantProvider` -- scoped service, reads from JWT claim or HttpContext
- `TenantResolutionMiddleware` -- not used; controllers resolve tenant manually
- `TenantInterceptor` -- EF Core SaveChangesInterceptor for auto-stamping TenantId

## Inter-Service Communication

### Dukaan → Dukaan.Media (HTTP polling)

- **Interface:** `IMediaService` (Application layer)
- **Implementation:** `MediaService` (Infrastructure layer, HttpClient)
- **Base URL:** `MediaService:BaseUrl` config (default: `http://dukaan-media:8080`)
- **Endpoint called:** `GET /api/media/{mediaId}` with header `X-Tenant-Id`
- **Response:** `MediaApiResponse { Id, Status, ImagePath }` (Status 2 = completed, 3 = failed)
- **Used by:** `MediaResolutionJob` (Hangfire recurring job, every 30 seconds)

### Dukaan → Dukaan.Notification (Redis Streams)

- **Mechanism:** Redis Streams (async, one-way)
- **Stream:** `order-events`
- **Consumer Group:** `notification-group`
- **Message format:** `{ event, tenant_id, customer_id, order_id, order_display_id, data }`
- **Events published:** order-placed, order-confirmed, order-shipped, order-delivered, order-cancelled

## Background Jobs (Hangfire)

**Storage:** PostgreSQL, schema `hangfire`
**Worker count:** 2
**Queue:** `media-resolution`
**Dashboard:** `/hangfire` (no auth)

### MediaResolutionJob

- **Schedule:** Every 30 seconds (`*/30 * * * * *`)
- **Retry:** 3 attempts
- **What it does:**
  1. Queries products with `PendingMediaId != null`
  2. Calls `GET /api/media/{mediaId}` on Dukaan.Media
  3. If status == 2 (completed): sets `product.ImageUrl`, clears `PendingMediaId`
  4. If status == 3 (failed): clears `PendingMediaId`

## Authentication

**Scheme:** JWT Bearer (HMAC-SHA256)

**Token claims:**
- `NameIdentifier` -- ApplicationUser.Id
- `Email` -- ApplicationUser.Email
- `tenant_id` -- ApplicationUser.TenantId

**Expiration:** `Jwt:ExpireInMinutes` (default 200 minutes)

**User types:** `Merchant = 1`, `Customer = 2`, `Admin = 3`

## Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost:5433;Database=dukaan;Username=postgres;Password=password"
  },
  "Jwt": {
    "Key": "...",
    "ExpireInMinutes": 200
  },
  "MediaService": {
    "BaseUrl": "http://localhost:5002"
  },
  "Observability": {
    "OtlpEndpoint": "http://localhost:4317",
    "ServiceName": "dukaan-api",
    "Environment": "Development"
  }
}
```

## Dependencies

| Dependency | Purpose | Config Key |
|-----------|---------|-----------|
| PostgreSQL 17 | Database + Hangfire storage | `ConnectionStrings:DefaultConnection` |
| Dukaan.Media | Image upload/processing | `MediaService:BaseUrl` |
| Redis | Event publishing (order events) | `Redis:ConnectionString` |
| MinIO | (via Dukaan.Media) | -- |

## Observability

**Custom metrics (DukaanMetrics):**
- `dukaan.cart_items_added` -- Counter
- `dukaan.cart_items_added_value` -- Counter
- `dukaan.cart_items_removed` -- Counter
- `dukaan.customer_registrations` -- Counter
- `dukaan.merchant_registrations` -- Counter
- `dukaan.auth_logins` -- Counter (tagged with tenant_id)
- `dukaan.auth_failures` -- Counter

**Tracing:** ASP.NET Core + HttpClient + EF Core instrumentation, OTLP gRPC export
**Logging:** Serilog (Console + OpenTelemetry sink)

## Database Seeding

On startup, if database is empty:
1. Creates "Demo Store" tenant (slug: `demo-store`)
2. Creates merchant: `demo@example.com` / `Demo@123`
3. Creates customer: `customer@example.com` / `Customer@123`
4. Creates 8 categories (4 top-level + 4 subcategories)
5. Creates 10 sample products

## Error Handling

- **ErrorOr** library (Result pattern) throughout
- `BaseApiController.ToActionResult<T>()` maps errors to HTTP status:
  - NotFound → 404, Validation → 400, Conflict → 409, Unauthorized → 401, Forbidden → 403
- `GlobalExceptionHandler` returns RFC 7807 `ProblemDetails` for unhandled exceptions

## CORS

Allows: `http://localhost:3000`, `http://localhost:3001`, `http://localhost:5001`
