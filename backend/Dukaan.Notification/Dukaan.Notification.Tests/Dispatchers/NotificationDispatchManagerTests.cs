using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Models;
using Dukaan.Notification.Infrastructure.Dispatchers;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Tests;

public class NotificationDispatchManagerTests
{
    private readonly Mock<INotificationDispatcher> _inApp;
    private readonly Mock<INotificationDispatcher> _signal;
    private readonly Mock<INotificationDispatcher> _email;
    private readonly NotificationDispatchManager _sut;

    public NotificationDispatchManagerTests()
    {
        _inApp = new Mock<INotificationDispatcher>();
        _inApp.Setup(d => d.ChannelType).Returns("in-app");

        _signal = new Mock<INotificationDispatcher>();
        _signal.Setup(d => d.ChannelType).Returns("signal");

        _email = new Mock<INotificationDispatcher>();
        _email.Setup(d => d.ChannelType).Returns("email");

        var logger = Mock.Of<ILogger<NotificationDispatchManager>>();
        _sut = new NotificationDispatchManager(
            [_inApp.Object, _signal.Object, _email.Object], logger);
    }

    [Fact]
    public async Task DispatchAsync_ShouldCallMatchingDispatchersOnly()
    {
        var data = new NotificationEventData("order-placed", Guid.NewGuid(), Guid.NewGuid(), null, null, null, null);
        var channels = new[] { "in-app", "signal" };

        await _sut.DispatchAsync(data, channels, default);

        _inApp.Verify(d => d.DispatchAsync(data, default), Times.Once);
        _signal.Verify(d => d.DispatchAsync(data, default), Times.Once);
        _email.Verify(d => d.DispatchAsync(It.IsAny<NotificationEventData>(), default), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_ShouldSkipUnknownChannelTypes()
    {
        var data = new NotificationEventData("order-placed", Guid.NewGuid(), Guid.NewGuid(), null, null, null, null);
        var channels = new[] { "in-app", "unknown-channel" };

        await _sut.DispatchAsync(data, channels, default);

        _inApp.Verify(d => d.DispatchAsync(data, default), Times.Once);
        _signal.Verify(d => d.DispatchAsync(It.IsAny<NotificationEventData>(), default), Times.Never);
        _email.Verify(d => d.DispatchAsync(It.IsAny<NotificationEventData>(), default), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_ShouldNotBlockOtherDispatchersWhenOneFails()
    {
        var data = new NotificationEventData("order-placed", Guid.NewGuid(), Guid.NewGuid(), null, null, null, null);
        var channels = new[] { "in-app", "signal" };

        _inApp.Setup(d => d.DispatchAsync(It.IsAny<NotificationEventData>(), default))
            .ThrowsAsync(new InvalidOperationException("failure"));

        await _sut.DispatchAsync(data, channels, default);

        _signal.Verify(d => d.DispatchAsync(data, default), Times.Once);
    }
}
