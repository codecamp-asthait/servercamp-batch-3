using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdHandler(IRepository<Category> repository)
    : IQueryHandler<GetCategoryByIdQuery, ErrorOr<CategoryDto?>>
{
    public async Task<ErrorOr<CategoryDto?>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, trackChanges: false, cancellationToken: cancellationToken);
        
        if (category is null)
            return CategoryErrors.NotFound;
        
        return MapToDto(category);
    }

    private static CategoryDto MapToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Description,
        category.ParentCategoryId,
        category.SubCategories.Where(s => s.IsActive).Select(MapToDto).ToList());
}
