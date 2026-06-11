using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Cart.Dtos;
using Dukaan.Application.Features.Cart.Commands.RemoveCartItem;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using MediatR;
using CartEntity = Dukaan.Domain.Entities.Cart;

namespace Dukaan.Application.Features.Cart.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityHandler(
    IRepository<CartEntity> cartRepository,
    IRepository<CartItem> cartItemRepository,
    IRepository<Product> productRepository,
    IMediator mediator)
    : ICommandHandler<UpdateCartItemQuantityCommand, CartDto>
{
    public async Task<CartDto> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            var removeCommand = new RemoveCartItemCommand(request.ProductId);
            return await mediator.Send(removeCommand, cancellationToken);
        }

        var customerId = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken)
            ?? throw new UnauthorizedAccessException("Customer context not found.");

        var cart = await GetOrCreateActiveCartAsync(customerId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId)
            ?? throw new KeyNotFoundException("Item not found in cart.");

        var product = await productRepository.GetByIdAsync(request.ProductId)
            ?? throw new KeyNotFoundException("Product not found.");

        if (request.Quantity > product.StockQuantity)
            throw new InvalidOperationException("Requested quantity exceeds available stock.");

        item.Quantity = request.Quantity;
        cartItemRepository.Update(item);
        await cartItemRepository.SaveChangesAsync();

        return MapToDto(cart);
    }

    private async Task<CartEntity> GetOrCreateActiveCartAsync(Guid customerId)
    {
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
            return await GetOrCreateActiveCartAsync(customerId);
        }

        return cart;
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
