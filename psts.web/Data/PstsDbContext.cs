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
    public DbSet<AppSettings> AppSettingss { get; set; } = default!;
    public DbSet<PstsUserProfile> PstsUserProfiles { get; set; } = default!;
    public DbSet<PstsClientProfile> PstsClientProfiles { get; set; } = default!;
    public DbSet<PstsProjectDefinition> PstsProjectDefinitions {  get; set; } = default!;
    public DbSet<PstsTaskDefinition> PstsTaskDefinitions { get; set; } = default!;
    public DbSet<PstsTimeTransactions> PstsTimeTransactionss { get; set; } = default!;
    public DbSet<PstsBillingRateResolutionSchedule> pstsBillingRateResolutionSchedules { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)       
    {
        base.OnModelCreating(builder);


        // **** PstsUserProfile ***
        builder.Entity<PstsUserProfile>()                   // Primary Key
            .HasKey(a => a.EmployeeId);

        builder.Entity<PstsUserProfile>()                   // Foreign Key
            .HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<PstsUserProfile>(a => a.EmployeeId);

        builder.Entity<PstsUserProfile>()                   // Foreign Key
            .HasOne(a => a.Manager)
            .WithMany()
            .HasForeignKey(a => a.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);


        // **** PstsClientProfile ***
        builder.Entity<PstsClientProfile>()                 // Primary Key
            .HasKey(b => b.ClientId);

        builder.Entity<PstsClientProfile>()                 // Foreign Key
            .HasOne(b => b.EmployeePOC)
            .WithOne()
            .HasForeignKey<PstsClientProfile>(b => b.EmployeePOCId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsClientProfile>()                 // Define short code as 4 characters
            .Property(b => b.ShortCode)
            .HasMaxLength(4)
            .IsRequired();

        builder.Entity<PstsClientProfile>()                 // Short code must be unique
            .HasIndex(b => b.ShortCode)
            .IsUnique();

        builder.Entity<PstsClientProfile>()                 // Short code must be 4 characters and UPPERCASE
            .ToTable(b => b.HasCheckConstraint(
            "CK_Project_ShortCode_Format",
            "char_length(\"ShortCode\") = 4 AND \"ShortCode\" = upper(\"ShortCode\")"
            ));



        // **** PstsProjectDefinition ***
        builder.Entity<PstsProjectDefinition>()             // Primary Key
            .HasKey(c => c.ProjectId);

        builder.Entity<PstsProjectDefinition>()             // Foreign Key
            .HasOne(c => c.Client)
            .WithOne()
            .HasForeignKey<PstsProjectDefinition>(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsProjectDefinition>()             // Foreign Key
            .HasOne(c => c.EmployeePOC)
            .WithOne()
            .HasForeignKey<PstsProjectDefinition>(c => c.EmployeePOCId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsProjectDefinition>()             // Define short code as 4 characters
            .Property(c => c.ShortCode)
            .HasMaxLength(4)
            .IsRequired();

        builder.Entity<PstsProjectDefinition>()             // Short code must be unique
            .HasIndex(c => c.ShortCode)
            .IsUnique();

        builder.Entity<PstsProjectDefinition>()            // Short code must be 4 characters and UPPERCASE
            .ToTable(c => c.HasCheckConstraint(
            "CK_Project_ShortCode_Format",
            "char_length(\"ShortCode\") = 4 AND \"ShortCode\" = upper(\"ShortCode\")"
            ));



        // **** PstsTaskDefinition ***
        builder.Entity<PstsTaskDefinition>()                // Primary Key
            .HasKey(d => d.TaskId);

        builder.Entity<PstsTaskDefinition>()                // Foreign Key
            .HasOne(d => d.Project)
            .WithOne()
            .HasForeignKey<PstsTaskDefinition>(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsTaskDefinition>()                // Define short code as 4 characters
            .Property(d => d.ShortCode)
            .HasMaxLength(4)
            .IsRequired();

        builder.Entity<PstsTaskDefinition>()                // Short code must be unique
            .HasIndex(d => d.ShortCode)
            .IsUnique();

        builder.Entity<PstsTaskDefinition>()                // Short code must be 4 characters and UPPERCASE
            .ToTable(d => d.HasCheckConstraint(
            "CK_Project_ShortCode_Format",
            "char_length(\"ShortCode\") = 4 AND \"ShortCode\" = upper(\"ShortCode\")"
            ));


        // **** PstsTimeTransactions ***
        builder.Entity<PstsTimeTransactions>()              // Primary Key
            .HasKey(e => e.TransactionId);

        builder.Entity<PstsTimeTransactions>()              // Ensure TransactionNum is incrementing.
            .Property(e => e.TransactionNum)
            .UseIdentityColumn();

        builder.Entity<PstsTimeTransactions>()              // Foreign Key
            .HasOne(e => e.Task)
            .WithOne()
            .HasForeignKey<PstsTimeTransactions>(e => e.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsTimeTransactions>()              // Foreign Key
            .HasOne(e => e.EnteredEmployee)
            .WithOne()
            .HasForeignKey<PstsTimeTransactions>(e => e.EnteredBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsTimeTransactions>()              // Foreign Key
            .HasOne(e => e.WorkCompletedEmployee)
            .WithOne()
            .HasForeignKey<PstsTimeTransactions>(e => e.WorkCompletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsTimeTransactions>()              // Foreign Key, Self refrencing to TransactionId
            .HasOne(t => t.RelatedTransaction)
            .WithMany()
            .HasForeignKey(t => t.RelatedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsTimeTransactions>()              // Explicitly tell DB its a date and time
            .Property(e => e.EnterdTimeStamp)
            .HasColumnType("timestamptz");

        builder.Entity<PstsTimeTransactions>()              // Explicity tell DB its date only
            .Property(t => t.WorkCompletedDate)
            .HasColumnType("date");

        builder.Entity<PstsTimeTransactions>()              // Set Precision to 5 digits, 2 decimal.  max is 999.99
            .Property(e => e.WorkCompletedHours)
            .HasPrecision(5, 2);

        // **** PstsBillingRateResolutionSchedule ***

        builder.Entity<PstsBillingRateResolutionSchedule>()              // Primary Key
            .HasKey(f => f.BillingRateNum);

        builder.Entity<PstsBillingRateResolutionSchedule>()              // Ensure BillingRateNum is incrementing.
            .Property(f => f.BillingRateNum)
            .UseIdentityColumn();

        builder.Entity<PstsBillingRateResolutionSchedule>()                          // Foreign Key
            .HasOne(f => f.Client)
            .WithOne()
            .HasForeignKey<PstsBillingRateResolutionSchedule>(f => f.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsBillingRateResolutionSchedule>()                          // Foreign Key
            .HasOne(f => f.Project)
            .WithOne()
            .HasForeignKey<PstsBillingRateResolutionSchedule>(f => f.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsBillingRateResolutionSchedule>()                          // Foreign Key
            .HasOne(f => f.Task)
            .WithOne()
            .HasForeignKey<PstsBillingRateResolutionSchedule>(f => f.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsBillingRateResolutionSchedule>()                          // Foreign Key
            .HasOne(f => f.Employee)
            .WithOne()
            .HasForeignKey<PstsBillingRateResolutionSchedule>(f => f.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PstsBillingRateResolutionSchedule>()                          // Foreign Key
            .HasOne(f => f.ChangedByEmployee)
            .WithOne()
            .HasForeignKey<PstsBillingRateResolutionSchedule>(f => f.ChangedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
