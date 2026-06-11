using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity
) : ICommand<bool>;
