using Dukaan.Notification.Domain.Enums;
using Dukaan.Notification.Domain.Interfaces;

namespace Dukaan.Notification.Domain.Entities;

public class NotificationEntity : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public NotificationChannelType ChannelType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
