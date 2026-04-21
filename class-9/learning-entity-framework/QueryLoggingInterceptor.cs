using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class QueryLoggingInterceptor : DbCommandInterceptor
{
    private readonly Stopwatch _stopwatch = new();

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        _stopwatch.Restart();

        Console.WriteLine("------ EF QUERY START ------");
        Console.WriteLine(command.CommandText);

        return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        _stopwatch.Stop();

        Console.WriteLine($"[EF QUERY END] Took: {_stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine("-----------------------------");

        return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }
}