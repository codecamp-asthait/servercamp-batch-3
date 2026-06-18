using ErrorOr;
using Dukaan.Application.Dtos;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;

namespace Dukaan.Application.Features.Categories.Queries.GetActiveCategories;

public record GetActiveCategoriesQuery(PaginationRequest Pagination) : IQuery<ErrorOr<PagedResponse<CategoryDto>>>;
