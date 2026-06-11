namespace Dukaan.Application.Features.Products.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity,
    bool IsActive,
    List<Guid> CategoryIds
);
