# Conventions: Dukaan

Practical coding conventions with examples. For architecture and project structure, see `AGENTS.md`.

---

## Backend (.NET)

### File & Folder Naming

| Item | Convention | Example |
|------|------------|---------|
| Files | PascalCase | `CreateProductCommand.cs` |
| Folders | PascalCase | `Features/Products/` |
| Namespaces | Match folder path | `Dukaan.Application.Features.Products.Create` |

### CQRS Pattern (MediatR)

**Commands** (write operations):
```csharp
// Request
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<ProductDto>>;

// Handler
public class CreateProductHandler(IProductRepository repo) 
    : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request, 
        CancellationToken ct)
    {
        // validation, business logic, persistence
        return Result.Success(dto);
    }
}
```

**Queries** (read operations):
```csharp
public record GetProductQuery(string Id) : IRequest<Result<ProductDto>>;
```

**Naming**: `{Action}{Entity}Command` / `{Action}{Entity}Query` / `{Action}{Entity}Handler`

### Validation (FluentValidation)

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

Validators live alongside their commands in `Features/{Entity}/`.

### Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetAll(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsQuery(pageNumber, pageSize);
        return await mediator.Send(query);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductCommand command)
    {
        return await mediator.Send(command);
    }
}
```

**Rules**:
- Primary constructor for DI
- One-line delegation to MediatR
- Return `ActionResult<T>` for proper HTTP status codes
- No business logic in controllers

### Entities

```csharp
public class Product : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public Guid TenantId { get; set; }
    
    // Navigation properties
    public ICollection<Category> Categories { get; set; } = [];
}
```

**Rules**:
- Inherit from `BaseEntity` (provides `Id`, `CreatedAt`, `UpdatedAt`)
- Implement `ITenantEntity` for multi-tenancy
- Use collection initializers `= []` for navigation properties
- Properties use `{ get; set; }` (not init-only)

### DTOs

```csharp
public record ProductDto(
    string Id,
    string Name,
    decimal Price,
    int StockQuantity,
    bool IsActive
);
```

**Rules**:
- Use `record` for immutability
- Suffix with `Dto` (e.g., `ProductDto`, `CategoryDto`)
- Match API response shape exactly

### Error Handling (ErrorOr)

```csharp
public async Task<Result<ProductDto>> Handle(...)
{
    var product = await repo.GetByIdAsync(request.Id);
    if (product is null)
        return Result.Failure(new Error("Product.NotFound", "Product not found"));
    
    return Result.Success(product.ToDto());
}
```

**Rules**:
- Use `ErrorOr` library for domain errors
- Error format: `"Entity.Reason"` (e.g., `"Product.NotFound"`)
- Return `Result<T>` from handlers, not exceptions

### Repository Pattern

**Interface** (in Application layer):
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
```

**Implementation** (in Infrastructure layer):
```csharp
public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }
}
```

**Rules**:
- `AsNoTracking()` for read-only queries
- CancellationToken on all async methods
- Generic `IRepository<T>` for common operations
- Specialized repositories for complex queries

### Dependency Injection

```csharp
// In Application layer
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}

// In Infrastructure layer
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IRepository<Product>, Repository<Product>>();
        return services;
    }
}
```

**Rules**:
- Each layer has a `DependencyInjection` static class
- Extension method pattern: `Add{Layer}Services()`
- Chain in `Program.cs`: `AddApplicationServices()` → `AddInfrastructureServices()`

---

## Frontend (Next.js + TypeScript)

### File & Folder Naming

| Item | Convention | Example |
|------|------------|---------|
| Components | kebab-case | `product-card.tsx` |
| Component names | PascalCase | `ProductCard` |
| Hooks | camelCase | `useProducts.ts` |
| API modules | camelCase | `api.ts` |
| Types | camelCase | `types.ts` |

### Module Structure

```
src/modules/{domain}/{feature}/
  api.ts              # API functions
  hooks.ts            # TanStack Query hooks
  types.ts            # TypeScript interfaces
  components/         # React components
    {feature}-view.tsx
    {feature}-card.tsx
    {feature}-table.tsx
    columns.tsx       # TanStack Table columns (if applicable)
```

