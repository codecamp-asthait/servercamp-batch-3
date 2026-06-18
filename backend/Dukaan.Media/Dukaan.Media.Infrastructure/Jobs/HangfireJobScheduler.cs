using Hangfire;
using Microsoft.Extensions.Hosting;

namespace Dukaan.Media.Infrastructure.Jobs;

public class HangfireJobScheduler : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<CleanupStagingJob>(
            "cleanup-staging",
            job => job.ExecuteAsync(),
            Cron.Daily);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
