# Dukaan.Notification Service

Real-time notification service. Consumes order lifecycle events from Dukaan API via Redis Streams, persists notification history, and pushes real-time updates to connected clients via WebSocket (SignalR).

## Responsibility

- Consume order lifecycle events from Redis Streams
- Persist notification history per customer
- Real-time notification delivery via SignalR WebSocket
- Notification inbox with pagination and unread count
- Mark-as-read functionality

**Does NOT handle:** order data, customer data, product data, email/SMS/push notification dispatch, merchant dashboard notifications, notification preferences.

## Ports

| Context | Port |
|---------|------|
| Docker (external) | 5003 |
| Docker (internal) | 8080 |
| SignalR Hub | /hubs/notifications |

## API Endpoints

### REST API (`/api/notifications`) -- `[Authorize]`

| Method | Route | Description | Query Params | Response |
|--------|-------|-------------|--------------|----------|
| GET | `/api/notifications` | List notifications (paginated) | `page` (default 1), `pageSize` (default 20), `unreadOnly` (bool) | `NotificationListDto` |
| GET | `/api/notifications/unread-count` | Get unread count | -- | `UnreadCountDto` |
| POST | `/api/notifications/{id}/read` | Mark as read | -- | `200 OK` |

### Response DTOs

```csharp
// NotificationDto
{
  id: Guid,
  eventType: string,
  orderId: Guid?,
  title: string,
  message: string,
  isRead: bool,
  createdAt: DateTime
}

// NotificationListDto
{
  items: NotificationDto[],
  totalCount: int,
  page: int,
  pageSize: int
}

// UnreadCountDto
{
  count: int
}
```

### SignalR Hub (`/hubs/notifications`)

**Authentication:** JWT Bearer token (same as REST API)

**Connection lifecycle:**
1. Client connects with JWT in query string or Authorization header
2. Server validates JWT, extracts `UserIdentifier` (customer ID from `nameid` claim)
3. Connection added to group `user-{customerId}`
4. Server pushes `Notification` events to group

**Client events (Server → Client):**

| Event | Payload | Description |
|-------|---------|-------------|
| `Notification` | `NotificationDto` | Real-time notification push |

**No client-invocable methods** -- hub is push-only.

**Multiple connections per customer supported** (e.g., multiple browser tabs).

## Database Schema

**Database:** PostgreSQL
**Schema:** `notification` (separate from main Dukaan tables)

### Table: `notification.notifications`

| Column | Type | Notes |
|--------|------|-------|
| id | uuid | PK |
| customer_id | uuid | NOT NULL |
| tenant_id | uuid | NOT NULL, indexed |
| event_type | varchar(100) | NOT NULL (e.g., `order-shipped`) |
| order_id | uuid | NULLABLE |
| title | varchar(200) | NOT NULL |
| message | varchar(2000) | NOT NULL |
| is_read | boolean | NOT NULL DEFAULT FALSE |
| created_at | timestamptz | NOT NULL DEFAULT NOW() |

**Indexes:**
- `ix_notifications_tenant_id` -- tenant filtering
- `ix_notifications_customer_tenant_read_created_at` -- composite: `(customer_id, tenant_id, is_read, created_at DESC)` for inbox queries

**Global query filter:** `HasQueryFilter(e => e.TenantId == tenantProvider.GetTenantId())`

## Notification Types (Event Templates)

| Event Type | Title | Message Template |
|------------|-------|------------------|
| `order-placed` | Order Placed | Your order #{0} has been placed successfully. |
| `order-confirmed` | Order Confirmed | Your order #{0} has been confirmed. |
| `order-shipped` | Order Shipped | Your order #{0} has been shipped. |
| `order-delivered` | Order Delivered | Your order #{0} has been delivered. |
| `order-cancelled` | Order Cancelled | Your order #{0} has been cancelled. |

**Fallback:** Unknown event types use title "Order Update" and raw `data` field as message.

## Inter-Service Communication

### Dukaan (main) → Dukaan.Notification (Redis Streams)

**Direction:** Async, one-way

**Stream:** `order-events`
**Consumer Group:** `notification-group`

**Message format (produced by Dukaan):**
```json
{
  "event": "order-shipped",
  "tenant_id": "guid",
  "customer_id": "guid",
  "order_id": "guid",
  "order_display_id": "789",
  "data": "{\"orderId\":\"guid\",\"newStatus\":\"Shipped\"}"
}
```

### Consumer Implementation (`OrderEventConsumer`)

- `BackgroundService` runs in every Notification Service instance
- Creates consumer group idempotently (handles `BUSYGROUP` error)
- Unique consumer name per instance: `consumer-{MachineName}-{Guid}`
- Blocking read with `StreamReadGroupAsync` (up to 10 messages per batch)
- Processes message → persists to DB → pushes to SignalR → acknowledges message
- Reclaims orphaned messages from crashed consumers via `StreamAutoClaimAsync` (60s idle timeout)

**Delivery guarantee:** At-least-once (messages stay in Pending Entries List until acknowledged)

