using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Queries.GetProductsByCategory;

public record GetProductsByCategoryQuery(Guid CategoryId, PaginationRequest Pagination) : IQuery<ErrorOr<PagedResponse<ProductDto>>>;
