using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart;
using Dukaan.Application.Features.Cart.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;
using MediatR;
using CartEntity = Dukaan.Domain.Entities.Cart;

namespace Dukaan.Application.Features.Cart.Queries.GetCart;

public class GetCartHandler(
    IRepository<CartEntity> repository,
    IMediator mediator)
    : IQueryHandler<GetCartQuery, ErrorOr<CartDto>>
{
    public async Task<ErrorOr<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError)
            return CartErrors.CustomerNotFound;

        var customerId = customerIdResult.Value;
        
        var carts = await repository.FindAsync(
            c => c.CustomerId == customerId,
            trackChanges: false,
            cancellationToken,
            c => c.Items.Select(i => i.Product));

        var cart = carts.FirstOrDefault();
        
        if (cart is null)
            return new CartDto(Guid.Empty, [], 0, 0);

        return MapToDto(cart);
    }

    private static CartDto MapToDto(CartEntity cart)
    {
        var items = cart.Items.Select(i => new CartItemDto(
            i.ProductId,
            i.Product?.Name ?? "Unknown Product",
            i.UnitPrice,
            i.Product?.Price ?? 0,
            i.Quantity,
            i.UnitPrice * i.Quantity,
            i.Product != null && i.UnitPrice != i.Product.Price
        )).ToList();

        return new CartDto(
            cart.Id,
            items,
            items.Sum(i => i.Subtotal),
            items.Sum(i => i.Quantity));
    }
}
