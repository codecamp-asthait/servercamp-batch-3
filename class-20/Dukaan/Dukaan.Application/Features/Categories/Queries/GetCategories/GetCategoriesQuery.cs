using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Categories.Dtos;

namespace Dukaan.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery(PaginationRequest PaginationRequest) : IQuery<PagedResponse<CategoryDto>>;
