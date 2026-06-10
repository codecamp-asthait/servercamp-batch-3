# Specification: Cart Management

**Status:** Pending
**Date:** 2026-05-20

## 1. Problem Statement

Customers need a way to collect products before placing an order. Without a cart, order creation would require the client to submit a full product list in a single request, which is poor UX and makes abandoned cart tracking impossible.

## 2. Scope

This spec covers cart lifecycle only — adding, updating, removing items, and viewing the cart. Order creation from a cart is covered in a separate spec.

## 3. Data Model

### 3.1. Cart

One active cart per customer per tenant. A customer cannot have two active carts in the same store.

```
Cart
├── Id: Guid
├── CustomerId: Guid          → Customer.Id
├── TenantId: Guid
├── CreatedAt: DateTime
└── UpdatedAt: DateTime
```

### 3.2. CartItem

```
CartItem
├── Id: Guid
├── CartId: Guid              → Cart.Id
├── ProductId: Guid           → Product.Id
├── TenantId: Guid
├── Quantity: int             (min: 1)
└── UnitPrice: decimal        (snapshot of Product.Price at time of add)
```

`UnitPrice` is snapshotted at add time so price changes don't silently affect the cart. The client should display a warning if the current product price differs from `UnitPrice`.

## 4. API Endpoints

All endpoints require `[Authorize(Policy = "CustomerOnly")]` and `X-Tenant-Slug` header (resolved via `ITenantService`).

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/Cart` | Get active cart with items |
| `POST` | `/api/Cart/items` | Add item or increment quantity if already in cart |
| `PUT` | `/api/Cart/items/{productId}` | Set exact quantity for an item |
| `DELETE` | `/api/Cart/items/{productId}` | Remove item from cart |
| `DELETE` | `/api/Cart` | Clear all items (cart remains, items removed) |

### 4.1. GET /api/Cart

Returns the customer's active cart. Creates an empty cart if none exists (lazy creation).

Response `200`:
```json
{
  "cartId": "...",
  "items": [
    {
      "productId": "...",
      "productName": "iPhone 15 Pro",
      "unitPrice": 999.99,
      "currentPrice": 949.99,
      "quantity": 2,
      "subtotal": 1999.98,
      "priceChanged": true
    }
  ],
  "total": 1999.98,
  "itemCount": 2
}
```

`priceChanged: true` when `currentPrice != unitPrice`.

### 4.2. POST /api/Cart/items

Request:
```json
{ "productId": "...", "quantity": 1 }
```

Rules:
- If product already in cart → increment quantity by the requested amount
- Validate `quantity >= 1`
- Validate product exists, belongs to tenant, and `IsActive == true`
- Validate `quantity <= Product.StockQuantity` (total in cart after add)
- Return `404` if product not found
- Return `409` if requested quantity exceeds stock

Response `200`: updated cart (same shape as GET).

### 4.3. PUT /api/Cart/items/{productId}

Request:
```json
{ "quantity": 3 }
```

Sets quantity to the exact value. If `quantity == 0`, removes the item (same as DELETE).

Rules:
- Same stock validation as POST
- Return `404` if item not in cart

Response `200`: updated cart.

### 4.4. DELETE /api/Cart/items/{productId}

Removes the item. Returns `404` if item not in cart.

Response `200`: updated cart.

### 4.5. DELETE /api/Cart

Removes all items. Cart entity remains (for future use). Returns `200` with empty cart.

## 5. Business Rules

- **Tenant isolation** — cart and items are scoped to `TenantId`. A customer's cart in store A is invisible in store B.
- **Stock validation** — checked at add/update time, not at checkout. Stock is not reserved.
- **Price snapshot** — `UnitPrice` is set once when the item is added. Subsequent product price changes do not update existing cart items.
- **Lazy cart creation** — `GET /api/Cart` creates an empty cart if none exists. No explicit "create cart" endpoint.
- **One active cart** — enforced at the service layer. If a cart already exists for the customer+tenant, it is reused.

## 6. New Files

| File | Description |
|------|-------------|
| `dukaan.Domain/Entities/Cart.cs` | Cart entity |
| `dukaan.Domain/Entities/CartItem.cs` | CartItem entity |
| `dukaan.Application/DTOs/CartDTOs.cs` | Request/response DTOs |
| `dukaan.Application/Services/ICartService.cs` | Interface |
| `dukaan.Infrastructure/Identity/Services/CartService.cs` | Implementation |
| `dukaan.Host/Controllers/CartController.cs` | Controller |
| `dukaan.Infrastructure/Data/ApplicationDbContext.cs` | Add Cart + CartItem DbSets |
| Migration | `AddCartEntities` |

## 7. Authorization

- All cart endpoints: `[Authorize(Policy = "CustomerOnly")]`
- `CustomerId` is resolved from the JWT `NameIdentifier` claim + a join to `Customer` table (same pattern as merchant profile)
- Merchants cannot access cart endpoints

## 8. Testing Strategy

**Integration Tests:**
- `GET /api/Cart` with no existing cart → creates and returns empty cart
- `POST /api/Cart/items` → item added, quantity correct, price snapshotted
- `POST /api/Cart/items` same product → quantity incremented
- `POST /api/Cart/items` quantity exceeds stock → `409`
- `PUT /api/Cart/items/{productId}` → quantity updated
- `PUT /api/Cart/items/{productId}` with quantity 0 → item removed
- `DELETE /api/Cart/items/{productId}` → item removed
- `DELETE /api/Cart` → all items cleared
- Cart from tenant A not visible in tenant B
