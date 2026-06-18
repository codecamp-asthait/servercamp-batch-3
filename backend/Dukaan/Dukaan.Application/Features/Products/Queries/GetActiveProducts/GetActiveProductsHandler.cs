using ErrorOr;
using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products.Dtos;

namespace Dukaan.Application.Features.Products.Queries.GetActiveProducts;

public class GetActiveProductsHandler(IRepository<Product> repository)
    : IQueryHandler<GetActiveProductsQuery, ErrorOr<PagedResponse<ProductDto>>>
{
    public async Task<ErrorOr<PagedResponse<ProductDto>>> Handle(GetActiveProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            p => p.IsActive,
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
