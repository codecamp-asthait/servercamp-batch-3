using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler(IRepository<Product> repository)
    : ICommandHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
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

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.ImageUrl,
            product.StockQuantity,
            product.IsActive,
            []);
    }
}
