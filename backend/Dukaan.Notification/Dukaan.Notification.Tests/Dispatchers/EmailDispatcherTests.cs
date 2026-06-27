using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Infrastructure.Dispatchers;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Tests;

public class EmailDispatcherTests
{
    private readonly Mock<IEmailService> _emailService;
    private readonly EmailDispatcher _sut;

    public EmailDispatcherTests()
    {
        _emailService = new Mock<IEmailService>();
        var logger = Mock.Of<ILogger<EmailDispatcher>>();
        _sut = new EmailDispatcher(_emailService.Object, logger);
    }

    [Fact]
    public async Task DispatchAsync_ShouldSendEmailWithFormattedContent()
    {
        var notification = new NotificationEntity
        {
            EventType = "order-placed",
            OrderId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        };
        var customerEmail = "customer@example.com";

        await _sut.DispatchAsync(notification, customerEmail, null, default);

        _emailService.Verify(e => e.SendEmailAsync(
            customerEmail,
            It.Is<string>(s => s.Contains("Placed")),
            It.Is<string>(html => html.Contains("<html>")),
            default), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_ShouldSkipWhenCustomerEmailIsEmpty()
    {
        var notification = new NotificationEntity { EventType = "order-placed" };

        await _sut.DispatchAsync(notification, null, null, default);
        await _sut.DispatchAsync(notification, string.Empty, null, default);
        await _sut.DispatchAsync(notification, "   ", null, default);

        _emailService.Verify(
            e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), default),
            Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_ShouldUseFallbackForUnknownEventType()
    {
        var notification = new NotificationEntity
        {
            EventType = "order-unknown",
            OrderId = Guid.Parse("33333333-3333-3333-3333-333333333333")
        };
        var customerEmail = "customer@example.com";

        await _sut.DispatchAsync(notification, customerEmail, null, default);

        _emailService.Verify(e => e.SendEmailAsync(
            customerEmail,
            It.Is<string>(s => s.Contains("order-unknown")),
            It.IsAny<string>(),
            default), Times.Once);
    }
}
