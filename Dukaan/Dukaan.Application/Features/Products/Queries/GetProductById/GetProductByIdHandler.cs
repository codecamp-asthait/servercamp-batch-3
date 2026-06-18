using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdHandler(IRepository<Product> repository)
    : IQueryHandler<GetProductByIdQuery, ErrorOr<ProductDto?>>
{
    public async Task<ErrorOr<ProductDto?>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await repository.GetByIdAsync(request.Id, trackChanges: false, cancellationToken: cancellationToken);
        
        if (p is null)
            return ProductErrors.NotFound;
        
        return new ProductDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.PendingMediaId, p.StockQuantity, p.IsActive,
            p.ProductCategories.Select(pc => pc.CategoryId).ToList());
    }
}
