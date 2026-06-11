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

namespace Dukaan.Application.Features.Cart.Commands.AddToCart;

public class AddToCartHandler(
    IRepository<CartEntity> cartRepository,
    IRepository<CartItem> cartItemRepository,
    IRepository<Product> productRepository,
    IMediator mediator)
    : ICommandHandler<AddToCartCommand, ErrorOr<CartDto>>
{
    public async Task<ErrorOr<CartDto>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError)
            return CartErrors.CustomerNotFound;

        var customerId = customerIdResult.Value;
        if (customerId is null)
            return CartErrors.CustomerNotFound;

        var cart = await GetOrCreateActiveCartAsync(customerId.Value);
        
        var product = await productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
            return ProductErrors.NotFound;
        
        if (!product.IsActive)
            return ProductErrors.NotActive;

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        var newQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        if (newQuantity > product.StockQuantity)
            return ProductErrors.InsufficientStock;

        if (existingItem != null)
        {
            existingItem.Quantity = newQuantity;
            cartItemRepository.Update(existingItem);
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            };
            await cartItemRepository.AddAsync(newItem);
            cart.Items.Add(newItem);
        }

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
