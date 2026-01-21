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

    protected override void OnModelCreating(ModelBuilder builder)       
    {
        base.OnModelCreating(builder);

        builder.Entity<PstsUserProfile>()                   // PstsUserProfile Primary and Foreign Key defininitions
            .HasKey(a => a.EmployeeId);

        builder.Entity<PstsUserProfile>()
            .HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<PstsUserProfile>(a => a.EmployeeId);

        builder.Entity<PstsUserProfile>()
            .HasOne(a => a.Manager)
            .WithMany()
            .HasForeignKey(a => a.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        
        
        builder.Entity<PstsClientProfile>()                 // PstsClientProfile Primary and Foreign Key definitions
            .HasKey(d => d.ClientId);

        builder.Entity<PstsClientProfile>()
            .HasOne(d => d.EmployeePOC)
            .WithOne()
            .HasForeignKey<PstsClientProfile>(p => p.EmployeePOCId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.Entity<PstsProjectDefinition>()             // PstsProjectDefinition Primary and Foreign Key definitions
            .HasKey(b => b.ProjectId);

        builder.Entity<PstsProjectDefinition>()
            .HasOne(d => d.Client)
            .WithOne()
            .HasForeignKey<PstsProjectDefinition>(d => d.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsProjectDefinition>()
            .HasOne(d => d.EmployeePOC)
            .WithOne()
            .HasForeignKey<PstsProjectDefinition>(d => d.EmployeePOCId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.Entity<PstsTaskDefinition>()                // PstsTaskDefinition Primary and Foreign Key definitions
            .HasKey(c => c.TaskId);

        
        
        builder.Entity<PstsTimeTransactions>()              // PstsTimeTransactions Primary and Foreign Key definitions
            .HasKey(e => e.TransactionId);

        builder.Entity<PstsTimeTransactions>()              // Ensure TransactionNum is incrementing.
            .Property(e => e.TransactionNum)
            .UseIdentityColumn();
    }

}
