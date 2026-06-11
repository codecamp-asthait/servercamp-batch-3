using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Cart.Dtos;

namespace Dukaan.Application.Features.Cart.Commands.AddToCart;

public record AddToCartCommand(AddToCartRequestDto Request) : ICommand<CartDto>;
