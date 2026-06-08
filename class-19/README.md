# Class 19 — Dockerizing a .NET API with PostgreSQL

A minimal ASP.NET Core Web API (Todo CRUD) containerized with Docker and orchestrated with Docker Compose alongside a PostgreSQL database and pgAdmin.

---

## Project Structure

```
class-19/
└── learning-docker/
    ├── Dockerfile                  # Multi-stage build for the .NET app
    ├── docker-compose.yml          # Orchestrates app + db + pgAdmin
    ├── Program.cs                  # Minimal API endpoints
    ├── TodoDb.cs                   # EF Core DbContext and models
    ├── appsettings.json            # Connection string config
    ├── Migrations/                 # EF Core migration files
    ├── postgres_data/              # Bind-mounted Postgres data directory
    └── docs.txt                    # Quick command reference
```

---

## Stack

| Component | Technology |
|-----------|-----------|
| API | ASP.NET Core 10 Minimal API |
| ORM | Entity Framework Core + Npgsql |
| Database | PostgreSQL 17 |
| Admin UI | pgAdmin 4 |
| Container runtime | Docker + Docker Compose |

---

## Key Concepts Covered

### Multi-Stage Dockerfile

```dockerfile
# Stage 1 — Build (full SDK image)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2 — Runtime (lightweight ASP.NET image, no SDK)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "learning-docker.dll"]
```

- The **build stage** compiles and publishes the app using the full SDK.
- The **runtime stage** copies only the published output into a much smaller image — the SDK is not included in the final image, keeping it lean and secure.

### Docker Compose Services

Three services are defined in `docker-compose.yml`:

| Service | Image | Port |
|---------|-------|------|
| `db` | `postgres:17` | internal only |
| `pgadmin` | `dpage/pgadmin4` | `5050 → 80` |
| `app` | built from `./Dockerfile` | `8080 → 8080` |

**Startup order:** `app` depends on `db` being healthy (via `pg_isready` health check) before it starts, preventing connection errors on boot.

### Custom Network

All services share a custom bridge network (`todo_network`). This means:
- Containers can reach each other by **service name** (e.g., the app connects to Postgres at host `db`).
- They are isolated from other Docker networks on the host.

### Named Volume with Bind Mount

```yaml
volumes:
  postgres_data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: ./postgres_data
```

Postgres data is stored in `./postgres_data` on the host, so data **survives container restarts**.

### Auto-Migration on Startup

```csharp
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<TodoDb>().Database.Migrate();
```

EF Core migrations are applied automatically when the app starts — no manual `dotnet ef database update` needed inside the container.

---

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/todos` | List all todos |
| `GET` | `/todos/{id}` | Get a todo by ID |
| `POST` | `/todos` | Create a todo |
| `PUT` | `/todos/{id}` | Update a todo's title |
| `PATCH` | `/todos/{id}/complete` | Mark a todo as completed |
| `DELETE` | `/todos/{id}` | Delete a todo |

**Request body** (POST / PUT):
```json
{ "title": "Buy groceries" }
```

---

## Running the Project

### With Docker Compose (recommended)

```bash
# Build images and start all services in the background
docker compose up --build -d

# API is available at:  http://localhost:8080
# pgAdmin is available at: http://localhost:5050
#   Email:    admin@admin.com
#   Password: admin
```

### Useful Compose Commands

```bash
docker compose up           # start (foreground)
docker compose up -d        # start (detached)
docker compose down         # stop and remove containers
docker compose down -v      # also remove volumes (wipes DB data)
docker compose logs -f      # stream logs from all services
docker compose ps           # list running services
```

### Single Container (no DB)

```bash
docker build -t todo-app .
docker run -d --name todo-container -p 8080:8080 todo-app
```

> Note: running without Compose means no Postgres — the app will fail to connect. Use Compose for the full stack.

---

## Connecting pgAdmin to the Database

1. Open `http://localhost:5050` and log in (`admin@admin.com` / `admin`).
2. Register a new server:
   - **Host:** `db`
   - **Port:** `5432`
   - **Username:** `postgres`
   - **Password:** `postgres`
   - **Database:** `todos`

> Use the service name `db` as the host — pgAdmin runs on the same Docker network.

---

## Commit History

### `494243b` — Add initial .NET API with non-multistage Dockerfile
Sets up the project from scratch: minimal ASP.NET Core Web API with in-memory Todo endpoints, a single-stage Dockerfile using the full SDK image, project config files, and a `.gitignore` for .NET artifacts.

### `93f3792` — Optimize Dockerfile with multi-stage build
Refactors the Dockerfile into two stages — a **build stage** (SDK image) that compiles and publishes the app, and a **runtime stage** (lightweight ASP.NET image) that only contains the published output. Significantly reduces the final image size.

### `6944476` — Add Docker Compose with PostgreSQL and pgAdmin
Replaces the in-memory store with a real PostgreSQL database via EF Core. Adds `docker-compose.yml` with three services (`app`, `db`, `pgadmin`), EF Core migrations for the initial schema, and connection string config.

### `0aef41c` — Configure Docker Compose with a custom bridge network
Moves all services onto a named bridge network (`todo_network`) so containers can communicate by service name. Updates the connection string and port config, and expands `docs.txt` with common Compose commands and service URLs.
