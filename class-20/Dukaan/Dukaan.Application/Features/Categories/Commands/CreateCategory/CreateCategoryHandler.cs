using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryHandler(IRepository<Category> repository)
    : ICommandHandler<CreateCategoryCommand, ErrorOr<CategoryDto>>
{
    public async Task<ErrorOr<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parent = await repository.GetByIdAsync(request.ParentCategoryId.Value);
            if (parent is null)
                return CategoryErrors.ParentNotFound;
        }

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId
        };

        await repository.AddAsync(category);
        await repository.SaveChangesAsync();

        return MapToDto(category);
    }

    private static CategoryDto MapToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Description,
        category.ParentCategoryId,
        category.SubCategories.Where(s => s.IsActive).Select(MapToDto).ToList());
}
