using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Infrastructure.Jobs;
using Hangfire;

namespace Dukaan.Media.Infrastructure.Services;

public class HangfireJobDispatcher(IBackgroundJobClient backgroundJobClient) : IJobDispatcher
{
    public void EnqueueProcessImage(Guid mediaId) =>
        backgroundJobClient.Enqueue<ProcessImageJob>(job => job.ExecuteAsync(mediaId));
}
