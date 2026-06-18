using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Queries.GetProductsByCategory;

public class GetProductsByCategoryHandler(IRepository<Product> repository)
    : IQueryHandler<GetProductsByCategoryQuery, ErrorOr<PagedResponse<ProductDto>>>
{
    public async Task<ErrorOr<PagedResponse<ProductDto>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            p => p.ProductCategories.Any(pc => pc.CategoryId == request.CategoryId),
            request.Pagination.PageNumber,
            request.Pagination.PageSize,
            trackChanges: false,
            cancellationToken,
            p => p.ProductCategories);

        var dtos = items.Select(MapToDto).ToList();
        return new PagedResponse<ProductDto>(dtos, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }

    private static ProductDto MapToDto(Product p) => new(
        p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.PendingMediaId, p.StockQuantity, p.IsActive,
        p.ProductCategories.Select(pc => pc.CategoryId).ToList());
}
