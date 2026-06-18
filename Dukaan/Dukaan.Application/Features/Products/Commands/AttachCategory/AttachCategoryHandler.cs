using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories;
using Dukaan.Application.Features.Products;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.AttachCategory;

public class AttachCategoryHandler(
    IRepository<Product> productRepository,
    IRepository<Category> categoryRepository)
    : ICommandHandler<AttachCategoryCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(AttachCategoryCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken: cancellationToken);
        
        if (product is null)
            return ProductErrors.NotFound;

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken: cancellationToken);
        
        if (category is null)
            return CategoryErrors.NotFound;

        if (!product.ProductCategories.Any(pc => pc.CategoryId == request.CategoryId))
        {
            product.ProductCategories.Add(new CategorizedProduct
            {
                ProductId = request.ProductId,
                CategoryId = request.CategoryId
            });
            await productRepository.SaveChangesAsync(cancellationToken);
        }

        return Result.Success;
    }
}
