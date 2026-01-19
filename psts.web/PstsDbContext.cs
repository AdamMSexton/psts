using Microsoft.EntityFrameworkCore;

namespace Psts.Web.Data;

public class PstsDbContext : DbContext
{
    public PstsDbContext(DbContextOptions<PstsDbContext> options)
        : base(options)
    {
    }

    // Temporary minimal table
    public DbSet<DbInfo> DbInfos => Set<DbInfo>();
}
