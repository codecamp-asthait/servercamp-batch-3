using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Categories.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery(PaginationRequest Pagination) : IQuery<ErrorOr<PagedResponse<CategoryDto>>>;
