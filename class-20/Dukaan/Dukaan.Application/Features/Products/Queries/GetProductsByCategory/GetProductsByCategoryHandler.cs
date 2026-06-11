using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Products.Queries.GetProductsByCategory;

public class GetProductsByCategoryHandler(IRepository<Product> repository)
    : IQueryHandler<GetProductsByCategoryQuery, PagedResponse<ProductDto>>
{
    public async Task<PagedResponse<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            p => p.ProductCategories.Any(pc => pc.CategoryId == request.CategoryId),
            request.PaginationRequest.PageNumber,
            request.PaginationRequest.PageSize,
            trackChanges: false,
            p => p.ProductCategories);

        var dtos = items.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.ImageUrl,
            p.StockQuantity,
            p.IsActive,
            p.ProductCategories.Select(pc => pc.CategoryId).ToList()));

        return new PagedResponse<ProductDto>(dtos, totalCount, request.PaginationRequest.PageNumber, request.PaginationRequest.PageSize);
    }
}
