using Dukaan.Notification.Application.Core.Abstractions;
using Dukaan.Notification.Application.Features.Notifications.Dtos;
using ErrorOr;

namespace Dukaan.Notification.Application.Features.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery()
    : IQuery<ErrorOr<UnreadCountDto>>;
