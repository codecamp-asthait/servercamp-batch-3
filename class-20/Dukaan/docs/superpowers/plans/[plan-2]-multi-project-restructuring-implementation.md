# Multi-Project Restructuring Implementation Plan

Status: Completed

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Transition the current single-project solution into a multi-project N-Tier architecture to enforce separation of concerns and maintainability.

**Architecture:** 
- **Domain:** Entities and core interfaces (zero dependencies).
- **Application:** Business logic and DTOs (depends on Domain).
- **Infrastructure:** Persistence and external integrations (depends on Application and Domain).
- **Host:** API entry point, DI wiring, and Controllers (depends on all others).

**Tech Stack:** .NET 10, EF Core, ASP.NET Core Identity, PostgreSQL.

---

### Task 1: New Project Initialization

**Files:**
- Create: `dukaan.Domain/dukaan.Domain.csproj`
- Create: `dukaan.Application/dukaan.Application.csproj`
- Create: `dukaan.Infrastructure/dukaan.Infrastructure.csproj`
- Create: `dukaan.Host/dukaan.Host.csproj`

- [ ] **Step 1: Create the Class Libraries and Web API project**
Run these commands from the root:
```bash
dotnet new classlib -n dukaan.Domain
dotnet new classlib -n dukaan.Application
dotnet new classlib -n dukaan.Infrastructure
dotnet new webapi -n dukaan.Host
```

- [ ] **Step 2: Add project references**
```bash
# Application depends on Domain
dotnet add dukaan.Application/dukaan.Application.csproj reference dukaan.Domain/dukaan.Domain.csproj

# Infrastructure depends on Application and Domain
dotnet add dukaan.Infrastructure/dukaan.Infrastructure.csproj reference dukaan.Application/dukaan.Application.csproj
dotnet add dukaan.Infrastructure/dukaan.Infrastructure.csproj reference dukaan.Domain/dukaan.Domain.csproj

# Host depends on all
dotnet add dukaan.Host/dukaan.Host.csproj reference dukaan.Infrastructure/dukaan.Infrastructure.csproj
dotnet add dukaan.Host/dukaan.Host.csproj reference dukaan.Application/dukaan.Application.csproj
dotnet add dukaan.Host/dukaan.Host.csproj reference dukaan.Domain/dukaan.Domain.csproj
```

- [ ] **Step 3: Add external packages to correct projects**
```bash
# Domain (needs Identity for Merchant)
dotnet add dukaan.Domain/dukaan.Domain.csproj package Microsoft.Extensions.Identity.Stores

# Infrastructure (Persistence)
dotnet add dukaan.Infrastructure/dukaan.Infrastructure.csproj package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add dukaan.Infrastructure/dukaan.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL

# Host (API)
dotnet add dukaan.Host/dukaan.Host.csproj package Microsoft.AspNetCore.OpenApi
```

- [ ] **Step 4: Update Solution file**
```bash
dotnet sln dukaan.sln add dukaan.Domain/dukaan.Domain.csproj dukaan.Application/dukaan.Application.csproj dukaan.Infrastructure/dukaan.Infrastructure.csproj dukaan.Host/dukaan.Host.csproj
```

- [ ] **Step 5: Commit**
```bash
git add . && git commit -m "chore: initialize multi-project solution structure"
```

---

### Task 2: Migrate Domain Layer

**Files:**
- Move: `Models/Entities/*` -> `dukaan.Domain/Entities/`
- Move: `Models/Interfaces/*` -> `dukaan.Domain/Interfaces/`
- Move: `Data/Repositories/IRepository.cs` -> `dukaan.Domain/Interfaces/`

- [ ] **Step 1: Move files**
```bash
mkdir -p dukaan.Domain/Entities dukaan.Domain/Interfaces
mv Models/Entities/* dukaan.Domain/Entities/
mv Models/Interfaces/* dukaan.Domain/Interfaces/
mv Data/Repositories/IRepository.cs dukaan.Domain/Interfaces/
```

- [ ] **Step 2: Update Namespaces in Domain**
Update all moved files to use `namespace dukaan.Domain.[Folder]`.

- [ ] **Step 3: Commit**
```bash
git add dukaan.Domain/ && git commit -m "feat: migrate domain entities and interfaces"
```

---

### Task 3: Migrate Application Layer

**Files:**
- Move: `Models/DTOs/*` -> `dukaan.Application/DTOs/`
- Move: `Services/*` -> `dukaan.Application/Services/`

- [ ] **Step 1: Move files**
```bash
mkdir -p dukaan.Application/DTOs dukaan.Application/Services
mv Models/DTOs/* dukaan.Application/DTOs/
mv Services/* dukaan.Application/Services/
```

- [ ] **Step 2: Update Namespaces in Application**
Update all moved files to use `namespace dukaan.Application.[Folder]`. Update internal references to `dukaan.Domain`.

- [ ] **Step 3: Commit**
```bash
git add dukaan.Application/ && git commit -m "feat: migrate application services and DTOs"
```

---

### Task 4: Migrate Infrastructure Layer

**Files:**
- Move: `Data/*` -> `dukaan.Infrastructure/Data/` (excluding Repositories)
- Move: `Data/Repositories/Repository.cs` -> `dukaan.Infrastructure/Data/Repositories/`
- Move: `Infrastructure/*` -> `dukaan.Infrastructure/Services/`

- [ ] **Step 1: Move files**
```bash
mkdir -p dukaan.Infrastructure/Data dukaan.Infrastructure/Data/Repositories dukaan.Infrastructure/Services
mv Data/ApplicationDbContext.cs dukaan.Infrastructure/Data/
mv Data/Repositories/Repository.cs dukaan.Infrastructure/Data/Repositories/
mv Infrastructure/* dukaan.Infrastructure/Services/
```

- [ ] **Step 2: Update Namespaces in Infrastructure**
Update all moved files to use `namespace dukaan.Infrastructure.[Folder]`. Update internal references to `dukaan.Domain` and `dukaan.Application`.

- [ ] **Step 3: Commit**
```bash
git add dukaan.Infrastructure/ && git commit -m "feat: migrate infrastructure data and services"
```

---

### Task 5: Migrate Host & Cleanup

**Files:**
- Move: `Controllers/*` -> `dukaan.Host/Controllers/`
- Move: `Program.cs` -> `dukaan.Host/Program.cs`
- Move: `appsettings.json` -> `dukaan.Host/appsettings.json`
- Delete: `dukaan.csproj` (old root project)

- [ ] **Step 1: Move files**
```bash
mv Controllers/* dukaan.Host/Controllers/
mv Program.cs dukaan.Host/Program.cs
mv appsettings.json dukaan.Host/appsettings.json
```

- [ ] **Step 2: Update Host project**
Update `Program.cs` namespaces and ensure all project references are correct.

- [ ] **Step 3: Cleanup old project**
```bash
dotnet sln dukaan.sln remove dukaan.csproj
rm dukaan.csproj
rm -rf Models/ Data/ Services/ Infrastructure/ Controllers/
```

- [ ] **Step 4: Update Tests Reference**
```bash
dotnet add dukaan.Tests/dukaan.Tests.csproj reference dukaan.Host/dukaan.Host.csproj
# (Or reference specific projects as needed for unit testing)
```

- [ ] **Step 5: Verify build**
Run `dotnet build` from root.

- [ ] **Step 6: Commit**
```bash
git add . && git commit -m "chore: complete multi-project restructuring and cleanup"
```
