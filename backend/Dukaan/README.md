# Dukaan - Multi-Tenant E-Commerce Platform

Dukaan is a robust, multi-tenant e-commerce platform built with ASP.NET Core. It provides a foundation for merchants to register their own stores (tenants) and manage them independently.

## Features

- **Multi-Tenancy**: Support for multiple isolated stores within a single application instance.
- **Product Management**: Robust product catalog management scoped to individual tenants.
- **Category Management**: Hierarchical product categories support with tree structure.
- **Identity Management**: Integrated ASP.NET Core Identity for secure merchant authentication and authorization.
- **Clean Architecture**: Organized into logical layers (Domain, Application, Infrastructure, Host) for maintainability and scalability.
- **Generic Repository Pattern**: Standardized data access layer for consistency across entities.
- **PostgreSQL Integration**: High-performance relational database storage using Entity Framework Core.
- **Logging**: Structured logging integrated using Serilog.

## Multi-Tenancy Implementation

Dukaan uses a modern approach to handle multi-tenancy seamlessly:

- **Tenant Identification**: The `TenantProvider` service extracts the `tenant_id` from the authenticated user's JWT claims. This ensures that every request is tied to a specific tenant context.
- **Automatic Scoping**: Entities that implement `ITenantEntity` (like `Product` and `Category`) are automatically scoped.
- **EF Core Global Query Filters**: Data isolation is enforced at the database level using EF Core global query filters, ensuring tenants can only access their own data.
- **EF Core Interceptor**: The `TenantInterceptor` automatically injects the current `TenantId` into entities during the `SaveChanges` operation. This prevents manual assignment errors and ensures data isolation at the application level.

## Project Structure

```text
Dukaan/
├── Dukaan.Domain/             # Core business models and abstractions
│   ├── Entities/              # Database entities (e.g., Tenant.cs, Product.cs, Category.cs)
│   └── Interfaces/            # Fundamental domain interfaces
├── Dukaan.Application/        # Application logic and DTOs
│   ├── Dtos/                  # Request/Response data structures
│   ├── Interfaces/            # Service abstractions (e.g., IProductService.cs, ICategoryService.cs)
│   └── Services/              # Core business logic implementation
├── Dukaan.Infrastructure/     # External concerns and data persistence
│   ├── Data/
│   │   ├── DbContext/         # EF Core context definition
│   │   ├── Model/             # Infrastructure-specific models (e.g., Merchant.cs)
│   │   ├── Repositories/      # Generic and specific repository implementations
│   │   └── Services/          # Data-centric services (e.g., UserService.cs)
│   └── Migrations/            # EF Core database migrations
└── Dukaan.Host/               # API Entry Point and Configuration
    ├── Controllers/           # REST API Endpoints
    ├── Program.cs             # DI registration and middleware pipeline
    └── appsettings.json       # Environment-specific configuration
```

## Technologies Used

- **Framework**: ASP.NET Core 10.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Identity**: Microsoft.AspNetCore.Identity.EntityFrameworkCore
- **Logging**: Serilog
- **Documentation**: OpenAPI (Swagger) with Swagger UI

## Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL Server

### Setup

1.  **Clone the repository**:
    ```bash
    git clone <repository-url>
    ```

2.  **Configure the database**:
    Update the `DefaultConnection` in `appsettings.json` with your PostgreSQL credentials.

3.  **Run Migrations**:
    ```bash
    dotnet ef database update --project Dukaan.Infrastructure --startup-project Dukaan.Host
    ```

4.  **Run the Application**:
    ```bash
    dotnet run --project Dukaan.Host/Dukaan.Host.csproj
    ```

## API Documentation

Once the application is running in development mode, you can access the interactive Swagger UI at:
`https://localhost:<port>/swagger/index.html`

The OpenAPI JSON specification is available at:
`https://localhost:<port>/openapi/v1.json`
