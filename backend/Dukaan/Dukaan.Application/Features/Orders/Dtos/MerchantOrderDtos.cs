using Dukaan.Domain.Enums;

namespace Dukaan.Application.Features.Orders.Dtos;

public record MerchantOrderSummaryDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    decimal Total,
    int ItemCount,
    string CustomerName,
    DateTime CreatedAt);

public record MerchantOrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    AddressSnapshotDto BillingAddress,
    AddressSnapshotDto DeliveryAddress,
    decimal Subtotal,
    decimal Total,
    DateTime CreatedAt,
    string CustomerName,
    List<OrderItemDto> Items);
