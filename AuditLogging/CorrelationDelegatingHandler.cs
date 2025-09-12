using Microsoft.AspNetCore.Http;

namespace AuditLogging;

public sealed class CorrelationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _acc;
    public CorrelationDelegatingHandler(IHttpContextAccessor acc) => _acc = acc;
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage req, CancellationToken ct)
    {
        if (_acc.HttpContext?.Items["Correlation-ID"] is string cid)
            req.Headers.TryAddWithoutValidation("X-Correlation-ID", cid);
        return await base.SendAsync(req, ct);
    }
}