using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;

namespace Dukaan.Application.Features.Cart.Commands.RemoveCartItem;

public record RemoveCartItemCommand(Guid ProductId) : ICommand<CartDto>;
