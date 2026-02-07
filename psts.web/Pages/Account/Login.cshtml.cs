using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Psts.Web.Data;

namespace Psts.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;
    private readonly UserManager<AppUser> _userManager;

    public LoginModel(SignInManager<AppUser> signInManager, ILogger<LoginModel> logger, UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _logger = logger;
        _userManager = userManager;
    }

    public class LoginInput
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public bool LoginFailed { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(
        string userName,
        string password)
    {
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
}
