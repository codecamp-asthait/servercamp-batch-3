using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Infrastructure.Dispatchers;
using Dukaan.Notification.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Tests;

public class SignalDispatcherTests
{
    private readonly Mock<IHubContext<NotificationHub>> _hubContext;
    private readonly Mock<IHubClients> _hubClients;
    private readonly Mock<ISingleClientProxy> _clientProxy;
    private readonly SignalDispatcher _sut;

    public SignalDispatcherTests()
    {
        _clientProxy = new Mock<ISingleClientProxy>();

        _hubClients = new Mock<IHubClients>();
        _hubClients.Setup(c => c.Group("user-11111111-1111-1111-1111-111111111111"))
            .Returns(_clientProxy.Object);

        _hubContext = new Mock<IHubContext<NotificationHub>>();
        _hubContext.Setup(h => h.Clients).Returns(_hubClients.Object);

        var logger = Mock.Of<ILogger<SignalDispatcher>>();
        _sut = new SignalDispatcher(_hubContext.Object, logger);
    }

    [Fact]
    public async Task DispatchAsync_ShouldSendSignalEventWithRawData()
    {
        var notification = new NotificationEntity
        {
            CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            EventType = "order-shipped"
        };
        var rawData = "{\"status\":\"shipped\"}";

        await _sut.DispatchAsync(notification, null, rawData, default);

        _clientProxy.Verify(
            c => c.SendCoreAsync("Signal", It.Is<object[]>(o => o[0].ToString() == rawData), default),
            Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_ShouldSkipWhenRawDataIsEmpty()
    {
        var notification = new NotificationEntity();

        await _sut.DispatchAsync(notification, null, null, default);
        await _sut.DispatchAsync(notification, null, string.Empty, default);

        _clientProxy.Verify(
            c => c.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
