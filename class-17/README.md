# Class 17 — Redis Fundamentals with ASP.NET Core

This class explores **Redis** as an in-memory data store through a hands-on ASP.NET Core Web API project. Each controller demonstrates a different Redis data structure and a real-world use case for it.

---

## Project: `learning-redis`

- **Framework:** ASP.NET Core (.NET 10)
- **Redis Client:** [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) v2.13.1
- **Default Port:** `http://localhost:5150`

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A running Redis instance (local or Docker)

```bash
# Start Redis with Docker
docker run -d -p 6379:6379 redis
```

### Configuration

Set the Redis connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

### Running the project

```bash
cd class-17/learning-redis
dotnet run
```

---

## API Modules

### Health Check

Verifies Redis connectivity by issuing a `PING` command.

| Method | Route     | Description              |
|--------|-----------|--------------------------|
| GET    | `/health` | Returns Redis liveness   |

---

### Module 1 — Strings (Caching with TTL)

Redis Strings are the most basic data type — a key mapped to a raw byte value. Used here for caching arbitrary JSON with an automatic expiry (TTL).

**Key concept:** `StringSet` with a `TimeSpan` calls Redis `SET key value EX ttl`. When the TTL elapses, Redis deletes the key automatically — no background job needed.

| Method | Route                    | Description                         |
|--------|--------------------------|-------------------------------------|
| POST   | `/string-example`        | Store a JSON value with a TTL       |
| GET    | `/string-example/{key}`  | Retrieve a cached value by key      |

**POST body:**
```json
{
  "key": "product:42",
  "value": { "name": "Widget", "price": 9.99 },
  "ttlSeconds": 60
}
```

**Cache miss** (key expired or never set) returns `404`.

---

### Module 2 — Hashes (Structured Object Storage)

Redis Hashes map a key to a set of field-value pairs — like a flat object or row. Used here to store user profiles. The critical advantage over Strings is **field-level writes**: you can update a single field (`lastLogin`) without touching the other fields (`name`, `email`).

| Method | Route                          | Description                           |
|--------|--------------------------------|---------------------------------------|
| POST   | `/hash-example`                | Create a user profile                 |
| GET    | `/hash-example/{id}`           | Retrieve full user profile            |
| PATCH  | `/hash-example/{id}/last-login`| Update only the `lastLogin` field     |

**POST body:**
```json
{
  "id": "1",
  "name": "Alice",
  "email": "alice@example.com"
}
```

Redis command equivalent:
```
HSET user:1 name Alice email alice@example.com lastLogin ""
HSET user:1 lastLogin "2026-06-09T..."   ← PATCH only touches this field
```

---

### Module 3 — Sets (Unique Collections)

Redis Sets are unordered collections of unique strings. Used here to count **unique daily visitors** by IP address. Duplicate `SADD` calls are silently ignored — no error, no overwrite.

The key rotates automatically by date (`visitors:2026-06-09`), so yesterday's data is naturally separated.

| Method | Route                      | Description                          |
|--------|----------------------------|--------------------------------------|
| POST   | `/set-example/visit`       | Record a visitor's IP (deduped)      |
| GET    | `/set-example/unique-count`| Count unique visitors today          |

**POST body:**
```json
{
  "ipAddress": "192.168.1.1"
}
```

Redis commands:
```
SADD visitors:2026-06-09 192.168.1.1   ← returns 1 (new) or 0 (duplicate)
SCARD visitors:2026-06-09              ← O(1) cardinality
```

---

### Module 4 — Lists (Activity Feeds / Search History)

Redis Lists are ordered sequences accessed by index, with fast push/pop from both ends. Used here to maintain a **capped search history** per user: the 10 most recent terms, newest first.

`ListLeftPush` prepends the new term; `ListTrim` immediately trims to 10 items — no cleanup job needed.

| Method | Route                                 | Description                           |
|--------|---------------------------------------|---------------------------------------|
| POST   | `/list-example/search`                | Prepend a search term to history      |
| GET    | `/list-example/search/history?userId=`| Get 5 most recent search terms        |

**POST body:**
```json
{
  "userId": "user:1",
  "term": "redis lists"
}
```

