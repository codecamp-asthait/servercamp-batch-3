using Hangfire;
using learning_testing.Services;

namespace learning_testing.BackgroundServices;

/// <summary>
/// Hangfire recurring job that archives overdue todo items.
///
/// This is a plain class (not a BackgroundService) — Hangfire resolves it
/// from the DI container automatically when the recurring job triggers.
/// The [AutomaticRetry] attribute ensures transient failures are retried.
///
/// Registered in Program.cs via:
///   RecurringJob.AddOrUpdate&lt;OverDueTodoArchieveJob&gt;(
///       "archive-overdue-todos",
///       job =&gt; job.ArchiveOverdueTodos(),
///       Cron.Minutely);
///
/// For development/testing, the schedule is set to every minute.
/// In production, change to Cron.Daily() or Cron.Hourly() as needed.
/// </summary>
public class OverDueTodoArchieveJob
{
    private readonly ITodoService _todoService;
    private readonly ILogger<OverDueTodoArchieveJob> _logger;

    /// <summary>
    /// Constructor injection. Hangfire resolves dependencies from the
    /// application's DI container (see Program.cs for registrations).
    /// </summary>
    public OverDueTodoArchieveJob(
        ITodoService todoService,
        ILogger<OverDueTodoArchieveJob> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the overdue-todo archiving logic.
    /// Hangfire will retry up to 3 times on failure (see [AutomaticRetry]).
    /// Logs start, completion, and any errors for observability.
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task ArchiveOverdueTodos()
    {
        _logger.LogInformation("Starting overdue todo archiving job at {Time}", DateTimeOffset.Now);

        try
        {
            var archivedCount = await _todoService.ArchiveOverdueTodos();
            _logger.LogInformation(
                "Completed overdue todo archiving job at {Time}. Archived {Count} todos.",
                DateTimeOffset.Now,
                archivedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while archiving overdue todos at {Time}", DateTimeOffset.Now);
        }
    }
}
