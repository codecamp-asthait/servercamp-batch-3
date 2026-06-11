using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;

namespace Dukaan.Application.Features.Cart.Commands.ClearCart;

public record ClearCartCommand() : ICommand<CartDto>;
