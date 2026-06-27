using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Dukaan.Notification.Infrastructure.Services;

public class SmtpEmailService(
    IOptions<SmtpSettings> smtpOptions,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpSettings _settings = smtpOptions.Value;

    public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, _settings.EnableSsl, ct);

        if (!string.IsNullOrEmpty(_settings.Username))
        {
            await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
        }

        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("Email sent to {To} with subject '{Subject}'", to, subject);
    }
}
