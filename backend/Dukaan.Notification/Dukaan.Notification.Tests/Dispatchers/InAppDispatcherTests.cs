using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Models;
using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Infrastructure.Dispatchers;
using Dukaan.Notification.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Tests;

public class InAppDispatcherTests
{
    private readonly Mock<IRepository<NotificationEntity>> _repository;
    private readonly Mock<IHubContext<NotificationHub>> _hubContext;
    private readonly Mock<IHubClients> _hubClients;
    private readonly Mock<ISingleClientProxy> _clientProxy;
    private readonly InAppDispatcher _sut;

    public InAppDispatcherTests()
    {
        _repository = new Mock<IRepository<NotificationEntity>>();

        _clientProxy = new Mock<ISingleClientProxy>();
        _hubClients = new Mock<IHubClients>();
        _hubClients.Setup(c => c.Group("user-11111111-1111-1111-1111-111111111111"))
            .Returns(_clientProxy.Object);
        _hubContext = new Mock<IHubContext<NotificationHub>>();
        _hubContext.Setup(h => h.Clients).Returns(_hubClients.Object);

        var serviceScope = new Mock<IServiceScope>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(s => s.GetService(typeof(IRepository<NotificationEntity>)))
            .Returns(_repository.Object);
        serviceScope.Setup(s => s.ServiceProvider).Returns(serviceProvider.Object);

        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(f => f.CreateScope()).Returns(serviceScope.Object);

        var logger = Mock.Of<ILogger<InAppDispatcher>>();
        _sut = new InAppDispatcher(scopeFactory.Object, _hubContext.Object, logger);
    }

    [Fact]
    public async Task DispatchAsync_ShouldPersistAndPush()
    {
        var data = new NotificationEventData(
            "order-shipped",
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null, null, null);

        await _sut.DispatchAsync(data, default);

        _repository.Verify(r => r.AddAsync(
            It.Is<NotificationEntity>(n => !string.IsNullOrEmpty(n.Title) && !string.IsNullOrEmpty(n.Message)),
            default), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(default), Times.Once);
        _clientProxy.Verify(
            c => c.SendCoreAsync("Notification", It.IsAny<object[]>(), default),
            Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_ShouldSendFormattedNotificationDto()
    {
        var customerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var orderId = Guid.NewGuid();
        var data = new NotificationEventData(
            "order-shipped",
            customerId,
            Guid.NewGuid(),
            orderId,
            null, null, null);

        await _sut.DispatchAsync(data, default);

        _clientProxy.Verify(c => c.SendCoreAsync("Notification",
            It.Is<object[]>(o => VerifyNotificationDto(o[0], customerId, "order-shipped", orderId)),
            default), Times.Once);
    }

    private static bool VerifyNotificationDto(object dto, Guid customerId, string eventType, Guid? orderId)
    {
        var dict = dto.GetType().GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(dto)?.ToString());
        return dict["eventType"] == eventType
            && dict["orderId"] == orderId.ToString()
            && !string.IsNullOrEmpty(dict["title"])
            && !string.IsNullOrEmpty(dict["message"]);
    }
}
