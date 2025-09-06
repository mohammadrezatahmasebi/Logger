using System.Diagnostics;

public sealed class CorrelationMiddleware : IMiddleware
{
    private readonly ILogger<CorrelationMiddleware> _log;
    public CorrelationMiddleware(ILogger<CorrelationMiddleware> log) => _log = log;
    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        if (!ctx.Request.Headers.TryGetValue("X-Correlation-ID", out var cid))
            cid = Guid.NewGuid().ToString();

        ctx.Items["Correlation-ID"] = cid.ToString();
        ctx.Response.Headers["X-Correlation-ID"] = cid.ToString();

        using (_log.BeginScope(new Dictionary<string, object>
                   { ["Correlation-ID"] = cid.ToString() }))
        {
            using var activity = new ActivitySource("orchestration-api")
                .StartActivity("HTTP " + ctx.Request.Method);
            await next(ctx);
        }
    }
}