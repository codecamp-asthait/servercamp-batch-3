using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesHandler(IRepository<Category> repository)
    : IQueryHandler<GetCategoriesQuery, ErrorOr<PagedResponse<CategoryDto>>>
{
    public async Task<ErrorOr<PagedResponse<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            c => true,
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
