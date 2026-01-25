using Microsoft.AspNetCore.Identity;

namespace Psts.Web.Data;

public class AppUser : IdentityUser
{
    // Optional extra fields
    public enum LoginMode { LocalOnly, OidcOnly, LocalAndOidc};         // Login modes authorized
}
