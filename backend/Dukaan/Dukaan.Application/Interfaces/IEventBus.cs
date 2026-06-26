namespace Dukaan.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync(string eventName, IReadOnlyDictionary<string, string> data, CancellationToken ct = default);
}
