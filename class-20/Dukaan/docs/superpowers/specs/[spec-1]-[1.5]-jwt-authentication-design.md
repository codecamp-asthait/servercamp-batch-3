# [Spec-1]-[1.5]-JWT-Authentication-Design

## Overview
This specification defines the JWT-based authentication system using ASP.NET Core Identity, ensuring secure multi-tenant sessions and providing authenticated Swagger documentation.

## Requirements
- Use ASP.NET Core Identity (`UserManager`, `SignInManager`) for authentication.
- Implement `IAuthService` to handle token generation and business logic.
- Configure `JwtBearer` authentication in the pipeline.
- Include `TenantId` as a mandatory claim in the JWT to maintain multi-tenant scope.
- Secure Swagger UI with a JWT Bearer token security definition.
- Use `record` types for all DTOs to ensure immutability.

## Architecture
- **Controllers (Presentation):** Must use DTOs for input (`LoginRequestDTO`, `RegisterRequestDTO`) and output (`AuthResponseDTO`). Controllers are thin and delegate logic to `IAuthService`.
- **Services (Business Logic):** `IAuthService` handles authentication business rules, token generation, and tenant-specific claim inclusion.
- **Data Access:** ASP.NET Core Identity handles persistence; `AuthService` leverages the standard Identity services.

## Security & Multi-Tenancy
- **Tenant Isolation:** The JWT issued upon login MUST contain a custom claim `tenant_id` corresponding to the merchant's `TenantId`.
- **Swagger Security:** Configure `SwaggerGen` with `OpenApiSecurityScheme` (Bearer format) and `OpenApiSecurityRequirement`.

## API Endpoints (Controllers)
- `POST /api/auth/login`: Accepts `LoginRequestDTO`, returns `AuthResponseDTO` (with Token).
- `POST /api/auth/register`: (Updates) Accepts `RegisterRequestDTO` and returns `AuthResponseDTO`.
