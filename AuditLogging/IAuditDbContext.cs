namespace AuditLogging;

public interface IAuditDbContext
{
    Task AddAuditAsync(HttpAudit audit);
}
