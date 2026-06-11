using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Dtos;

namespace Dukaan.Application.Features.Products.Queries.GetActiveProductsByCategory;

public record GetActiveProductsByCategoryQuery(Guid CategoryId, PaginationRequest PaginationRequest) : IQuery<PagedResponse<ProductDto>>;
