using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoriesByParent;

public class GetCategoriesByParentHandler(IRepository<Category> repository)
    : IQueryHandler<GetCategoriesByParentQuery, ErrorOr<IEnumerable<CategoryDto>>>
{
    public async Task<ErrorOr<IEnumerable<CategoryDto>>> Handle(GetCategoriesByParentQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.FindAsync(
            c => c.ParentCategoryId == request.ParentId && c.IsActive,
            trackChanges: false,
            cancellationToken: cancellationToken);

        return categories.Select(MapToDto).ToList();
    }

    private static CategoryDto MapToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Description,
        category.ParentCategoryId,
        category.SubCategories.Where(s => s.IsActive).Select(MapToDto).ToList());
}
