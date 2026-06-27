namespace Dukaan.Notification.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct);
}
