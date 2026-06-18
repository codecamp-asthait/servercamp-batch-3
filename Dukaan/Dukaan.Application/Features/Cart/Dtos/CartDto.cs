namespace Dukaan.Application.Features.Cart.Dtos;

public record CartDto(
    Guid CartId,
    List<CartItemDto> Items,
    decimal Total,
    int ItemCount);

public record CartItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    decimal CurrentPrice,
    int Quantity,
    decimal Subtotal,
    bool PriceChanged);
