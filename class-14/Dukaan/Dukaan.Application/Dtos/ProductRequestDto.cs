namespace Dukaan.Application.Dtos;

public record ProductRequestDto(
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity
);

public record ProductResponseDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity,
    bool IsActive
);