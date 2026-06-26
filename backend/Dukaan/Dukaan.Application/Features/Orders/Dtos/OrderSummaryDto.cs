using Dukaan.Domain.Enums;

namespace Dukaan.Application.Features.Orders.Dtos;

public record OrderSummaryDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    decimal Total,
    int ItemCount,
    DateTime CreatedAt);
