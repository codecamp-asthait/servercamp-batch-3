# Design Spec: Multi-Project Restructuring (N-Tier)

**Date:** 2026-04-17
**Topic:** Transitioning from Single-Project to Multi-Project Solution
**Parent Spec:** `[spec-1]-dukaan-initial-version-design.md`
**Status:** Complete

## 1. Overview
As the Dukaan platform grows, maintaining all layers in a single project becomes unwieldy. This spec defines the migration to a multi-project solution to enforce strict architectural boundaries.

## 2. Project Structure & Responsibilities

### 2.1 dukaan.Domain (Class Library)
- **Goal:** The core "heart" of the system. Zero external dependencies.
- **Contents:**
  - `Entities/` (Tenant, Merchant, StoreBranding)
  - `Interfaces/` (ITenantEntity, IRepository<T>)
  - `Constants/` (Business constants)

### 2.2 dukaan.Application (Class Library)
- **Goal:** Orchestrates business logic and defines the "what" of the system.
- **Dependencies:** References `dukaan.Domain`.
- **Contents:**
  - `Services/` (Interfaces and implementations like `ITenantService`, `TenantService`)
  - `DTOs/` (RegisterRequest, RegisterResponse)
  - `Validators/` (FluentValidation logic)

### 2.3 dukaan.Infrastructure (Class Library)
- **Goal:** Handles the "how" of the system (persistence, external APIs).
- **Dependencies:** References `dukaan.Application` and `dukaan.Domain`.
- **Contents:**
  - `Data/` (ApplicationDbContext, Migrations)
  - `Repositories/` (Repository implementations)
  - `Services/` (Infrastructure-specific services like `TenantProvider`)

### 2.4 dukaan.Host (Web API)
- **Goal:** The entry point of the application.
- **Dependencies:** References all other projects.
- **Contents:**
  - `Controllers/` (TenantsController)
  - `Middleware/` (TenantMiddleware, ErrorHandling)
  - `Program.cs` (DI registration, middleware pipeline)
  - `appsettings.json`

## 3. Migration Plan
1. Create the new projects (`dotnet new classlib`, `dotnet new webapi`).
2. Set up project references (`dotnet add reference`).
3. Move files from the current project to their respective new locations.
4. Update namespaces across the solution.
5. Fix Dependency Injection in `Program.cs` in the new `Host` project.

## 4. Verification Strategy
- The solution must compile without errors.
- Existing unit tests must be updated to reference the new project structure.
- `dotnet test` must pass all tests across the new projects.
