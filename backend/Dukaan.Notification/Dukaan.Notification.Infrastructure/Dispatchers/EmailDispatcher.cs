using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Models;
using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Infrastructure.Dispatchers;

public class EmailDispatcher(
    IServiceScopeFactory scopeFactory,
    IEmailService emailService,
    ILogger<EmailDispatcher> logger) : INotificationDispatcher
{
    public string ChannelType => "email";

    private static readonly Dictionary<string, (string Subject, string BodyTemplate)> EmailTemplates = new()
    {
        ["order-placed"] = ("Order #{0} Placed", "Your order #{0} has been placed successfully."),
        ["order-confirmed"] = ("Order #{0} Confirmed", "Your order #{0} has been confirmed and is being prepared."),
        ["order-shipped"] = ("Order #{0} Shipped", "Your order #{0} has been shipped and is on its way."),
        ["order-delivered"] = ("Order #{0} Delivered", "Your order #{0} has been delivered."),
        ["order-cancelled"] = ("Order #{0} Cancelled", "Your order #{0} has been cancelled."),
    };

    public async Task DispatchAsync(NotificationEventData eventData, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(eventData.CustomerEmail))
        {
            logger.LogWarning("Skipping email for Customer={CustomerId}: customer email is empty", eventData.CustomerId);
            return;
        }

        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = eventData.CustomerId,
            TenantId = eventData.TenantId,
            EventType = eventData.EventType,
            OrderId = eventData.OrderId,
            ChannelType = NotificationChannelType.Email,
            Title = string.Empty,
            Message = string.Empty,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<NotificationEntity>>();
        await repository.AddAsync(notification, ct);
        await repository.SaveChangesAsync(ct);

        var orderDisplayId = eventData.OrderDisplayId ?? eventData.OrderId?.ToString("N")[..8] ?? "N/A";

        string subject, bodyText;
        if (EmailTemplates.TryGetValue(eventData.EventType, out var template))
        {
            subject = string.Format(template.Subject, orderDisplayId);
            bodyText = string.Format(template.BodyTemplate, orderDisplayId);
        }
        else
        {
            subject = $"Order {eventData.EventType}";
            bodyText = $"Your order has been updated (event: {eventData.EventType}).";
        }

        var htmlBody = BuildHtmlEmail(subject, bodyText);
        await emailService.SendEmailAsync(eventData.CustomerEmail, subject, htmlBody, ct);

        logger.LogInformation("Email sent to {Email} for NotificationId={NotificationId}, EventType={EventType}",
            eventData.CustomerEmail, notification.Id, eventData.EventType);
    }

    private static string BuildHtmlEmail(string subject, string bodyText)
    {
        return $"""
            <html>
            <body style="font-family: Arial, sans-serif; background-color: #f9f9f9; margin: 0; padding: 20px;">
              <div style="max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">
                <h2 style="color: #333333; margin-top: 0;">{subject}</h2>
                <p style="color: #555555; line-height: 1.6;">{bodyText}</p>
                <hr style="border: none; border-top: 1px solid #eeeeee; margin: 30px 0;" />
                <p style="color: #999999; font-size: 12px; text-align: center;">Thank you for shopping with us.</p>
              </div>
            </body>
            </html>
            """;
    }
}
