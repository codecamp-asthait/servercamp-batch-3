using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Cart.Queries.GetCart;

public record GetCartQuery() : IQuery<ErrorOr<CartDto>>;
