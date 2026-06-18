using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Cart.Commands.AddToCart;

public record AddToCartCommand(Guid ProductId, int Quantity) : ICommand<ErrorOr<CartDto>>;