**Fault tolerance:**
- If consumer crashes mid-processing, message stays in PEL
- Another instance reclaims orphaned messages after 60s idle timeout
- No message loss (unless Redis itself loses data)

### No Direct HTTP/RPC

Notification Service does **not** expose HTTP endpoints for other services to call. All inter-service communication is async via Redis Streams.

## Scalability

### Redis Backplane (SignalR)

**Purpose:** Multi-instance deployment support

**How it works:**
- All instances subscribe to Redis channel `SignalR`
- When one instance sends a message to a group, Redis broadcasts to all instances
- Each instance delivers to its local connections in that group

**Configuration:**
```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis("redis:6379", options =>
    {
        options.Configuration.ChannelPrefix = RedisChannel.Literal("SignalR");
    });
```

**Result:** Clients can connect to any instance; messages reach all connected clients regardless of which instance they're connected to.

### Redis Streams (Consumer Groups)

**Purpose:** Load-balanced event consumption

**How it works:**
- All instances join consumer group `notification-group`
- Each message delivered to exactly one consumer (load balancing)
- If instance crashes, message stays in PEL
- Other instances reclaim orphaned messages after 60s idle timeout

### Horizontal Scaling

Deploy multiple instances:
```yaml
notification-api:
  deploy:
    replicas: 3
```

Each instance runs its own `OrderEventConsumer` (consumer group ensures load balancing) and its own SignalR hub (Redis backplane ensures cross-instance messaging).

## Authentication

**Scheme:** JWT Bearer (same token as Dukaan main service)

**Token claims used:**
- `nameid` -- Customer ID (used as `UserIdentifier` in SignalR)
- `tenant_id` -- Tenant ID (used for tenant resolution)

**Validation:**
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
    NameClaimType = "nameid"
};
```

### SignalR Authentication

- JWT passed via `accessTokenFactory` in SignalR client
- Token sent in query string (`?access_token=...`) or `Authorization` header
- Server validates JWT on connection handshake
- `Context.UserIdentifier` resolves from `nameid` claim

### Tenant Resolution

**Middleware:** `TenantResolutionMiddleware`

**Priority:**
1. JWT `tenant_id` claim (if authenticated)
2. `X-Tenant-Id` header (fallback)

**EF Core filter:** `HasQueryFilter(e => e.TenantId == tenantProvider.GetTenantId())`

## Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=dukaan;Username=dukaan;Password=dukaan123"
  },
  "Redis": {
    "ConnectionString": "redis:6379"
  },
  "Jwt": {
    "Key": "placeholder-key-change-in-production"
  }
}
```

## Dependencies

| Dependency | Purpose | Config Key |
|-----------|---------|-----------|
| PostgreSQL | Notification history | `ConnectionStrings:DefaultConnection` |
| Redis | SignalR backplane + event consumption | `Redis:ConnectionString` |
| Dukaan (main) | Produces order events to Redis Streams | -- |

## Frontend Integration

### SignalR Client Setup

```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${NEXT_PUBLIC_NOTIFICATION_URL}/hubs/notifications`, {
    accessTokenFactory: () => token,
  })
  .withAutomaticReconnect()
  .build();
```

### Event Handler

```typescript
connection.on("Notification", (data: NotificationDto) => {
  toast(data.title, { description: data.message });
  qc.invalidateQueries({ queryKey: ["notifications", slug] });
  qc.invalidateQueries({ queryKey: ["unread-count", slug] });
});
```

## Error Handling & Edge Cases

| Scenario | Behavior |
|----------|----------|
| Redis unavailable during Dukaan publish | Notification lost (best-effort). Order update succeeds. |
| Redis unavailable at Notification Service startup | BackgroundService retries connection. |
| Notification Service crashes mid-consumption | Message stays in PEL. Reclaimed after 60s idle timeout. |
| Frontend loses WebSocket | `withAutomaticReconnect()` retries. REST API still works for history. |
| Customer connects to different instance after reconnect | Redis backplane remaps group. No messages lost. |
| Customer offline | Notification stored in DB. Visible in history when they return. |
| JWT expired on WebSocket | `accessTokenFactory` provides fresh token on reconnect. |
| Unknown event type | Uses fallback title "Order Update" and raw `data` field. |

## NuGet Packages

| Project | Package | Version | Purpose |
|---------|---------|---------|---------|
| Application | `MediatR` | 14.1.0 | CQRS |
| Application | `FluentValidation` | 12.1.1 | Validation |
| Application | `ErrorOr` | 2.1.1 | Result pattern |
| Infrastructure | `StackExchange.Redis` | 2.8.31 | Redis Streams + SignalR backplane |
| Infrastructure | `Microsoft.AspNetCore.SignalR.StackExchangeRedis` | 10.0.9 | SignalR Redis backplane |
| Infrastructure | `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.2 | Database |
| Host | `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.9 | JWT auth |
| Host | `Microsoft.AspNetCore.OpenApi` | 10.0.9 | OpenAPI/Swagger |
