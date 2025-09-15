using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuditLogging;

public sealed class AuditMiddleware : IMiddleware
{
    private readonly ILogger<AuditMiddleware> _log;

    public AuditMiddleware(ILogger<AuditMiddleware> logger)
    {
        _log = logger;
    }

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        if (ctx.Request.Path.StartsWithSegments("/health") ||
            ctx.Request.Path.StartsWithSegments("/swagger"))
        {
            await next(ctx);
            return;
        }
        
        var audit = new HttpAudit
        {
            TimeStamp = DateTimeOffset.UtcNow,
            TraceId = Activity.Current?.RootId ?? ctx.TraceIdentifier,
            CorrelationId = ctx.Items["Correlation-ID"] as string ?? Guid.NewGuid().ToString(),
            UserId = ctx.User?.Identity?.Name,
            Method = ctx.Request.Method,
            Url = ctx.Request.Path + ctx.Request.QueryString,
            ClientIp = ctx.Connection.RemoteIpAddress?.ToString()
        };

        // 1. start async-local bucket
        AuditBucketScope.StartScope();

        // 2. capture request
        ctx.Request.EnableBuffering();
        audit.RequestHeaders = SerializeHeaders(ctx.Request.Headers);
        if (ctx.Request.ContentLength > 0)
        {
            using var r = new StreamReader(ctx.Request.Body, leaveOpen: true);
            audit.RequestBody = await r.ReadToEndAsync();
            ctx.Request.Body.Position = 0;
        }

        // 3. swap response stream
        var originalRespBody = ctx.Response.Body;
        using var respMem = new MemoryStream();
        ctx.Response.Body = respMem;

        var sw = Stopwatch.StartNew();
        Exception? ex = null;
        try { await next(ctx); }
        catch (Exception e) { ex = e; audit.Exception = e.ToString(); throw; }
        finally
        {
            sw.Stop();
            audit.DurationMs = sw.Elapsed.TotalMilliseconds;
            audit.StatusCode = ctx.Response.StatusCode;

            respMem.Position = 0;
            await respMem.CopyToAsync(originalRespBody);
            ctx.Response.Body = originalRespBody;

            using var respReader = new StreamReader(respMem);
            audit.ResponseBody = await respReader.ReadToEndAsync();
            audit.ResponseHeaders = SerializeHeaders(ctx.Response.Headers);

            // 4. fill collections from bucket
            var bucket = AuditBucketScope.CurrentBucket!;
            audit.SqlCommands = JsonSerializer.Serialize(bucket.SqlList);
            audit.OutgoingCalls = JsonSerializer.Serialize(bucket.OutList);

            // 5. end scope
            AuditBucketScope.EndScope();

            // 6. fire-and-forget save (do not block client)
            var sp = ctx.RequestServices;          // capture root provider
            _ = Task.Run(async () =>
            {
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IAuditDbContext>();
                await db.AddAuditAsync(audit);
            });
        }
    }

    private static string SerializeHeaders(IHeaderDictionary h)
        => JsonSerializer.Serialize(h.ToDictionary(k => k.Key, k => string.Join(";", k.Value)));
}