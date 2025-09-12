namespace AuditLogging;

public class HttpAudit
{
    public long Id { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public string TraceId { get; set; } = default!;
    public string CorrelationId { get; set; } = default!;
    public string? UserId { get; set; }
    public string Method { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string? RequestBody { get; set; }
    public int? StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public double DurationMs { get; set; }
    public string? ClientIp { get; set; }
    public string? Exception { get; set; }
    public string? RequestHeaders { get; set; }
    public string? ResponseHeaders { get; set; }

    // NEW: collections stored as JSON
    public string? SqlCommands { get; set; }   // List<SqlAudit>
    public string? OutgoingCalls { get; set; } // List<HttpCallAudit>
}

// Sub-POCOs
public record SqlAudit(string CommandText, double DurationMs, DateTimeOffset TimeStamp);
public record HttpCallAudit(string Method, string Url, int? StatusCode,
    double DurationMs, string? RequestBody, string? ResponseBody,
    DateTimeOffset TimeStamp);
