using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AuditLogging;

public class MongoAuditLogReader : IAuditLogReader
{
    private readonly IMongoCollection<HttpAudit> _httpAudits;

    public MongoAuditLogReader(IMongoDatabase database)
    {
        _httpAudits = database.GetCollection<HttpAudit>("HttpAudits");
    }

    public async Task<IEnumerable<HttpAudit>> GetByCorrelationIdAsync(string correlationId)
    {
        var filter = Builders<HttpAudit>.Filter.Eq(a => a.CorrelationId, correlationId);
        var cursor = await _httpAudits.FindAsync(filter);
        return await cursor.ToListAsync();
    }

    public async Task<IEnumerable<object>> GetAllGroupedByCorrelationIdAsync()
    {
        var pipeline = _httpAudits.Aggregate()
            .Group(
                a => a.CorrelationId,
                g => new { CorrelationId = g.Key, Audits = g.ToList() }
            );
        return await pipeline.ToListAsync();
    }
}
