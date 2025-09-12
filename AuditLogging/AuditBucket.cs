namespace AuditLogging;


public sealed class AuditBucket
{
    public List<SqlAudit> SqlList { get; } = new();
    public List<HttpCallAudit> OutList { get; } = new();
}

public static class AuditBucketScope
{
    private static readonly AsyncLocal<AuditBucket?> _bucket = new();

    public static AuditBucket? CurrentBucket => _bucket.Value;

    public static void StartScope() => _bucket.Value = new AuditBucket();
    public static void EndScope()   => _bucket.Value = null;
}
