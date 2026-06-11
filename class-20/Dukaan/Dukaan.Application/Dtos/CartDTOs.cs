namespace Dukaan.Application.Dtos;

/// <summary>
/// Data transfer object for adding an item to the cart.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Quantity">The quantity to add.</param>
public record AddToCartRequestDto(Guid ProductId, int Quantity);

/// <summary>
/// Data transfer object for updating the quantity of an item in the cart.
/// </summary>
/// <param name="Quantity">The new quantity.</param>
public record UpdateQuantityRequestDto(int Quantity);

/// <summary>
/// Data transfer object representing a shopping cart.
/// </summary>
/// <param name="CartId">The unique identifier of the cart.</param>
/// <param name="Items">The collection of items in the cart.</param>
/// <param name="Total">The total value of the cart.</param>
/// <param name="ItemCount">The total number of items in the cart.</param>
public record CartResponseDto(
    Guid CartId,
    List<CartItemResponseDto> Items,
    decimal Total,
    int ItemCount);

/// <summary>
/// Data transfer object representing an item in the shopping cart.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="ProductName">The name of the product.</param>
/// <param name="UnitPrice">The price at which the item was added (snapshot).</param>
/// <param name="CurrentPrice">The current price of the product.</param>
/// <param name="Quantity">The quantity in the cart.</param>
/// <param name="Subtotal">The subtotal for this item (UnitPrice * Quantity).</param>
/// <param name="PriceChanged">Indicates if the current price differs from the snapshotted unit price.</param>
public record CartItemResponseDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    decimal CurrentPrice,
    int Quantity,
    decimal Subtotal,
    bool PriceChanged);
