namespace Dukaan.Application.Interfaces;

public interface IOrderNumberService
{
    Task<(long SequenceNumber, string OrderNumber)> GetNextOrderNumberAsync(CancellationToken cancellationToken = default);
}
