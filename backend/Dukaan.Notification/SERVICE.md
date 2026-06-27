# Dukaan.Notification Service

Notification dispatch service. Consumes order lifecycle events from Dukaan API via Redis Streams and dispatches them to configured channels using a strategy pattern. Channels: `"in-app"` (persisted inbox + SignalR push), `"signal"` (instant raw data push via SignalR), `"email"` (formatted HTML via SMTP). Channel selection is producer-driven via `notification_types` in the event payload.

## Responsibility

- Consume order lifecycle events from Redis Streams
- Persist notification history per customer
- Real-time notification delivery via SignalR WebSocket (in-app inbox)
- Real-time signal dispatch via SignalR WebSocket (instant toasts)
- Email notification delivery via SMTP
- Notification inbox with pagination and unread count
- Mark-as-read functionality

**Does NOT handle:** order data, customer data, product data, SMS/push notification dispatch, merchant dashboard notifications, notification preferences.

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
| `Notification` | `NotificationDto` | In-app inbox notification push |
| `Signal` | `string` | Instant raw signal for toasts |

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
| is_read | boolean | NOT NULL DEFAULT FALSE |
| created_at | timestamptz | NOT NULL DEFAULT NOW() |

**Indexes:**
- `ix_notifications_tenant_id` -- tenant filtering
- `ix_notifications_customer_tenant_read_created_at` -- composite: `(customer_id, tenant_id, is_read, created_at DESC)` for inbox queries

**Global query filter:** `HasQueryFilter(e => e.TenantId == tenantProvider.GetTenantId())`

## Dispatch Pattern

The service uses a strategy pattern for notification delivery:

- `INotificationDispatcher` interface with `ChannelType` property (`"in-app"`, `"signal"`, `"email"`)
- `INotificationDispatchManager` orchestrates dispatchers based on `notification_types` in the event payload
- `OrderEventConsumer` delegates to the manager — it never loops through dispatchers directly
- The manager filters dispatchers by channel type and executes each with try/catch isolation
- Adding a new channel = create a new dispatcher class — no changes to the consumer or manager

### Channels

| Channel | Dispatcher | Persists? | SignalR Event | Description |
|---------|-----------|-----------|---------------|-------------|
| `"in-app"` | `InAppDispatcher` | Yes | `"Notification"` | Inbox notification — persisted + pushed for bell icon history |
| `"signal"` | `SignalDispatcher` | No | `"Signal"` | Instant real-time toast — raw data pushed verbatim |
| `"email"` | `EmailDispatcher` | No | N/A | Formatted HTML email via SMTP |

### Templates

Each dispatcher owns its own templates:
- `InAppDispatcher` formats `NotificationDto` with event type and order ID — no title/message on the entity
- `EmailDispatcher` has its own `EmailTemplates` dictionary for subject and HTML body per event type
- `SignalDispatcher` pushes raw data verbatim — no templates

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
  "customer_email": "customer@example.com",
  "notification_types": "in-app,signal,email",
  "order_id": "guid",
  "order_display_id": "789",
  "data": "{\"orderId\":\"guid\",\"newStatus\":\"Shipped\"}"
}
```

`notification_types` is a comma-separated list of channel identifiers. Supported values: `in-app`, `signal`, `email`. If omitted, defaults to `in-app`.

### Consumer Implementation (`OrderEventConsumer`)

- `BackgroundService` runs in every Notification Service instance
- Creates consumer group idempotently (handles `BUSYGROUP` error)
- Unique consumer name per instance: `consumer-{MachineName}-{Guid}`
- Blocking read with `StreamReadGroupAsync` (up to 10 messages per batch)
- Processes message → resolves `INotificationDispatchManager` → dispatches to configured channels → acknowledges message
- Persistence is handled by `InAppDispatcher` (not the consumer)
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
  },
  "Email": {
    "Smtp": {
      "Host": "mailhog",
      "Port": 1025,
      "Username": "",
      "Password": "",
      "FromName": "Dukaan",
      "FromAddress": "noreply@dukaan.com",
      "EnableSsl": false
    }
  }
}
```

## Dependencies

| Dependency | Purpose | Config Key |
|-----------|---------|-----------|
| PostgreSQL | Notification history | `ConnectionStrings:DefaultConnection` |
| Redis | SignalR backplane + event consumption | `Redis:ConnectionString` |
| Dukaan (main) | Produces order events to Redis Streams | -- |
| MailHog | Local email capture (dev) | `Email:Smtp:Host`, `Email:Smtp:Port` |

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
| Customer offline | In-app notification stored in DB. Visible in history when they return. |
| JWT expired on WebSocket | `accessTokenFactory` provides fresh token on reconnect. |
| Unknown event type | Each dispatcher uses its own fallback — `InAppDispatcher` shows event type, `EmailDispatcher` uses generic text, `SignalDispatcher` passes raw data. |
| SMTP connection fails | `NotificationDispatchManager` catches and logs. Email fails but in-app/signal still succeed. |
| Customer email missing when `"email"` channel active | `EmailDispatcher` skips, logs warning. |
| `notification_types` missing from event | Defaults to `"in-app"` only. Backward compatible. |

## NuGet Packages

| Project | Package | Version | Purpose |
|---------|---------|---------|---------|
| Application | `MediatR` | 14.1.0 | CQRS |
| Application | `FluentValidation` | 12.1.1 | Validation |
| Application | `ErrorOr` | 2.1.1 | Result pattern |
| Infrastructure | `StackExchange.Redis` | 2.8.31 | Redis Streams + SignalR backplane |
| Infrastructure | `Microsoft.AspNetCore.SignalR.StackExchangeRedis` | 10.0.9 | SignalR Redis backplane |
| Infrastructure | `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.2 | Database |
| Infrastructure | `MailKit` | 4.12.1 | SMTP email sending |
| Host | `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.9 | JWT auth |
| Host | `Microsoft.AspNetCore.OpenApi` | 10.0.9 | OpenAPI/Swagger |
