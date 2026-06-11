using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler(IRepository<Product> repository)
    : ICommandHandler<CreateProductCommand, ErrorOr<ProductDto>>
{
    public async Task<ErrorOr<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            StockQuantity = request.StockQuantity
        };

        await repository.AddAsync(product);
        await repository.SaveChangesAsync();

        return MapToDto(product);
    }

    private static ProductDto MapToDto(Product p) => new(
        p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.IsActive,
        p.ProductCategories.Select(pc => pc.CategoryId).ToList());
}
