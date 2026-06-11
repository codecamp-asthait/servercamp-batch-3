using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Queries.GetActiveProducts;

public record GetActiveProductsQuery(PaginationRequest Pagination) : IQuery<ErrorOr<PagedResponse<ProductDto>>>;
