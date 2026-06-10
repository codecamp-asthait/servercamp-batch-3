# [Spec-3] Pagination and Structured Responses

## Overview
This specification defines a centralized system for handling paginated requests and responses across the Dukaan API. The goal is to provide a consistent interface for consumers and a reusable implementation for developers to prevent memory issues and improve performance when handling large datasets.

## Requirements

### 1. Paginated Request
- Implement a standard `PaginationRequest` that accepts `PageNumber` and `PageSize`.
- Default values should be `PageNumber = 1` and `PageSize = 10`.
- Validation should ensure `PageNumber >= 1` and `PageSize` is within a reasonable range (e.g., 1 to 100).

### 2. Paginated Response
- Implement a generic `PagedResponse<T>` wrapper.
- **Fields:**
    - `Items`: The collection of data for the current page.
    - `TotalCount`: The total number of records across all pages.
    - `PageNumber`: Current page.
    - `PageSize`: Number of items per page.
    - `TotalPages`: Calculated total pages.
    - `HasPreviousPage`: Boolean indicating if a previous page exists.
    - `HasNextPage`: Boolean indicating if a next page exists.

### 3. Centralized Logic
- Provide a reusable way to convert an `IQueryable<T>` into a `PagedResponse<T>` to avoid code duplication in Services.

## Architecture

### Data Models (Application Layer)
The models will reside in `dukaan.Application/Common/Models/`.

```csharp
public record PaginationRequest(int PageNumber = 1, int PageSize = 10);

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

### Data Access Integration (Infrastructure/Application Layer)
We will implement an extension method for `IQueryable<T>` to simplify the pagination process.

```csharp
public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
    this IQueryable<T> query, 
    int pageNumber, 
    int pageSize)
{
    var count = await query.CountAsync();
    var items = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResponse<T>(items, count, pageNumber, pageSize);
}
```

## Impact on Existing Modules
- **Product Catalog:** The `GET /api/products` endpoint will be updated to accept `PaginationRequest` and return `PagedResponse<ProductResponseDto>`.
- **Repository Layer:** No changes required to the Generic Repository as `IQueryable` is already exposed via `FindAsync` or `GetAllAsync`.

## Security & Performance
- **Validation:** Use FluentValidation to enforce maximum `PageSize` to prevent Resource Exhaustion (DoS) attacks where a user requests millions of rows.
- **AsNoTracking:** Pagination queries should typically use `AsNoTracking()` for performance.
