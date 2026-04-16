# ServerCamp Batch 3 - Class 6: Database Access in .NET

This repository contains learning projects from Class 6 of ServerCamp Batch 3, focusing on two different ways to interact with databases in C# .NET: **ADO.NET** and **Entity Framework Core**.

## Project Structure

- **`learning-ado-net/`**: Demonstrates low-level database interaction using ADO.NET with the Npgsql provider for PostgreSQL.
- **`learning-entity-framework/`**: Demonstrates high-level database interaction using Entity Framework Core (EF Core) as an Object-Relational Mapper (ORM).

---

## 1. ADO.NET Project (`learning-ado-net`)

This project shows how to manually manage database connections, commands, and parameters.

### Features
- Using `NpgsqlConnection` to connect to PostgreSQL.
- Using `NpgsqlCommand` for raw SQL execution.
- Parameterized queries to prevent SQL injection.
- Manual connection management.

### Prerequisites & Setup
1. **Database Table**: Create the `todos` table in PostgreSQL:
   ```sql
   CREATE TABLE todos (
       id SERIAL PRIMARY KEY,
       title VARCHAR(200) NOT NULL,
       is_completed BOOLEAN DEFAULT FALSE,
       created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
   );
   ```
2. **PostgreSQL Driver**: Installed via `dotnet add package Npgsql`.
3. **Connection String**: Update the connection string in `Program.cs` to match your local PostgreSQL configuration.

---

## 2. Entity Framework Core Project (`learning-entity-framework`)

This project focuses on using EF Core to handle database operations through domain entities.

### Features
- **Code-First Approach**: Defining entities like `User` and generating database schema via migrations.
- **Change Tracking**: Demonstrates how EF Core tracks entity states (`Added`, `Unchanged`, `Modified`, etc.) before and after saving.
- **DbContext Configuration**: Dependency injection setup for PostgreSQL.

### Prerequisites & Setup
1. **EF Core Tools**: Install EF Core tools globally if you haven't:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
2. **PostgreSQL Provider**: Installed via `dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL`.
3. **Database Migrations**:
   - List migrations: `dotnet ef migrations list`
   - Add new migration: `dotnet ef migrations add "YourMigrationName"`
   - Apply migrations: `dotnet ef database update`

---

## Getting Started

1. Clone the repository.
2. Ensure you have a PostgreSQL server running locally.
3. Update connection strings in `Program.cs` for both projects.
4. Run the projects using:
   ```bash
   dotnet run --project learning-ado-net
   # OR
   dotnet run --project learning-entity-framework
   ```

## Learning Objectives
- Understanding the difference between low-level (ADO.NET) and high-level (ORM) database access.
- Learning how to use PostgreSQL as a database provider in .NET.
- Managing database schema via SQL scripts vs. EF Core Migrations.
- Understanding the role of the EF Core `ChangeTracker`.
