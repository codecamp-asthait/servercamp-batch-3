# Learning Testing — Class 21

A .NET Todo API project designed to teach testing concepts — from unit tests with mocks to full integration tests with a real database.

## Solution Structure

```
class-21/
├── learning-testing.todo/            # Main API application
│   ├── Controllers/TodosController.cs   # HTTP endpoints
│   ├── Services/TodoService.cs          # Business logic
│   ├── Repositories/TodoRepository.cs   # Data access (EF Core)
│   ├── Models/Todo.cs                   # Domain entity
│   ├── DTOs/                            # Request/response models
│   ├── Data/AppDbContext.cs             # EF Core DbContext
│   └── Program.cs                       # App entry point
├── learning-testing.UnitTests/        # Unit tests (mocked)
│   └── Services/TodoServiceTests.cs
└── learning-testing.IntegrationTests/ # Integration tests (real DB)
    ├── Controllers/TodosControllerTests.cs
    └── CustomWebApplicationFactory.cs
```

## Architecture

```
Controller → Service → Repository → EF Core → PostgreSQL
     ↑          ↑            ↑
  (HTTP)    (business    (data access,
             logic,       queries)
             DTO mapping)
```

This is a **layered architecture**. Each layer depends only on the layer below it through interfaces (`ITodoService`, `ITodoRepository`), making the code testable — you can mock any layer in isolation.

## Technologies

| Tool | Purpose |
|---|---|
| ASP.NET Core 10 | Web API framework |
| Entity Framework Core 10 | ORM / data access |
| PostgreSQL | Database |
| xUnit | Test framework |
| Moq | Mocking library (unit tests) |
| FluentAssertions | Readable assertions |
| Testcontainers | Docker containers in tests |
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

Swagger UI: https://localhost:5001/swagger

## Running Tests

```bash
# All tests
dotnet test

# Unit tests only
dotnet test learning-testing.UnitTests

# Integration tests only (requires Docker)
dotnet test learning-testing.IntegrationTests
```

Integration tests use **Testcontainers** — they will automatically pull and run a PostgreSQL Docker container. Make sure Docker is running.

## Testing Strategy

### Unit Tests (`TodoServiceTests`)

- Test the service layer in **isolation** using mocked repositories
- No database needed — fast and reliable
- Verify business logic: DTO mapping, existence checks, system-set fields (Id, timestamps)
- Uses **Moq** to simulate repository behavior

### Integration Tests (`TodosControllerTests`)

- Test the **full stack** (HTTP → Controller → Service → Repository → Database)
- Uses `WebApplicationFactory` to create an in-memory test server
- Uses **Testcontainers** with a real PostgreSQL for realistic DB interactions
- Tests HTTP status codes, response shapes, and end-to-end flows

## Key Concepts for Students

- **`[Fact]`** — xUnit attribute marking a parameterless test method
- **AAA Pattern** — Arrange, Act, Assert (separate each with blank lines)
- **Naming convention** — `MethodName_Scenario_ExpectedBehavior` (e.g., `GetById_ShouldReturn404_WhenNotExists`)
- **DTOs** — separate the API contract from the domain model so they can evolve independently
- **Dependency Injection** — services receive their dependencies through constructors; the DI container wires everything together in `Program.cs`
