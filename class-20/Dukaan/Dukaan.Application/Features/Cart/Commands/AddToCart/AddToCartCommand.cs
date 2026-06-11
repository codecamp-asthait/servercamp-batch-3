using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;

namespace Dukaan.Application.Features.Cart.Commands.AddToCart;

public record AddToCartCommand(Guid ProductId, int Quantity) : ICommand<CartDto>;
