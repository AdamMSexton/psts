using Microsoft.AspNetCore.Identity;

namespace Psts.Web.Data;

public class AppUser : IdentityUser
{
    // Optional extra fields
    public bool LoginPassAllowed { get; set; }              // Flag to allow/disallow login using Login & Password
    public bool OIDCAllowed { get; set; }                   // Flag to allow/disallow login using OIDC
    public bool ResetPassOnLogin { get; set; }              // Flag to require password reset on next login
    public DateTime LastSuccessfulLogin { get; set; }       // Date and time of last successful login
    public bool StaleAccountLockoutEnabled { get; set; }    // Account has been locked due to login inactivity
}
