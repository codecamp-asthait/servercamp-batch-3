using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using MediatR;
using CartEntity = Dukaan.Domain.Entities.Cart;

namespace Dukaan.Application.Features.Cart.Queries.GetCart;

public class GetCartHandler(
    IRepository<CartEntity> cartRepository,
    IMediator mediator)
    : IQueryHandler<GetCartQuery, CartDto>
{
    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var customerId = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken)
            ?? throw new UnauthorizedAccessException("Customer context not found.");

        var results = await cartRepository.FindAsync(
            c => c.CustomerId == customerId,
            true,
            c => c.Items.Select(i => i.Product));

        var cart = results.FirstOrDefault();

        if (cart == null)
        {
            cart = new CartEntity { CustomerId = customerId };
            await cartRepository.AddAsync(cart);
            await cartRepository.SaveChangesAsync();

            var refetched = await cartRepository.FindAsync(
                c => c.CustomerId == customerId,
                true,
                c => c.Items.Select(i => i.Product));
            cart = refetched.FirstOrDefault() ?? cart;
        }

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
