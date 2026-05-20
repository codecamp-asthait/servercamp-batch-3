using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;

namespace Dukaan.Application.Services;

/// <summary>
/// Service implementation for managing products.
/// </summary>
/// <param name="repository">The generic repository for Product entities.</param>
public class ProductService(IRepository<Product> repository) : IProductService
{
    /// <summary>
    /// Creates a new product and persists it to the database.
    /// </summary>
    /// <param name="request">The product details.</param>
    /// <returns>A DTO representing the created product.</returns>
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

        await repository.AddAsync(product);
        await repository.SaveChangesAsync();

        return new ProductResponseDto(product.Id, product.Name, product.Description,
            product.Price, product.ImageUrl, product.StockQuantity, product.IsActive);
    }
}