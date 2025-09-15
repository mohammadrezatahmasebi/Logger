using MongoDB.Driver;

namespace AuditLogging;

public class MongoAuditDbContext : IAuditDbContext
{
    private readonly IMongoCollection<HttpAudit> _httpAudits;

    public MongoAuditDbContext(IMongoDatabase database)
    {
        _httpAudits = database.GetCollection<HttpAudit>("HttpAudits");
    }

    public Task AddAuditAsync(HttpAudit audit)
    {
        return _httpAudits.InsertOneAsync(audit);
    }
}
