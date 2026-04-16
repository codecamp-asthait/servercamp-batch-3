# ServerCamp Batch 3 - Class 8: Advanced Entity Framework Core

This project is an extension of **Class 7**, diving deeper into **Entity Framework Core (EF Core)**. While Class 7 introduced the basics of database access and change tracking, Class 8 focuses on complex entity relationships and advanced configuration using the **Fluent API**.

## What's New in Class 8?

In this class, we expanded the `learning-entity-framework` project to include:

### 1. Complex Relationships
- **One-to-One**: `User` and `UserProfile` linked via a unique foreign key.
- **One-to-Many**: `User` and `Order` where a user can have multiple orders.
- **Many-to-Many**: `Student` and `Course` managed through a join entity `Enrollment` with a composite primary key.

### 2. Fluent API Configuration
We moved beyond basic conventions and data annotations to use the **Fluent API** in `AppDbContext.cs`:
- Explicit table and column renaming (e.g., `APP_USERS`, `USER_EMAIL`).
- Property constraints like `.IsRequired()` and `.HasMaxLength()`.
- Complex relationship mapping using `.HasOne()`, `.WithOne()`, `.HasMany()`, and `.WithMany()`.
- Configuring composite keys for many-to-many join tables.

### 3. Deep Entity Tracking
The `Program.cs` now demonstrates how EF Core tracks and saves entire object graphs. When you add a `User` with an associated `UserProfile` or a list of `Orders`, EF Core automatically handles the insertion of all related entities in the correct order.

## Project Structure

- **`Entities/`**: Contains the domain models (`User.cs`, `UserProfile.cs`, `Order.cs`, `Student.cs`, `Course.cs`, `Enrollment.cs`).
- **`AppDbContext.cs`**: The heart of the EF Core configuration, utilizing the Fluent API for advanced mapping.
- **`Program.cs`**: Minimal API endpoints demonstrating how to work with related data and monitor the `ChangeTracker`.

## Prerequisites & Setup

1. **Database**: Ensure you have a PostgreSQL server running (default port `5433` as per `Program.cs`).
2. **EF Core Tools**:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
3. **Migrations**:
   - Initial migration: `dotnet ef migrations add "InitialSetup"`
   - Update database: `dotnet ef database update`

## Learning Objectives
- Mastering **Fluent API** for fine-grained control over the database schema.
- Implementing and managing various **Entity Relationships**.
- Working with **Composite Keys** in join tables.
- Understanding how EF Core handles **Cascading Saves** for related entities.

---
*This class is part of the ServerCamp Batch 3 curriculum.*
