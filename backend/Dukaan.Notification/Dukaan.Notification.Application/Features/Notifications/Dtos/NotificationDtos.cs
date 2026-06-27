namespace Dukaan.Notification.Application.Features.Notifications.Dtos;

public record NotificationDto(
    Guid Id,
    string EventType,
    Guid? OrderId,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt);

public record NotificationListDto(
    List<NotificationDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record UnreadCountDto(int Count);
