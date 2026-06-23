# Learning Testing — Class 22

A .NET Todo API project demonstrating **background job processing** with both `BackgroundService` (IHostedService) and **Hangfire**.

## Solution Structure

```
class-22/
├── learning-testing.todo/                # Main API application
│   ├── BackgroundServices/               # Background job classes
│   │   ├── OverDueTodoArchieveJob.cs         # Hangfire recurring job
│   │   └── OverdueTodoArchieveService.cs     # BackgroundService (alternative)
│   ├── Controllers/TodosController.cs    # HTTP endpoints
│   ├── Services/TodoService.cs           # Business logic
│   ├── Repositories/TodoRepository.cs    # Data access (EF Core)
│   ├── Models/Todo.cs                    # Domain entity
│   ├── DTOs/                             # Request/response models
│   ├── Data/AppDbContext.cs              # EF Core DbContext
│   ├── Migrations/                       # EF Core database migrations
│   ├── docs/cron-expressions.md          # Cron schedule reference
│   └── Program.cs                        # App entry point + Hangfire config

```

## Architecture

```
 HTTP Request
      ↓
 Controller → Service → Repository → EF Core → PostgreSQL
      ↑          ↑            ↑
   (HTTP)    (business    (data access,
              logic,       queries)
              DTO mapping)

 Background (Hangfire Server)
      ↓
 Recurring Job (Cron.Minutely)
      ↓
 OverDueTodoArchieveJob
      ↓
 ITodoService.ArchiveOverdueTodos()
      ↓
 ITodoRepository.ArchiveOverdueTodosAsync()
      ↓
 UPDATE Todos SET IsArchived=true WHERE DueDate < UtcNow
```

The main API follows a **layered architecture** through interfaces (`ITodoService`, `ITodoRepository`). Hangfire runs as an **in-process background server** that executes the recurring archiving job independently of HTTP requests.

## Technologies

| Tool | Purpose |
|---|---|
| ASP.NET Core 10 | Web API framework |
| Entity Framework Core 10 | ORM / data access |
| PostgreSQL | Database |
| Hangfire | Background job processing |

| Swagger / OpenAPI | API docs (dev only) |

## Setup

### 1. Start PostgreSQL (for local development)

```bash
docker run -d --name postgres-todo \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=TodoDb \
  -p 5434:5432 \
  postgres:17
```

### 2. Run the API

```bash
cd learning-testing.todo
dotnet run
```

### 3. Available URLs (development)

| URL | Purpose |
|---|---|
| `http://localhost:5182/api/todos` | Todo REST API |
| `http://localhost:5182/swagger` | Swagger UI (API documentation) |
| `http://localhost:5182/hangfire` | Hangfire dashboard (job monitoring) |
| `https://localhost:7054/api/todos` | Same API over HTTPS |

### 4. Apply EF migrations (if needed)

```bash
cd learning-testing.todo
dotnet ef database update
```

## Background Job — Overdue Todo Archiving

The project includes a **recurring Hangfire job** that automatically archives overdue todos.

### How it works

1. **Hangfire server** starts in-process and polls PostgreSQL for due jobs
2. Every minute (`Cron.Minutely`), `OverDueTodoArchieveJob.ArchiveOverdueTodos()` is enqueued
3. The job finds all non-archived todos with `DueDate < UtcNow`
4. Sets `IsArchived = true` and `ArchivedAt = DateTime.UtcNow`
5. Archived todos are excluded from the default `GET /api/todos` listing
6. On failure, the job retries up to 3 times (`[AutomaticRetry(Attempts = 3)]`)

### Monitoring

The Hangfire dashboard at `/hangfire` shows:
- Recurring jobs with their schedule and next run time
- Completed, failed, and in-progress job history
- Manual job triggering for testing
- Retry and deletion of failed jobs

### Alternative: BackgroundService

An `OverdueTodoArchiveService` (traditional `BackgroundService`) is provided as an alternative. It is commented out in `Program.cs`. To switch, disable Hangfire and uncomment `builder.Services.AddHostedService<OverdueTodoArchiveService>()`.

### Cron schedules

See [`docs/cron-expressions.md`](learning-testing.todo/docs/cron-expressions.md) for a comprehensive cron expression reference with 200+ examples.

## Key Concepts

- **DTOs** — separate the API contract from the domain model so they can evolve independently
- **Dependency Injection** — services receive their dependencies through constructors; the DI container wires everything together in `Program.cs`
- **BackgroundService** — `IHostedService` that polls every minute in a loop; uses `IServiceScopeFactory` to resolve scoped services
- **Hangfire** — a background job framework that stores job state in PostgreSQL; provides recurring jobs, retry, and a web dashboard at `/hangfire`
