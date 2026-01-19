using Microsoft.AspNetCore.Identity;

namespace Psts.Web.Data;

public class AppUser : IdentityUser
{
    // Optional extra fields
    public string fName { get; set; } = string.Empty;
    public string lName { get; set; } = string.Empty;
}
