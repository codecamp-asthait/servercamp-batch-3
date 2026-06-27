using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Domain.Entities;
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

    public async Task DispatchAsync(NotificationEntity notification, string? customerEmail, string? rawData, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            logger.LogWarning("Skipping email for NotificationId={NotificationId}: customer email is empty", notification.Id);
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<NotificationEntity>>();

        notification.IsRead = false;
        notification.CreatedAt = DateTime.UtcNow;

        await repository.AddAsync(notification, ct);
        await repository.SaveChangesAsync(ct);

        var orderDisplayId = notification.OrderId?.ToString("N")[..8] ?? "N/A";

        string subject, bodyText;
        if (EmailTemplates.TryGetValue(notification.EventType, out var template))
        {
            subject = string.Format(template.Subject, orderDisplayId);
            bodyText = string.Format(template.BodyTemplate, orderDisplayId);
        }
        else
        {
            subject = $"Order {notification.EventType}";
            bodyText = $"Your order has been updated (event: {notification.EventType}).";
        }

        var htmlBody = BuildHtmlEmail(subject, bodyText);
        await emailService.SendEmailAsync(customerEmail, subject, htmlBody, ct);

        logger.LogInformation("Email sent to {Email} for NotificationId={NotificationId}, EventType={EventType}",
            customerEmail, notification.Id, notification.EventType);
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
