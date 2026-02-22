using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using psts.web.Services;
using Psts.Web.Data;
using Psts.Web.Pages.appSettings;

namespace Psts.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly ISettingsService _settings;

        public LoginModel(
            SignInManager<AppUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<AppUser> userManager,
            ISettingsService settings)
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

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        public async Task OnGetAsync()
        {
            LoginErrorText = TempData["LoginError"] as string;

            OIDCEnabled = await _settings.GetSetting<bool>(
                psts.web.Domain.Enums.SystemSettings.MakeOIDCAvailable);

            ExternalLogins = (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync(string userName, string password)
        {
            // Always reload OIDC + external providers for UI consistency
            OIDCEnabled = await _settings.GetSetting<bool>(
                psts.web.Domain.Enums.SystemSettings.MakeOIDCAvailable);

            ExternalLogins = (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
                .ToList();

            var result = await _signInManager.PasswordSignInAsync(
                userName,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return NotFound();

                if (user.LoginPassAllowed)
                {
                    if (user.ResetPassOnLogin == true)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        TempData["PasswordChange"] = "You must enter a new password to unlock account.";
                        return RedirectToPage("/Account/Profile", new { userId = user.Id, token });
                    }

                    return RedirectToPage("/Account/Home");
                }

                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login");
            }

            if (result.IsLockedOut)
            {
                LoginFailed = true;
                LoginErrorText = "Account is locked.";
                return Page();
            }

            if (result.IsNotAllowed)
            {
                LoginFailed = true;
                LoginErrorText = "Sign-in not allowed.";
                return Page();
            }

            // Generic failure (wrong username/password)
            LoginFailed = true;
            LoginErrorText = "Invalid username or password.";
            return Page();
        }

        public IActionResult OnPostExternalLogin(string provider)
        {
            var redirectUrl = Url.Page(
                "./ExternalLoginCallback",
                pageHandler: null,
                values: null,
                protocol: Request.Scheme);

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
    }
}
