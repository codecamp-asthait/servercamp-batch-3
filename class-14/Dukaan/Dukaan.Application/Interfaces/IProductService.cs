using Dukaan.Application.Dtos;

namespace Dukaan.Application.Interfaces;

public interface IProductService
{
    Task<ProductResponseDto> CreateAsync(ProductRequestDto request);
}