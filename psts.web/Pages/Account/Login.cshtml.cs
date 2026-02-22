using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using psts.web.Services;
using Psts.Web.Data;
using Psts.Web.Pages.appSettings;

namespace Psts.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly ISettingsService _settings;

    public LoginModel(SignInManager<AppUser> signInManager, ILogger<LoginModel> logger, UserManager<AppUser> userManager, ISettingsService settings)
    {
        _signInManager = signInManager;
        _logger = logger;
        _userManager = userManager;
        _settings = settings;
    }

    public class LoginInput
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public bool LoginFailed { get; set; }
    public bool OIDCEnabled { get; set; }
    public string LoginErrorText { get; set; } = string.Empty;

    // List of external authentication providers
    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public async Task OnGetAsync()
    {
        LoginErrorText = TempData["LoginError"] as string;

        // Check system setting for OIDC enablement
        OIDCEnabled = await _settings.GetSetting<bool>(psts.web.Domain.Enums.SystemSettings.MakeOIDCAvailable);
     
        
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

        var result = await _signInManager.PasswordSignInAsync(
            userName,
            password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync( userName );
            if (user == null)
            {
                return NotFound();
            }

            if (user.LoginPassAllowed)      // Account can use login and password
            {
                // Check if user needs to change password
                if (user.ResetPassOnLogin == true)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    return RedirectToPage("/Account/Profile", new { userId = user.Id, token });
                }
                else
                {
                    return RedirectToPage("/Account/Home");
                }
            }
            else    // Account not authorized for login and password, logout and kick back to login.
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login");
            }
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "Account is locked.");
            return Page();
        }

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError("", "Sign-in not allowed.");
            return Page();
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return Page();
        }

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