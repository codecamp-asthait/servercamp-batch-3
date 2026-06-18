using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Cart.Commands.ClearCart;

public record ClearCartCommand() : ICommand<ErrorOr<CartDto>>;
