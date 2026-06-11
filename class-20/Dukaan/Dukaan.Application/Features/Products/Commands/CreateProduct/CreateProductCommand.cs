using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity) : ICommand<ErrorOr<ProductDto>>;
