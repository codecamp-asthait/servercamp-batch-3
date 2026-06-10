# Design Spec: Hangfire Integration for Background Processing

**Status:** Pending
Date: 2026-05-07

## 1. Overview
The goal is to integrate Hangfire into the Dukaan multi-tenant system to enable background job processing and task scheduling. This integration must maintain strict tenant isolation, ensuring that background tasks operate within the correct tenant context.

## 2. Architecture & Storage

### 2.1 Storage Provider
- **Database:** PostgreSQL (sharing the main application's connection).
- **Library:** `Hangfire.PostgreSql`.
- **Schema:** Hangfire will create its own tables prefixed with `Hangfire.` in the `public` schema.

### 2.2 Host Integration
- **Service Registration:** Configured in `dukaan.Host/Program.cs` via `AddHangfire` and `AddHangfireServer`.
- **Dependency Injection:** Configured to use the standard .NET DI container.
- **Lifecycle:** The Hangfire server will run as a background service within the Web Host.

## 3. Multi-Tenant Context Flow

To ensure background jobs are tenant-aware, we will implement an automatic context flow using Hangfire Job Filters.

### 3.1 TenantJobFilter
A custom filter implementing `IClientFilter` and `IServerFilter`:
- **OnCreating (Client):** 
    - Resolve `ITenantProvider` from the current scope.
    - If a `TenantId` is present, store it as a job parameter (metadata).
- **OnPerforming (Server):** 
    - Retrieve the `TenantId` job parameter.
    - Resolve `ITenantProvider` from the job's background scope.
    - Inject the `TenantId` into the provider before the job method executes.

### 3.2 Tenant Isolation
All database queries executed within a Hangfire job will automatically benefit from the existing EF Core Global Query Filters because the `ITenantProvider` will be correctly populated by the `TenantJobFilter`.

## 4. Dashboard & Security

### 4.1 Access Control
- **Endpoint:** `/hangfire`.
- **Authorization:** Custom implementation of `IDashboardAuthorizationFilter`.

### 4.2 Security Strategy
- **Development:** Allow all local requests (`localhost`) without authentication.
- **Production:** 
    - Implement **Basic Authentication**.
    - Credentials sourced from environment variables: `HANGFIRE_DASHBOARD_USER` and `HANGFIRE_DASHBOARD_PASSWORD`.

## 5. Implementation Strategy

### 5.1 Infrastructure Changes (`dukaan.Infrastructure`)
- Create `HangfireExtensions.cs` for service registration.
- Implement `TenantJobFilter.cs`.
- Implement `HangfireDashboardAuthorizationFilter.cs`.

### 5.2 Host Changes (`dukaan.Host`)
- Update `Program.cs` to call registration methods.
- Map the Hangfire Dashboard endpoint.

### 5.3 Verification Plan
- Create a test endpoint `POST /test/enqueue-job`.
- The job should log the current `TenantId` to verify context flow.
- Verify that queries inside the job are scoped to the correct tenant.

## 6. Success Criteria
1. Background jobs can be enqueued and executed successfully.
2. Jobs automatically inherit the `TenantId` of the enqueuing request.
3. Jobs fail/retry gracefully if an exception occurs.
4. The Hangfire Dashboard is accessible and secured in production.
