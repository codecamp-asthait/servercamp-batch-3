using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesHandler(IRepository<Category> repository)
    : IQueryHandler<GetCategoriesQuery, PagedResponse<CategoryDto>>
{
    public async Task<PagedResponse<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            c => c.ParentCategoryId == null && c.IsActive,
            request.PaginationRequest.PageNumber,
            request.PaginationRequest.PageSize,
            false,
            c => c.SubCategories);

        return new PagedResponse<CategoryDto>(
            items.Select(MapToDto),
            totalCount,
            request.PaginationRequest.PageNumber,
            request.PaginationRequest.PageSize);
    }

    private static CategoryDto MapToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Description,
        category.ParentCategoryId,
        category.SubCategories.Where(s => s.IsActive).Select(MapToDto).ToList());
}
