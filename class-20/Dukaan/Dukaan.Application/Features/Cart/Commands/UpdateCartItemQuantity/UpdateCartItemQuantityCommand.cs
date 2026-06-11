using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;

namespace Dukaan.Application.Features.Cart.Commands.UpdateCartItemQuantity;

public record UpdateCartItemQuantityCommand(Guid ProductId, int Quantity) : ICommand<CartDto>;
