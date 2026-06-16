namespace Dukaan.Media.Application.Interfaces;

public interface IJobDispatcher
{
    void EnqueueProcessImage(Guid mediaId, Guid tenantId);
}
