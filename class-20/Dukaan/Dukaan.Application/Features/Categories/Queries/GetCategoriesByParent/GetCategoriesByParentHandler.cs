using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoriesByParent;

public class GetCategoriesByParentHandler(IRepository<Category> repository)
    : IQueryHandler<GetCategoriesByParentQuery, IEnumerable<CategoryDto>>
{
    public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesByParentQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.FindAsync(
            c => c.ParentCategoryId == request.ParentCategoryId && c.IsActive,
            false,
            c => c.SubCategories);

        return categories.Select(MapToDto);
    }

    private static CategoryDto MapToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Description,
        category.ParentCategoryId,
        category.SubCategories.Where(s => s.IsActive).Select(MapToDto).ToList());
}
