using Dukaan.Application.Dtos;

namespace Dukaan.Application.Interfaces;

/// <summary>
/// Service interface for managing shopping carts.
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Retrieves the current customer's cart, creating one if it doesn't exist.
    /// </summary>
    /// <returns>A DTO representing the cart.</returns>
    Task<CartResponseDto> GetCartAsync();

    /// <summary>
    /// Adds an item to the current customer's cart.
    /// </summary>
    /// <param name="request">The details of the item to add.</param>
    /// <returns>The updated cart DTO.</returns>
    Task<CartResponseDto> AddItemAsync(AddToCartRequestDto request);

    /// <summary>
    /// Updates the quantity of an item in the current customer's cart.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="request">The new quantity.</param>
    /// <returns>The updated cart DTO.</returns>
    Task<CartResponseDto> UpdateQuantityAsync(Guid productId, UpdateQuantityRequestDto request);

    /// <summary>
    /// Removes an item from the current customer's cart.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to remove.</param>
    /// <returns>The updated cart DTO.</returns>
    Task<CartResponseDto> RemoveItemAsync(Guid productId);

    /// <summary>
    /// Clears all items from the current customer's cart.
    /// </summary>
    /// <returns>An empty cart DTO.</returns>
    Task<CartResponseDto> ClearCartAsync();
}
