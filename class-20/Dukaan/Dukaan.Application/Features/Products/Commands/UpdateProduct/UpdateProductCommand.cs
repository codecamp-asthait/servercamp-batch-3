using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid? PendingMediaId,
    int StockQuantity,
    bool IsActive) : ICommand<ErrorOr<Success>>;
