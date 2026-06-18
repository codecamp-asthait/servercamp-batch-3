using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Infrastructure.Jobs;
using Hangfire;

namespace Dukaan.Media.Infrastructure.Services;

public class HangfireJobDispatcher(IBackgroundJobClient backgroundJobClient) : IJobDispatcher
{
    public void EnqueueProcessImage(Guid mediaId, Guid tenantId) =>
        backgroundJobClient.Enqueue<ProcessImageJob>(job => job.ExecuteAsync(mediaId, tenantId));
}
