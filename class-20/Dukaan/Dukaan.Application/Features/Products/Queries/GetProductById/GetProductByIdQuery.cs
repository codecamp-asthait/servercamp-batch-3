using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products.Dtos;

namespace Dukaan.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto?>;
