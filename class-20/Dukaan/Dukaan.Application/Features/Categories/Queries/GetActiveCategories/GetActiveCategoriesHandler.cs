using ErrorOr;
using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;

namespace Dukaan.Application.Features.Categories.Queries.GetActiveCategories;

public class GetActiveCategoriesHandler(IRepository<Category> repository)
    : IQueryHandler<GetActiveCategoriesQuery, ErrorOr<PagedResponse<CategoryDto>>>
{
    public async Task<ErrorOr<PagedResponse<CategoryDto>>> Handle(GetActiveCategoriesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            c => c.ParentCategoryId == null && c.IsActive,
            request.Pagination.PageNumber,
            request.Pagination.PageSize,
            trackChanges: false,
            cancellationToken: cancellationToken,
           c => c.SubCategories);

        var dtos = items.Select(MapToDto).ToList();
        return new PagedResponse<CategoryDto>(dtos, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }

    private static CategoryDto MapToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Description,
        category.ParentCategoryId,
        category.SubCategories.Where(s => s.IsActive).Select(MapToDto).ToList());
}
