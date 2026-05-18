using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;

namespace Dukaan.Application.Services;

public class ProductService(IRepository<Product> repository) : IProductService
{
    public async Task<ProductResponseDto> CreateAsync(ProductRequestDto request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            StockQuantity = request.StockQuantity
        };

        // category = tenantId
        // product = tenantId
        // order = tenantId
        // customer = tenantId

        await repository.AddAsync(product);
        // common
        await repository.SaveChangesAsync();

        // product - are you tenant based?
        // product.TenantId = TenantProvider.GetTenantId()
        // database

        return new ProductResponseDto(product.Id, product.Name, product.Description,
            product.Price, product.ImageUrl, product.StockQuantity, product.IsActive);
    }
}