Redis commands:
```
LPUSH search-history:user:1 "redis lists"
LTRIM search-history:user:1 0 9          ← keep only 10 items
LRANGE search-history:user:1 0 4         ← read top 5
```

---

### Module 5 — Sorted Sets (Leaderboards)

Redis Sorted Sets associate each member with a floating-point **score** and keep members sorted by that score at all times. Used here to build a **product view leaderboard** — each view atomically increments a product's score.

| Method | Route                         | Description                            |
|--------|-------------------------------|----------------------------------------|
| POST   | `/sorted-set-example/view`    | Increment view count for a product     |
| GET    | `/sorted-set-example/top`     | Get top 5 most-viewed products         |

**POST body:**
```json
{
  "productId": "product:42"
}
```

Redis commands:
```
ZINCRBY product-views 1 product:42          ← atomic increment, creates if missing
ZREVRANGE product-views 0 4 WITHSCORES      ← top 5, highest score first
```

---

### Module 6 — Distributed Locks (Concurrency Control)

Demonstrates preventing **race conditions** in a multi-instance deployment using Redis as a distributed lock. The inventory decrement follows a read-modify-write pattern that is unsafe without a lock.

`LockTake` uses `SET NX PX` under the hood — atomic acquire. A unique token per request ensures only the lock owner can release it. A 5-second TTL auto-releases the lock if the process crashes.

| Method | Route                              | Description                                  |
|--------|------------------------------------|----------------------------------------------|
| POST   | `/lock-example/inventory/seed`     | Set initial inventory count                  |
| GET    | `/lock-example/inventory`          | Read current inventory                       |
| POST   | `/lock-example/inventory/decrement`| Decrement inventory under a distributed lock |

**Seed body:**
```json
{
  "count": 10
}
```

**Lock flow:**
```
SET lock:inventory <token> NX PX 5000   ← acquire (returns OK or nil)
  → read stock, check, decrement
DEL lock:inventory <token>              ← release (only if token matches)
```

Returns `409 Conflict` if the lock is already held by another request.

---

### Module 7 — Pub/Sub (Messaging)

Redis Pub/Sub allows publishers to send messages to a channel without knowing who is subscribed. All running instances subscribe to `system-notifications` on startup and log every message they receive.

| Method | Route                      | Description                                   |
|--------|----------------------------|-----------------------------------------------|
| POST   | `/pubsub-example/notify`   | Publish a message to `system-notifications`   |

**POST body:**
```json
{
  "message": "Deployment complete."
}
```

The subscriber is registered in `Program.cs` at startup — every running instance logs published messages via `ILogger`.

---

## Project Structure

```
class-17/
└── learning-redis/
    ├── Controllers/
    │   ├── StringExampleController.cs     # Strings / caching
    │   ├── HashExampleController.cs       # Hashes / structured objects
    │   ├── SetExampleController.cs        # Sets / unique visitors
    │   ├── ListExampleController.cs       # Lists / search history
    │   ├── SortedSetController.cs         # Sorted Sets / leaderboards
    │   ├── LockExampleController.cs       # Distributed locking
    │   └── PubSubExampleController.cs     # Pub/Sub messaging
    ├── Models/                            # Request / response records
    ├── RedisHealthCheck.cs                # /health endpoint
    ├── Program.cs                         # DI setup, Pub/Sub subscriber
    ├── appsettings.json                   # Redis connection string
    └── learning-redis.http                # HTTP test file (all endpoints)
```

---

## Redis Data Structure Summary

| Data Structure | Use Case in This Project    | Key Redis Commands                        |
|----------------|-----------------------------|-------------------------------------------|
| String         | Cache with TTL              | `SET`, `GET`, `EX`                        |
| Hash           | User profile (field-level)  | `HSET`, `HGETALL`, `HEXISTS`              |
| Set            | Unique visitor counting     | `SADD`, `SCARD`                           |
| List           | Capped search history       | `LPUSH`, `LTRIM`, `LRANGE`               |
| Sorted Set     | Product view leaderboard    | `ZINCRBY`, `ZREVRANGE WITHSCORES`        |
| String + Lock  | Distributed concurrency     | `SET NX PX`, `DEL` (token-guarded)       |
| Pub/Sub        | Cross-instance messaging    | `PUBLISH`, `SUBSCRIBE`                    |
