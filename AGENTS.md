# Project Standards: Dukaan

Multi-vendor e-commerce platform with a .NET backend and Next.js frontend.

## Project Structure

```
servercamp-batch-3/
├── backend/
│   ├── docker-compose.yml       # Infrastructure & API services
│   ├── Dukaan/                  # .NET Web API (merchant & store APIs)
│   ├── Dukaan.Media/            # .NET media service (image/file upload)
│   └── Dukaan.Notification/     # .NET notification service (real-time push)
├── frontend/
│   └── dukaan-web/              # Next.js 16 web app
└── AGENTS.md                    # This file
```

## Planning & Documentation Conventions

### Spec File Naming

- **Main Specs:** `[spec-index-number]-spec-details`
  - *Example:* `[spec-1]-program-cs-refactoring`
- **Sub-specs:** `[spec-index-number]-[subindex-number]-spec-details`
  - *Example:* `[spec-1]-[1.1]-multi-tenant-resolution`
- **Experiment Specs:** `spec-experiment-[experiment-name]-[number]`
  - *Example:* `spec-experiment-observability-1`
  - Used for experimental features or integrations outside the regular development flow

### Plan File Naming

- **Main Plans:** `[plan-index-number]-plan-details`
  - *Example:* `[plan-1]-planning-multitenancy`
- **Sub-plans:** `[plan-index-number]-[subindex-number]-plan-details`
  - *Example:* `[plan-1]-[1.1]-decision-on-multitenancy-management`

### Spec & Plan Status Management

- All spec and plan files MUST include a `Status:` field at the top (e.g., `Status: Draft`, `Status: Approved`, `Status: Rejected` for specs; `Status: Pending`, `Status: In Progress`, `Status: Completed` for plans).
- The status MUST be updated as progress is made.

### Spec & Plan File Location (Project-Based Structure)

Documentation is organized by project to support multi-service architecture.

- **Main Project (Dukaan):**
  - Specs: `docs/superpowers/specs/`
  - Plans: `docs/superpowers/plans/`

- **Dedicated Projects / Microservices:**
  - Specs: `docs/superpowers/specs/`
  - Plans: `docs/superpowers/plans/`

**Examples:**
- Main specs: `Dukaan/docs/superpowers/specs/[spec-1]-program-cs-refactoring.md`
- Media microservice specs: `Dukaan.Media/docs/superpowers/specs/[spec-1]-media-microservice-design.md`
- Notification microservice specs: `Dukaan.Notification/docs/superpowers/specs/[spec-1]-[1.1]-notification-service-detailed.md`

> **Rule:** Never mix specs or plans from different projects in the same folder. Each project maintains its own `docs/superpowers/` folder.

## Architecture: Clean Architecture (Backend)

All .NET backend services follow **Clean Architecture** (Onion Architecture). Dependencies always point inward.

### Layer Structure (per service)

| Layer | Project | Responsibility | Dependencies |
|-------|---------|---------------|-------------|
| Domain | `*.Domain` | Entities, Value Objects, Domain Interfaces | Zero dependencies |
| Application | `*.Application` | CQRS Handlers, DTOs, Validators, App Interfaces | Domain only |
| Infrastructure | `*.Infrastructure` | EF Core DbContext, Repositories, External Services | Application + Domain |
| Host | `*.Host` | ASP.NET Controllers, Middleware, Program.cs, DI | Infrastructure + Application |

### Dependency Direction

```
*.Domain
    ↑
*.Application
    ↑
*.Infrastructure
    ↑
*.Host
```

### Key Rules

- **Interfaces in Application, Implementations in Infrastructure**
- **No Direct DbContext Access** from Application layer — use `IRepository<>`
- **CQRS via MediatR** — business logic in Handlers under `Features/`
- **Validation Pipeline** — FluentValidation wired via `ValidationBehavior`
- **Solution Format** — Use `.slnx` for new projects

## Coding Standards (.NET)

- **File-Scoped Namespaces:** `namespace MyProject;`
- **Primary Constructors:** C# 12+ syntax for DI
- **Immutability:** Use `record` for DTOs and API response models
- **Naming:** .NET PascalCase (classes), camelCase (JSON)
- **Performance:** `AsNoTracking()` for read-only queries with `trackChanges` parameter

## API Rules (.NET)

- **No Entities in Controllers** — use DTOs
- **Thin Controllers** — delegate to MediatR Handlers via `IMediator.Send()`
- **Explicit Result Types** — `Task<ActionResult<T>>`

## Multi-Tenancy

- **Database:** PostgreSQL
- **Tenant Isolation:** Entities implement `ITenantEntity` with `TenantId`
- **Global Filtering:** EF Core `HasQueryFilter` scoped per tenant
- **Resolution:** `TenantResolutionMiddleware` sets `ITenantProvider` per request

## Backend Services

| Service | Port | Description |
|---------|------|-------------|
| Postgres | 5433 | Database |
| Dukaan API | 5001 | Main backend API |
| Dukaan Media | 5002 | Media upload service |
| Notification API | 5003 | Real-time notification push |
| MinIO | 9000 | Object storage (S3-compatible) |
| MinIO Console | 9001 | MinIO web UI |
| Grafana | 3001 | Observability dashboard |
| Loki | 3100 | Log aggregation |
| Tempo | 3200 | Distributed tracing |
| Prometheus | 9091 | Metrics |
| Otel Collector | 4317 | OpenTelemetry endpoint |

## Frontend (dukaan-web)

- **Framework:** Next.js 16
- **State Management:** TanStack Query
- **Module Structure:** `src/modules/store/` organized by feature (auth, cart, products, notifications)
- **API Layer:** Custom `http()` wrapper with `tenantHeaders` helper
- **Auth:** JWT Bearer token with `Authorization` header + `x-tenant-slug` for tenant context

## Global Conventions

### .gitignore

Each .NET project should have its own `.gitignore` generated via `dotnet new .gitignore`. Project-level docs folders should be gitignored (add `docs/` entry).

### Naming Consistency

- Specs and plans follow the naming conventions above without exception
- Service names match: folder name → namespace prefix → docker-compose service key
- Example: `Dukaan.Notification/` → `Dukaan.Notification.*` → `notification-api`
