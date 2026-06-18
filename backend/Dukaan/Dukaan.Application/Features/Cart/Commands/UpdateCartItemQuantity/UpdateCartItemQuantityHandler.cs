using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart;
using Dukaan.Application.Features.Cart.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Features.Products;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;
using MediatR;
using CartEntity = Dukaan.Domain.Entities.Cart;

namespace Dukaan.Application.Features.Cart.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityHandler(
    IRepository<CartEntity> cartRepository,
    IRepository<CartItem> cartItemRepository,
    IRepository<Product> productRepository,
    IMediator mediator)
    : ICommandHandler<UpdateCartItemQuantityCommand, ErrorOr<CartDto>>
{
    public async Task<ErrorOr<CartDto>> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError)
            return CartErrors.CustomerNotFound;

        var customerId = customerIdResult.Value;
        
        var carts = await cartRepository.FindAsync(
            c => c.CustomerId == customerId,
            trackChanges: true,
            cancellationToken,
            c => c.Items.Select(i => i.Product));
        var cart = carts.FirstOrDefault();
        
        if (cart is null)
            return CartErrors.NotFound;

        var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (item is null)
            return CartErrors.ItemNotFound;

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken: cancellationToken);
        if (product is null)
            return ProductErrors.NotFound;
        
        if (request.Quantity > product.StockQuantity)
            return ProductErrors.InsufficientStock;

        item.Quantity = request.Quantity;
        await cartItemRepository.SaveChangesAsync(cancellationToken);

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
