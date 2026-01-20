using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using psts.web.Data;

namespace Psts.Web.Data;

public class PstsDbContext : IdentityDbContext<AppUser>
{
    public PstsDbContext(DbContextOptions<PstsDbContext> options)
        : base(options)
    {
    }

    // Data tables
    DbSet<PstsUserProfile> PstsUserProfiles { get; set; }
    DbSet<PstsClientProfile> PstsClientProfiles { get; set; }
    DbSet<PstsProjectDefinition> PstsProjectDefinitions {  get; set; }
    DbSet<PstsTaskDefinition> PstsTaskDefinitions { get; set; }
    DbSet<PstsTimeTransactions> PstsTimeTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)       // Ensure TransactionNum is incrementing.
    {
        base.OnModelCreating(builder);

        builder.Entity<PstsTimeTransactions>()
            .Property(t => t.TransactionNum)
            .UseIdentityColumn();
    }

}
