using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class SaveChangesTimingInterceptor : SaveChangesInterceptor
{
    private readonly Stopwatch _stopWatch = new();

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        _stopWatch.Restart();
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        _stopWatch.Stop();
        Console.WriteLine($"[EF SaveChanges] Took: {_stopWatch.ElapsedMilliseconds} ms");
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _stopWatch.Stop();
        Console.WriteLine($"[EF SaveChanges FAILED] Took: {_stopWatch.ElapsedMilliseconds} ms");
        base.SaveChangesFailed(eventData);
    }
}
