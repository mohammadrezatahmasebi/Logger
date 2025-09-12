using System.Diagnostics;

namespace AuditLogging;

public sealed class AuditHttpHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage req, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        string? reqBody = null;
        if (req.Content != null)
            reqBody = await req.Content.ReadAsStringAsync(ct);

        HttpResponseMessage? resp;
        string? respBody = null;
        int? status = null;
        try
        {
            resp = await base.SendAsync(req, ct);
            status = (int)resp.StatusCode;
            if (resp.Content != null)
                respBody = await resp.Content.ReadAsStringAsync(ct);
        }
        catch
        {
            status = null; // will be null, exception logged elsewhere
            throw;
        }
        finally
        {
            sw.Stop();
            AuditBucketScope.CurrentBucket?.OutList.Add(new HttpCallAudit(
                req.Method.Method,
                req.RequestUri?.ToString() ?? "",
                status,
                sw.Elapsed.TotalMilliseconds,
                reqBody,
                respBody,
                DateTimeOffset.UtcNow));
        }

        return resp!;
    }
}