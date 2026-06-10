# [Spec-1]-[1.3] FluentValidation Integration Design

## Overview
This specification defines the requirement to integrate FluentValidation into the application's request pipeline to ensure robust input validation for DTOs.

## Requirements
- Integrate `FluentValidation` to enable input validation.
- Implement a custom Action Filter to handle validation, ensuring it executes before controller actions.
- Register all validators in the dependency injection container.
- Implement validation logic for existing and future DTOs.
- Return standardized validation error responses (e.g., 400 Bad Request with details) when validation fails.
- Adhere to the N-Tier architecture: validation should occur via the custom Action Filter.

## Architecture
- **Dependency:** NuGet package `FluentValidation`.
- **Implementation:** Configuration in `dukaan.Host` to register validators. Create a global Action Filter in `dukaan.Host` or `dukaan.Application` that resolves validators for incoming models. Validators defined in `dukaan.Application`.
