using learning_testing.Services;

namespace learning_testing.BackgroundServices;

/// <summary>
/// Alternative BackgroundService-based approach for archiving overdue todos.
///
/// This is a classic IHostedService that polls every minute in a loop.
/// It is currently NOT registered in DI (see Program.cs) because the
/// Hangfire recurring job (OverDueTodoArchieveJob) is used instead.
///
/// Hangfire provides better reliability (retry, dashboard, persistence)
/// over a raw BackgroundService. Keep this file for reference or as a
/// fallback if Hangfire is ever removed.
///
/// To enable: uncomment in Program.cs:
///   builder.Services.AddHostedService&lt;OverdueTodoArchiveService&gt;();
/// </summary>
public class OverdueTodoArchiveService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OverdueTodoArchiveService> _logger;

    /// <summary>
    /// Constructor injection. Uses IServiceScopeFactory instead of direct
    /// dependencies because BackgroundService has a singleton lifetime
    /// while ITodoService and ITodoRepository are scoped (per-request).
    /// A scope must be created manually for each execution.
    /// </summary>
    public OverdueTodoArchiveService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OverdueTodoArchiveService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Background service main loop. Runs until the application stops.
    /// On each tick: creates a DI scope, resolves ITodoService, and
    /// archives overdue todos. On error, logs and continues.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OverdueTodoArchiveService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var todoService = scope.ServiceProvider.GetRequiredService<ITodoService>();

                var archivedCount = await todoService.ArchiveOverdueTodos();
                _logger.LogInformation(
                    "Archived {Count} overdue todos via BackgroundService.",
                    archivedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while archiving overdue todos.");
            }

            // Wait 1 minute before the next check.
            // In production, consider a longer interval (e.g., 1 hour).
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
