namespace Dukaan.Application.Interfaces;

public interface IOrderNumberService
{
    Task<(int SequenceNumber, string OrderNumber)> GetNextOrderNumberAsync(CancellationToken cancellationToken = default);
}
