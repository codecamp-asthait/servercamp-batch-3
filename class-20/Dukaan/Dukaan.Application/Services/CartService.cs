using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;

namespace Dukaan.Application.Services;

/// <summary>
/// Service implementation for managing customer shopping carts.
/// </summary>
public class CartService(
    IRepository<Cart> cartRepository,
    IRepository<CartItem> cartItemRepository,
    IRepository<Product> productRepository,
    ICustomerService customerService) : ICartService
{
    public async Task<CartResponseDto> GetCartAsync()
    {
        var cart = await GetOrCreateActiveCartAsync();
        return MapToDto(cart);
    }

    public async Task<CartResponseDto> AddItemAsync(AddToCartRequestDto request)
    {
        var cart = await GetOrCreateActiveCartAsync();
        var product = await productRepository.GetByIdAsync(request.ProductId)
            ?? throw new KeyNotFoundException("Product not found.");

        if (!product.IsActive) throw new InvalidOperationException("Product is not active.");
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        var newQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        if (newQuantity > product.StockQuantity)
            throw new InvalidOperationException("Requested quantity exceeds available stock.");

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

    public async Task<CartResponseDto> UpdateQuantityAsync(Guid productId, UpdateQuantityRequestDto request)
    {
        if (request.Quantity <= 0) return await RemoveItemAsync(productId);

        var cart = await GetOrCreateActiveCartAsync();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new KeyNotFoundException("Item not found in cart.");

        var product = await productRepository.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException("Product not found.");

        if (request.Quantity > product.StockQuantity)
            throw new InvalidOperationException("Requested quantity exceeds available stock.");

        item.Quantity = request.Quantity;
        cartItemRepository.Update(item);
        await cartItemRepository.SaveChangesAsync();

        return MapToDto(cart);
    }

    public async Task<CartResponseDto> RemoveItemAsync(Guid productId)
    {
        var cart = await GetOrCreateActiveCartAsync();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item != null)
        {
            cartItemRepository.Remove(item);
            cart.Items.Remove(item);
            await cartItemRepository.SaveChangesAsync();
        }

        return MapToDto(cart);
    }

    public async Task<CartResponseDto> ClearCartAsync()
    {
        var cart = await GetOrCreateActiveCartAsync();
        foreach (var item in cart.Items.ToList())
        {
            cartItemRepository.Remove(item);
        }
        cart.Items.Clear();
        await cartItemRepository.SaveChangesAsync();
        
        return MapToDto(cart);
    }

    private async Task<Cart> GetOrCreateActiveCartAsync()
    {
        var customerId = await customerService.GetCurrentCustomerIdAsync()
            ?? throw new UnauthorizedAccessException("Customer context not found.");

        // Load Cart + Items + Products in ONE query using nested LINQ include
        // The repository will convert this expression to a string path
        var results = await cartRepository.FindAsync(
            c => c.CustomerId == customerId, 
            true, 
            c => c.Items.Select(i => i.Product));
        
        var cart = results.FirstOrDefault();

        if (cart == null)
        {
            cart = new Cart
            {
                CustomerId = customerId
            };
            await cartRepository.AddAsync(cart);
            await cartRepository.SaveChangesAsync();
            
            // Re-fetch to ensure nested items/products are loaded for the new cart
            return await GetOrCreateActiveCartAsync();
        }

        return cart;
    }

    private static CartResponseDto MapToDto(Cart cart)
    {
        var items = cart.Items.Select(i => new CartItemResponseDto(
            i.ProductId,
            i.Product?.Name ?? "Unknown Product",
            i.UnitPrice,
            i.Product?.Price ?? 0,
            i.Quantity,
            i.UnitPrice * i.Quantity,
            i.Product != null && i.UnitPrice != i.Product.Price
        )).ToList();

        return new CartResponseDto(
            cart.Id,
            items,
            items.Sum(i => i.Subtotal),
            items.Sum(i => i.Quantity));
    }
}
