using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Models;
using Dukaan.Notification.Infrastructure.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Tests;

public class EmailDispatcherTests
{
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<IRepository<Domain.Entities.NotificationEntity>> _repository;
    private readonly EmailDispatcher _sut;

    public EmailDispatcherTests()
    {
        _emailService = new Mock<IEmailService>();
        _repository = new Mock<IRepository<Domain.Entities.NotificationEntity>>();

        var serviceScope = new Mock<IServiceScope>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(s => s.GetService(typeof(IRepository<Domain.Entities.NotificationEntity>)))
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
        var data = new NotificationEventData(
            "order-placed",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            null,
            "customer@example.com",
            null);

        await _sut.DispatchAsync(data, default);

        _repository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.NotificationEntity>(), default), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(default), Times.Once);
        _emailService.Verify(e => e.SendEmailAsync(
            "customer@example.com",
            It.Is<string>(s => s.Contains("Placed")),
            It.Is<string>(html => html.Contains("<html>")),
            default), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_ShouldSkipWhenCustomerEmailIsEmpty()
    {
        var data = new NotificationEventData(
            "order-placed",
            Guid.NewGuid(),
            Guid.NewGuid(),
            null, null, null, null);

        await _sut.DispatchAsync(data, default);

        var emptyEmail = data with { CustomerEmail = string.Empty };
        await _sut.DispatchAsync(emptyEmail, default);

        var whitespaceEmail = data with { CustomerEmail = "   " };
        await _sut.DispatchAsync(whitespaceEmail, default);

        _repository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.NotificationEntity>(), default), Times.Never);
        _emailService.Verify(
            e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), default),
            Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_ShouldUseFallbackForUnknownEventType()
    {
        var data = new NotificationEventData(
            "order-unknown",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            null,
            "customer@example.com",
            null);

        await _sut.DispatchAsync(data, default);

        _emailService.Verify(e => e.SendEmailAsync(
            "customer@example.com",
            It.Is<string>(s => s.Contains("order-unknown")),
            It.IsAny<string>(),
            default), Times.Once);
    }
}
