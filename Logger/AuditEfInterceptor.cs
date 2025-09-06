// using System.Data.Common;
// using System.Diagnostics;
// using Microsoft.EntityFrameworkCore.Diagnostics;
// using Logger;                      // AuditBucketScope
//
// public sealed class AuditEfInterceptor : DbCommandInterceptor
// {
//     /* ---------- READER ---------- */
//     public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
//         DbCommand command,
//         CommandEventData eventData,
//         InterceptionResult<DbDataReader> result,
//         CancellationToken ct = default)
//     {
//         var sw = Stopwatch.StartNew();
//
//         // register callback that runs AFTER command finishes
//         result = result.WithContinuation(r =>
//         {
//             sw.Stop();
//             AuditBucketScope.CurrentBucket?.SqlList.Add(new SqlAudit(
//                 command.CommandText,
//                 sw.Elapsed.TotalMilliseconds,
//                 DateTimeOffset.UtcNow));
//             return r;   // return reader to EF
//         });
//
//         return new ValueTask<InterceptionResult<DbDataReader>>(result);
//     }
//
//     /* ---------- NON-QUERY ---------- */
//     public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
//         DbCommand command,
//         CommandEventData eventData,
//         InterceptionResult<int> result,
//         CancellationToken ct = default)
//     {
//         var sw = Stopwatch.StartNew();
//
//         result = result.WithContinuation(i =>
//         {
//             sw.Stop();
//             AuditBucketScope.CurrentBucket?.SqlList.Add(new SqlAudit(
//                 command.CommandText,
//                 sw.Elapsed.TotalMilliseconds,
//                 DateTimeOffset.UtcNow));
//             return i;
//         });
//
//         return new ValueTask<InterceptionResult<int>>(result);
//     }
//
//     /* ---------- SCALAR ---------- */
//     public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
//         DbCommand command,
//         CommandEventData eventData,
//         InterceptionResult<object> result,
//         CancellationToken ct = default)
//     {
//         var sw = Stopwatch.StartNew();
//
//         result = result.WithContinuation(s =>
//         {
//             sw.Stop();
//             AuditBucketScope.CurrentBucket?.SqlList.Add(new SqlAudit(
//                 command.CommandText,
//                 sw.Elapsed.TotalMilliseconds,
//                 DateTimeOffset.UtcNow));
//             return s;
//         });
//
//         return new ValueTask<InterceptionResult<object>>(result);
//     }
// }