using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryHandler(IRepository<Category> repository)
    : ICommandHandler<DeleteCategoryCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        
        if (category is null)
            return CategoryErrors.NotFound;

        if (category.SubCategories.Any())
            return CategoryErrors.HasSubCategories;

        if (category.ProductLinks.Any())
            return CategoryErrors.HasProducts;

        repository.Remove(category);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
