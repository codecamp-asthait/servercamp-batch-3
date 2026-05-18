using Dukaan.Application.Dtos;

namespace Dukaan.Application.Interfaces;

/// <summary>
/// Interface for product management services.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Asynchronously creates a new product.
    /// </summary>
    /// <param name="request">The product creation request data.</param>
    /// <returns>A task representing the asynchronous operation, containing the created product response.</returns>
    Task<ProductResponseDto> CreateAsync(ProductRequestDto request);
}