### API Layer

```typescript
// api.ts
export const productsApi = {
  getAll: (pageNumber: number, pageSize = 10) =>
    http<PagedResponse<ProductDto>>(
      `/api/products?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      { headers: authHeaders() }
    ),

  create: (data: CreateProductRequest) =>
    http<ProductDto>("/api/products", {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify(data),
    }),
};
```

**Rules**:
- Group API functions in a `const` object: `productsApi`, `cartApi`, `authApi`
- Use `http<T>()` generic wrapper for type safety
- `authHeaders()` for merchant APIs (JWT token)
- `tenantHeaders(slug)` for store APIs (x-tenant-slug header)
- Override `baseUrl` for different services (e.g., media service on port 5002)

### TanStack Query Hooks

```typescript
// hooks.ts
export function useProducts(pageNumber: number, pageSize = 10) {
  return useQuery({
    queryKey: ["products", pageNumber, pageSize],
    queryFn: () => productsApi.getAll(pageNumber, pageSize),
  });
}

export function useCreateProduct(onSuccess: () => void) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateProductRequest) => productsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      onSuccess();
    },
  });
}
```

**Rules**:
- Wrap every API call in a custom hook: `useXxx`
- Query keys: hierarchical array `["scope", "id", "resource"]`
- Mutations: accept `onSuccess` callback from component
- Invalidate queries after mutations: `queryClient.invalidateQueries()`
- Use `enabled` flag to prevent queries when data is missing

### TypeScript Types

```typescript
// types.ts
export interface ProductDto {
  id: string;
  name: string;
  price: number;
  stockQuantity: number;
  isActive: boolean;
}

