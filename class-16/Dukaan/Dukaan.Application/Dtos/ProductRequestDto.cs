namespace Dukaan.Application.Dtos;

/// <summary>
/// Data transfer object for creating a product.
/// </summary>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">An optional description of the product.</param>
/// <param name="Price">The unit price of the product.</param>
/// <param name="ImageUrl">An optional URL for the product's image.</param>
/// <param name="StockQuantity">The initial stock quantity.</param>
public record ProductRequestDto(
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity
);

/// <summary>
/// Data transfer object representing a product in responses.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">The product description.</param>
/// <param name="Price">The product price.</param>
/// <param name="ImageUrl">The product image URL.</param>
/// <param name="StockQuantity">The current stock level.</param>
/// <param name="IsActive">Indicates whether the product is active.</param>
public record ProductResponseDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity,
    bool IsActive
);