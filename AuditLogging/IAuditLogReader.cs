using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuditLogging;

public interface IAuditLogReader
{
    Task<IEnumerable<HttpAudit>> GetByCorrelationIdAsync(string correlationId);
    Task<IEnumerable<object>> GetAllGroupedByCorrelationIdAsync();
}
