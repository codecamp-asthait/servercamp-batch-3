using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Dtos;

namespace Dukaan.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery(PaginationRequest PaginationRequest) : IQuery<PagedResponse<ProductDto>>;
