using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryHandler(IRepository<Category> repository)
    : ICommandHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, trackChanges: true);
        if (category == null) return false;

        if (category.SubCategories.Any(sc => sc.IsActive))
            throw new Exception("Cannot delete category with active sub-categories.");
        if (category.ProductLinks.Any())
            throw new Exception("Cannot delete category assigned to products.");

        category.IsActive = false;
        await repository.SaveChangesAsync();
        return true;
    }
}
