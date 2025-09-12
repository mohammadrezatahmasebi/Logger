using Microsoft.EntityFrameworkCore;
using AuditLogging;

public class DemoDb : DbContext, IAuditDbContext
{
    public DemoDb(DbContextOptions<DemoDb> opt) : base(opt) { }
    public DbSet<HttpAudit> HttpAudits => Set<HttpAudit>();
}