using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Infrastructure.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Tests;

public class EmailDispatcherTests
{
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<IRepository<NotificationEntity>> _repository;
    private readonly EmailDispatcher _sut;

    public EmailDispatcherTests()
    {
        _emailService = new Mock<IEmailService>();
        _repository = new Mock<IRepository<NotificationEntity>>();

        var serviceScope = new Mock<IServiceScope>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(s => s.GetService(typeof(IRepository<NotificationEntity>)))
            .Returns(_repository.Object);
        serviceScope.Setup(s => s.ServiceProvider).Returns(serviceProvider.Object);

        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(f => f.CreateScope()).Returns(serviceScope.Object);

        var logger = Mock.Of<ILogger<EmailDispatcher>>();
        _sut = new EmailDispatcher(scopeFactory.Object, _emailService.Object, logger);
    }

    [Fact]
    public async Task DispatchAsync_ShouldPersistAndSendEmail()
    {
        var notification = new NotificationEntity
        {
            EventType = "order-placed",
            OrderId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        };
        var customerEmail = "customer@example.com";

        await _sut.DispatchAsync(notification, customerEmail, null, default);

        _repository.Verify(r => r.AddAsync(It.IsAny<NotificationEntity>(), default), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(default), Times.Once);
        _emailService.Verify(e => e.SendEmailAsync(
            customerEmail,
            It.Is<string>(s => s.Contains("Placed")),
            It.Is<string>(html => html.Contains("<html>")),
            default), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_ShouldNotPersistOrSendWhenCustomerEmailIsEmpty()
    {
        var notification = new NotificationEntity { EventType = "order-placed" };

        await _sut.DispatchAsync(notification, null, null, default);
        await _sut.DispatchAsync(notification, string.Empty, null, default);
        await _sut.DispatchAsync(notification, "   ", null, default);

        _repository.Verify(r => r.AddAsync(It.IsAny<NotificationEntity>(), default), Times.Never);
        _repository.Verify(r => r.SaveChangesAsync(default), Times.Never);
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
