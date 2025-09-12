using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AuditLogging;

public class AuditEfInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        LogCommand(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        LogCommand(command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    private static void LogCommand(DbCommand command)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            // This is a synchronous operation, but we are not awaiting it.
            // We are just starting the stopwatch and then stopping it.
            // The actual command is executed by the base class.
        }
        finally
        {
            sw.Stop();
            var audit = new SqlAudit(command.CommandText, sw.Elapsed.TotalMilliseconds, DateTimeOffset.UtcNow);
            AuditBucketScope.CurrentBucket?.SqlList.Add(audit);
        }
    }
}
