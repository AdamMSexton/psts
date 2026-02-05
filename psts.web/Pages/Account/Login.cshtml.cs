using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Psts.Web.Data;

namespace Psts.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public class LoginInput
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public bool LoginFailed { get; set; }

    // List of external authentication providers
    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public async Task OnGetAsync()
    {
        // Load available external login providers
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    // Handle traditional username/password login
    public async Task<IActionResult> OnPostAsync(
        string userName,
        string password)
    {
        // Reload external logins for display if login fails
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        _logger.LogCritical(
            "POST user={User} passLen={Len}",
            userName,
            password?.Length ?? -1
        );

        var result = await _signInManager.PasswordSignInAsync(
            userName,
            password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
            return RedirectToPage("/Account/Home");

        LoginFailed = true;
        return Page();
    }

    // Handle external login (Google, Microsoft, Auth0, Okta)
    public IActionResult OnPostExternalLogin(string provider)
    {
        // Request a redirect to the external login provider
        var redirectUrl = Url.Page("./ExternalLoginCallback", pageHandler: null, values: null, protocol: Request.Scheme);
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

}