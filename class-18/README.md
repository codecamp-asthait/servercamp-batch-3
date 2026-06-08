# Class 18 — Validation & MediatR

This class covers building a clean, layered ASP.NET Core Web API using annotation-based validation, FluentValidation, MediatR, and the CQRS pattern.

---

## What We Learned

### 1. Annotation-Based Validation
Using `System.ComponentModel.DataAnnotations` attributes (`[Required]`, `[StringLength]`, `[Range]`) directly on model/DTO properties. ASP.NET Core's model binder automatically validates and returns `400 Bad Request` via `ModelState`.

### 2. FluentValidation
Replacing annotation attributes with FluentValidation's `AbstractValidator<T>` for more expressive, testable validation rules. Registered via `AddFluentValidationAutoValidation` with `DisableDataAnnotationsValidation = true` so only FluentValidation rules apply.

### 3. MediatR & CQRS
Removing the service layer and replacing it with the mediator pattern:
- **Queries** — read-only requests (`IRequest<T>`) with dedicated handlers
- **Commands** — write operations (`IRequest<T>`) with dedicated handlers
- The controller becomes a thin dispatcher: it only calls `mediator.Send()` and returns the result

### 4. Co-located Validators on Commands
Instead of a separate DTO + validator, the command carries its own properties and its validator lives in the same file. This keeps each feature self-contained.

### 5. FluentValidation as a MediatR Pipeline Behavior
Moving validation out of the model binder and into a `IPipelineBehavior<TRequest, TResponse>`. The behavior runs all `IValidator<TRequest>` instances before the handler — the handler only executes if validation passes.

### 6. Global Exception Handler Middleware
A single `GlobalExceptionHandler` middleware catches `ValidationException` (→ 400) and any unhandled exception (→ 500), removing all try-catch blocks from controllers.

---

## Commit History

| Commit | Description |
|--------|-------------|
| `26facb2` | Initial Product CRUD with controller, service layer, `ProductCreateDto` with annotation-based validation (`[Required]`, `[StringLength]`, `[Range]`), XML docs, and `.http` test calls |
| `8bcf5ed` | Replaced annotation validation with FluentValidation. Added `ProductCreateDtoValidator`, registered `AddFluentValidationAutoValidation` with annotations disabled |
| `d90fbba` | Migrated to MediatR. Removed service layer and DTO. Added `GetAllProductsQuery`, `GetProductByIdQuery`, `CreateProductCommand` with co-located `CreateProductCommandValidator`. Controller now dispatches via `IMediator` |
| `60ec859` | Moved FluentValidation from model binder into a MediatR `ValidationBehavior<TRequest, TResponse>` pipeline. Registered via `cfg.AddOpenBehavior()`. Controller catches `ValidationException` for 400 response |
| `77a4ef5` | Added `GlobalExceptionHandler` middleware to handle `ValidationException` (400) and unhandled exceptions (500) globally. Removed try-catch from controller |

---

## Project Structure

```
learning-validation-mediatr/
├── Behaviors/
│   └── ValidationBehavior.cs       # MediatR pipeline — runs validators before handler
├── Controllers/
│   └── ProductController.cs        # Thin dispatcher — calls mediator.Send()
├── Features/
│   └── Products/
│       ├── Commands/
│       │   └── CreateProduct.cs    # Command + Validator + Handler
│       └── Queries/
│           ├── GetAllProducts.cs   # Query + Handler
│           └── GetProductById.cs   # Query + Handler
├── Middleware/
│   └── GlobalExceptionHandler.cs  # Catches ValidationException and unhandled errors
├── Product.cs                      # Domain model + in-memory ProductStore
└── Program.cs                      # DI setup: MediatR, FluentValidation, Middleware
```
