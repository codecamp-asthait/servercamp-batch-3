using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Dtos;

namespace Dukaan.Application.Features.Products.Queries.GetActiveProducts;

public record GetActiveProductsQuery(PaginationRequest PaginationRequest) : IQuery<PagedResponse<ProductDto>>;
