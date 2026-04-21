# ServerCamp Batch 3 - Class 9: EF Core Interceptors and Change Tracking

This project is an extension of **Class 8**, continuing our journey with **Entity Framework Core (EF Core)**. While Class 8 focused on complex relationships and Fluent API, Class 9 dives into **Interceptors**, **ChangeTracker** internals, and deep data retrieval.

## What's New in Class 9?

In this class, we enhanced the `learning-entity-framework` project with observability and better understanding of EF Core's internals.

### 1. EF Core Interceptors
Interceptors allow us to hook into EF Core operations without modifying the application logic. We implemented:
- **`SaveChangesTimingInterceptor`**: Inherits from `SaveChangesInterceptor`. It measures the time taken for `SaveChangesAsync` operations.
- **`QueryLoggingInterceptor`**: Inherits from `DbCommandInterceptor`. It captures the generated SQL commands and logs them along with their execution time.

### 2. Interceptor Registration
Registered in `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.AddInterceptors(
        new SaveChangesTimingInterceptor(),
        new QueryLoggingInterceptor()
    );
});
```

### 3. ChangeTracker Observation
The `/user` endpoint demonstrates the **Entity Lifecycle**:
- **Before Add**: Entity is not tracked.
- **After Add**: Entity enters the `Added` state.
- **After SaveChanges**: Entity transitions to the `Unchanged` state.

### 4. Advanced Data Retrieval
We added a new GET endpoint to demonstrate retrieving deep object graphs in many-to-many relationships:
- **`GET /students-courses`**: Uses `.Include()` and `.ThenInclude()` to fetch students along with their courses via the `Enrollment` join table.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/user` | Demonstrates basic add and `ChangeTracker` states. |
| POST | `/one-to-one` | Adds a User with a UserProfile. |
| POST | `/one-to-many` | Adds a User with multiple Orders. |
| POST | `/many-to-many` | Adds a Student, a Course, and an Enrollment record. |
| GET | `/students-courses` | Retrieves students with their enrolled courses. |

## Project Structure

- **`Entities/`**: Domain models (`User`, `UserProfile`, `Student`, `Course`, `Enrollment`).
- **`AppDbContext.cs`**: Context configuration using Fluent API and composite keys.
- **`SaveChangesTimingInterceptor.cs`**: Interceptor for timing save operations.
- **`QueryLoggingInterceptor.cs`**: Interceptor for logging SQL queries.
- **`Program.cs`**: Minimal API endpoints and service configuration.

## Prerequisites & Setup

1. **Database**: PostgreSQL server running (default port `5433`).
2. **Migrations**:
   - Update database: `dotnet ef database update`
3. **Packages**:
   - `Microsoft.EntityFrameworkCore`
   - `Npgsql.EntityFrameworkCore.PostgreSQL`
   - `Microsoft.EntityFrameworkCore.Design`

---
*This class is part of the ServerCamp Batch 3 curriculum.*
