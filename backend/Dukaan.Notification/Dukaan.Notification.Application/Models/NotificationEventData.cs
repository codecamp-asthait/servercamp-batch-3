namespace Dukaan.Notification.Application.Models;

public record NotificationEventData(
    string EventType,
    Guid CustomerId,
    Guid TenantId,
    Guid? OrderId,
    string? OrderDisplayId,
    string? CustomerEmail,
    string? RawData);
