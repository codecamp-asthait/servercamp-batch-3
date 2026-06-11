using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdHandler(IRepository<Product> repository)
    : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await repository.GetByIdAsync(request.Id, trackChanges: false);
        return p == null
            ? null
            : new ProductDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.IsActive,
                p.ProductCategories.Select(pc => pc.CategoryId).ToList());
    }
}
