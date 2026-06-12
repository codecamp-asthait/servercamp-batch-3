using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.DetachCategory;

public class DetachCategoryHandler(IRepository<Product> repository)
    : ICommandHandler<DetachCategoryCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DetachCategoryCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.ProductId, cancellationToken: cancellationToken);
        
        if (product is null)
            return ProductErrors.NotFound;

        var productCategory = product.ProductCategories
            .FirstOrDefault(pc => pc.CategoryId == request.CategoryId);
        
        if (productCategory is not null)
        {
            product.ProductCategories.Remove(productCategory);
            await repository.SaveChangesAsync(cancellationToken);
        }

        return Result.Success;
    }
}
