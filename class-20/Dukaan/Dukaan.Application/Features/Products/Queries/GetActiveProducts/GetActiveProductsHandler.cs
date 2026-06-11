using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Products.Queries.GetActiveProducts;

public class GetActiveProductsHandler(IRepository<Product> repository)
    : IQueryHandler<GetActiveProductsQuery, PagedResponse<ProductDto>>
{
    public async Task<PagedResponse<ProductDto>> Handle(GetActiveProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            p => p.IsActive, request.PaginationRequest.PageNumber, request.PaginationRequest.PageSize, trackChanges: false,
            p => p.ProductCategories);

        return new PagedResponse<ProductDto>(items.Select(MapToDto), totalCount, request.PaginationRequest.PageNumber, request.PaginationRequest.PageSize);
    }

    private static ProductDto MapToDto(Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.IsActive,
            p.ProductCategories.Select(pc => pc.CategoryId).ToList());
}