export interface CreateProductRequest {
  name: string;
  price: number;
  stockQuantity: number;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

**Rules**:
- Use `interface` (not `type`) for data shapes
- Suffix DTOs: `ProductDto`, `CategoryDto`
- Suffix requests: `CreateProductRequest`, `UpdateProductRequest`
- Nullable fields: `description: string | null`
- IDs are `string` (UUIDs from backend)
- Generic `PagedResponse<T>` for paginated results

### Components

```typescript
// Named export, not default
export function ProductCard({ product }: { product: ProductDto }) {
  return (
    <div className="rounded-lg border p-4">
      <h3 className="font-semibold">{product.name}</h3>
      <p className="text-sm text-muted-foreground">{product.price}</p>
    </div>
  );
}
```

**Rules**:
- Named exports from modules (not `export default`)
- `page.tsx` re-exports as default: `export { ProductCard as default }`
- Inline props typing for simple components: `{ product }: { product: ProductDto }`
- `interface Props` for complex components with multiple props
- `"use client"` directive at top for interactive components (hooks, events)
- Pure presentational components don't need `"use client"`

### Error Handling

```typescript
function errorMessage(error: Error) {
  if (error instanceof HttpError) {
    if (error.status === 401) return "Invalid credentials";
    if (error.status === 404) return "Not found";
    if (error.status === 409) return "Already exists";
  }
  return error.message;
}

// Usage in component
{error && <p className="text-red-500 text-sm">{errorMessage(error)}</p>}
```

**Rules**:
- Check `error instanceof HttpError` for typed error handling
- Map HTTP status codes to user-friendly messages
- Inline error rendering for forms (red text below fields)
- Toast notifications for action errors: `toast.error("Failed to add to cart")`
- Skeleton placeholders for data loading states

### Auth Patterns

**Merchant auth** (single global token):
```typescript
const authHeaders = () => ({
  "Content-Type": "application/json",
  Authorization: `Bearer ${localStorageService.getToken()}`,
});
```

**Customer auth** (per-tenant tokens):
```typescript
const tenantHeaders = (slug: string) => ({
  "Content-Type": "application/json",
  "x-tenant-slug": slug,
});

// Token storage
localStorageService.setCustomerToken(slug, token);
```

**Rules**:
- Merchant: single `token` key in localStorage
- Customer: namespaced per tenant: `customer_token_{slug}`
- SSR-safe access: `isBrowser ? localStorage.getItem(key) : null`
- Three-state auth guard: `undefined` = loading, `null` = redirect, `string` = render

### Styling

```typescript
// Tailwind utilities + cn() helper
import { cn } from "@/lib/utils";

<div className={cn("rounded-lg border", isActive && "border-primary")} />

// shadcn/ui variants with CVA
const buttonVariants = cva("btn-base", {
  variants: {
    variant: {
      default: "bg-primary text-primary-foreground",
      outline: "border border-input bg-background",
    },
    size: {
      default: "h-9 px-4 py-2",
      sm: "h-8 px-3 text-xs",
    },
  },
});
```

**Rules**:
- Tailwind CSS v4 with oklch color system
- `cn()` helper for conditional classes (combines `clsx` + `tailwind-merge`)
- No CSS modules — Tailwind only
- shadcn/ui components with `@base-ui/react` primitives
- Icons: `lucide-react` with named imports

### Route Pages

```typescript
// src/app/(merchant)/merchant/products/page.tsx
export { ProductsView as default } from "@/modules/merchant/products/components/products-view";
```

**Rules**:
- Route pages are one-liners that re-export from modules
- Zero logic in `page.tsx` files
- Route groups: `(store)` for customer-facing, `(merchant)` for admin
- Dynamic segments: `[slug]` for tenant, `[id]` for resources

---

## Testing

### Backend (xUnit)

```csharp
public class ProductTests
{
    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateProductCommand("Test Product", 99.99m);
        var handler = new CreateProductHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test Product");
    }
}
```

**Rules**:
- xUnit + FluentAssertions
- Arrange-Act-Assert pattern
- Test naming: `{Method}_{Scenario}_{ExpectedResult}`
- Mock dependencies with Moq or NSubstitute

### Frontend (Jest + Testing Library)

```typescript
// __tests__/product-card.test.tsx
import { render, screen } from "@testing-library/react";
import { ProductCard } from "../components/product-card";

describe("ProductCard", () => {
  it("renders product name and price", () => {
    const product = { id: "1", name: "Test", price: 99.99 };
    render(<ProductCard product={product} />);
    
    expect(screen.getByText("Test")).toBeInTheDocument();
    expect(screen.getByText("$99.99")).toBeInTheDocument();
  });
});
```

**Rules**:
- Jest + @testing-library/react
- Tests in `__tests__/` co-located with components
- Query by role/text, not by test ID
- Mock Next.js navigation: `jest.mock("next/navigation", ...)`

---

## Git & Documentation

### .gitignore

Each project has its own `.gitignore`:
- Backend: `dotnet new gitignore` + `.env`, `appsettings.Development.json`, `docs/*`
- Frontend: Next.js template + `.env*`, `docs/*`, `AGENTS.md`

### Documentation

- `docs/*` is gitignored in all projects (local-only)
- Specs/plans follow naming: `[spec-1]-feature-name.md`, `[plan-1]-planning-topic.md`
- Each service maintains its own `docs/superpowers/` folder

---

## Quick Reference

| Task | Command |
|------|---------|
| Start backend (Docker) | `docker-compose up -d` |
| Start frontend | `npm run dev` |
| Run backend tests | `dotnet test` |
| Run frontend tests | `npm test` |
| Lint frontend | `npm run lint` |
| Build frontend | `npm run build` |

---

## Summary

**Backend**: Clean Architecture, CQRS with MediatR, FluentValidation, ErrorOr, repository pattern, primary constructors, file-scoped namespaces.

**Frontend**: Module-based structure, TanStack Query for state, `http<T>()` wrapper, named exports, Tailwind + shadcn/ui, SSR-safe auth guards.

**Testing**: xUnit + FluentAssertions (backend), Jest + Testing Library (frontend).

**Git**: Per-project `.gitignore`, docs gitignored, conventional spec/plan naming.
