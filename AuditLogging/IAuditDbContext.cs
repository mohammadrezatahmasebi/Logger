using Microsoft.EntityFrameworkCore;

namespace AuditLogging;

public interface IAuditDbContext
{
    DbSet<HttpAudit> HttpAudits { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
