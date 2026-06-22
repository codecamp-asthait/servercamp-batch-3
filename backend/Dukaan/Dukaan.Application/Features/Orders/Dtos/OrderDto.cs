using Dukaan.Domain.Enums;

namespace Dukaan.Application.Features.Orders.Dtos;

public record OrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    AddressSnapshotDto BillingAddress,
    AddressSnapshotDto DeliveryAddress,
    decimal Subtotal,
    decimal Total,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);

public record AddressSnapshotDto(
    string Street,
    string City,
    string District,
    string PostalCode,
    string Phone
);
