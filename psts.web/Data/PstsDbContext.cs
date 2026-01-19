using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Psts.Web.Data;

public class PstsDbContext : IdentityDbContext<AppUser>
{
    public PstsDbContext(DbContextOptions<PstsDbContext> options)
        : base(options)
    {
    }

    // Data tables

}
