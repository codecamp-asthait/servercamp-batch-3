using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;

namespace Dukaan.Application.Features.Cart.Queries.GetCart;

public record GetCartQuery() : IQuery<CartDto>;
