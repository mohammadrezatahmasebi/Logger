using Microsoft.EntityFrameworkCore;

public class DemoDb : DbContext
{
    public DemoDb(DbContextOptions<DemoDb> opt) : base(opt) { }
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<HttpAudit> HttpAudits => Set<HttpAudit>();

}