using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Psts.Web.Data;
using System.Security.Claims;

namespace Psts.Web.Pages.Account;

public class ExternalLoginCallbackModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ExternalLoginCallbackModel> _logger;

    public ExternalLoginCallbackModel(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        ILogger<ExternalLoginCallbackModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Get the information about the user from the external login provider
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            _logger.LogWarning("Error loading external login information.");
            return RedirectToPage("./Login");
        }

        // Sign in the user with this external login provider if the user already has a login
        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
            return RedirectToPage("/Account/Home");
        }

        //// If the user does not have an account, create one
        //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;

        //if (email != null)
        //{
        //    var user = new AppUser
        //    {
        //        UserName = email,
        //        Email = email
        //    };

        //    var createResult = await _userManager.CreateAsync(user);
        //    if (createResult.Succeeded)
        //    {
        //        createResult = await _userManager.AddLoginAsync(user, info);
        //        if (createResult.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
        //            return RedirectToPage("/Account/Home");
        //        }
        //    }

        //    foreach (var error in createResult.Errors)
        //    {
        //        _logger.LogError("Error creating user: {Error}", error.Description);
        //    }
        //}



        // If we get here, something went wrong
        _logger.LogError("Unable to load external login information or create user.");
        return RedirectToPage("./Login");
    }
}