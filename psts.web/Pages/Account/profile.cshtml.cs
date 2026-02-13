using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using psts.web.Data;
using Psts.Web.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Psts.Web.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PstsDbContext _db;
        private readonly SignInManager<AppUser> _signInManager;

        public enum PasswordChangeReason { Undetermined, Voluntary, ForcedOnLogin, NewUser }

        public ProfileModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, PstsDbContext db, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _signInManager = signInManager;
        }

        [BindProperty]
        public ProfileInputModel ProfileInput { get; set; } = new();

        public class ProfileInputModel
        {
            [Required]
            public string UserName { get; set; } = string.Empty;
            [Required]
            public string FirstName { get; set; } = string.Empty;
            [Required]
            public string LastName { get; set; } = string.Empty;
            [Phone]
            public string? PhoneNumber { get; set; }
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;
            public string ManagerName { get; set; } = string.Empty;
        }

        [BindProperty]
        public PasswordInputModel PasswordInput { get; set; } = new();

        public class PasswordInputModel
        {
            public string UserId { get; set; } = string.Empty;
            public string CurrentPassword { get; set; } = string.Empty;
            [Required]
            public string NewPassword { get; set; } = string.Empty;
            [Required]
            public string VerifyPassword { get; set; } = string.Empty;
            public string SecurityTokenStash {  get; set; } = string.Empty;
        }

        public bool RequireCurrentPassword { get; set; } = true;

        //List of available external authentication providers(Google, Microsoft, Auth0, Okta)
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        //List of OIDC providers currently linked to this user's account
        public IList<UserLoginInfo>? CurrentLogins { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            AppUser user;       // Hold on to identity core user data for use after reasoning logic

            if ((!string.IsNullOrEmpty(token)) && (!string.IsNullOrEmpty(userId)))        // Forced visit for password change (usually)
            {
                
                // Validate received User ID
                user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                // Stash token and userId for later use
                PasswordInput.SecurityTokenStash = token;
                PasswordInput.UserId = user.Id;

                if (!user.ResetPassOnLogin)
                {
                    return Forbid();
                }

                // AMS - This needs to be last to make sure reliance for this setting is not based on the provided URL.
                // By this point token has been verified in DB and ResetPassOnLogin flag was checked in Db, therefore we
                // do not need current password as they are either a new user or already authenticated using their expired
                // password and are here to update.
                RequireCurrentPassword = false;     
            }
            else        // Voluntary visit, normal user usage
            {
                user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                // Stash userId for later use
                PasswordInput.UserId = user.Id;
            }

            // We have ASP Identity profile in user
            // Get PSTS user profile
            var userProfile = await _db.FindAsync<PstsUserProfile>(user.Id);
            if (userProfile == null)
            {
                return NotFound();
            }

            ProfileInput.UserName = user.UserName;
            ProfileInput.FirstName = userProfile.FName;
            ProfileInput.LastName = userProfile.LName;
            ProfileInput.Email = user.Email;
            ProfileInput.PhoneNumber = user.PhoneNumber;

            var managerProfile = await _db.FindAsync<PstsUserProfile>(userProfile.ManagerId);
            if (managerProfile == null)
            {
                ProfileInput.ManagerName = string.Empty;
            }
            else
            {
                ProfileInput.ManagerName = managerProfile.LName + ", " + managerProfile.FName;
            }


            //Load available OIDC providers (Google, Microsoft, Auth0, Okta)
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            //Load which OIDC providers are currently linked to this user
            CurrentLogins = await _userManager.GetLoginsAsync(user);

            return Page();
        }



        //Initiates OIDC provider linking process
        public IActionResult OnPostLinkLogin(string provider)
        {
            // Redirect to the external provider (Google, Microsoft, etc.) for authentication
            // After authentication, provider redirects back to OnGetLinkLoginCallbackAsync
            var redirectUrl = Url.Page("./Profile", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        //Handles callback after user authenticates with OIDC provider
        //runs after Google/Microsoft/Auth0/Okta redirects back
        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Get the external login info from the provider 
            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                TempData["StatusMessage"] = "Error: Could not load external login information.";
                return RedirectToPage();
            }

            // Link the external login to the current user's account
            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                // This error typically means the OIDC account is already linked to another user
                TempData["StatusMessage"] = "Error: The external login was not added. The login may already be associated with another account.";
                return RedirectToPage();
            }

            // Clear the external authentication cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            TempData["StatusMessage"] = $"The external login {info.LoginProvider} was added successfully.";
            return RedirectToPage();
        }


        //Removes a linked OIDC provider from user's account
        //runs when user clicks "Remove" next to a linked provider on the profile page
        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Remove the external login from the user's account
            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = "Error: The external login was not removed.";
                return RedirectToPage();
            }

            // Refresh the user's authentication session
            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = $"The external login {loginProvider} was removed successfully.";
            return RedirectToPage();
        }




        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            if (!TryValidateModel(ProfileInput, nameof(ProfileInput)))
                return Page();

            return Page();
        }



        public async Task<IActionResult> OnPostChangePasswordAsync(string userId, string token)
        {
            ModelState.Clear();
            if (!TryValidateModel(PasswordInput, nameof(PasswordInput)))
            {
                return Page();
            }

            // Verify current user exists
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            PasswordChangeReason changeReason = PasswordChangeReason.Undetermined;

            // Is this an existing user executing a voluntary change
            if ((!user.ResetPassOnLogin) && (!string.IsNullOrEmpty(user.PasswordHash)))
            {
                changeReason = PasswordChangeReason.Voluntary;
            }

            // This was existing user that was forced to change password
            if ((user.ResetPassOnLogin) && (!string.IsNullOrEmpty(user.PasswordHash)))
            {
                changeReason = PasswordChangeReason.ForcedOnLogin;
            }

            // This is a new user who is setting their password first time in
            if ((user.ResetPassOnLogin) && (string.IsNullOrEmpty(user.PasswordHash)))
            {
                changeReason = PasswordChangeReason.NewUser;
            }


            // For some reason we can not determine what teh circumstance is, so terminate.
            if (changeReason == PasswordChangeReason.Undetermined)
            {
                return Forbid();
            }


            // Verify both new passowrds entered match
            if (PasswordInput.NewPassword != PasswordInput.VerifyPassword)          // Ensure New and Verify passwords match
            {
                ModelState.AddModelError("PasswordInput.VerifyPassword", "Passwords do not match.");
                return Page();
            }

           
            // If not a new account, verify password is not a rerun 
            if ((changeReason==PasswordChangeReason.Voluntary) || (changeReason == PasswordChangeReason.ForcedOnLogin))
            {
                // Test if new password is same as old
                var recycledPassword = await _userManager.CheckPasswordAsync(user, PasswordInput.NewPassword);
                if (recycledPassword)
                {
                    ModelState.AddModelError("PasswordInput.VerifyPassword", "Password must be new.");
                    return Page();
                }
            }
            
            // Ensure password meets system requirements
            foreach (var validator in _userManager.PasswordValidators)
            {
                var result = await validator.ValidateAsync(_userManager, user, PasswordInput.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(nameof(PasswordInput.NewPassword), error.Description);
                    }
                    return Page();
                }
            }

            // For forced changes, validate received token is valid, not expired, and for user.
            if ((changeReason == PasswordChangeReason.ForcedOnLogin) || (changeReason == PasswordChangeReason.NewUser))
            {
                var isValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
                if (!isValid)
                {
                    return Unauthorized();
                }
            }

            // If voluntary change then use change password
            if (changeReason == PasswordChangeReason.Voluntary)
            {
                var result = await _userManager.ChangePasswordAsync(user, PasswordInput.CurrentPassword, PasswordInput.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return Page();
                }
            }

            // If new user (NULL password) or forced (credentials verified with login and token) then reset password
            if ((changeReason == PasswordChangeReason.NewUser) || (changeReason == PasswordChangeReason.ForcedOnLogin))
            {
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, PasswordInput.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return Page();
                }
            }

            // Password changed, reset failed login count
            var updateResult = await _userManager.ResetAccessFailedCountAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            // Password changed, remove lockout end date
            updateResult = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            // Password changed, clear the reset on login flag
            user.ResetPassOnLogin = false;
            updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) 
            { 
                foreach (var error in updateResult.Errors)
                ModelState.AddModelError("", error.Description);
            }

            // Force signout, should be anybody but voluntary changes.
            if ((changeReason == PasswordChangeReason.ForcedOnLogin) || (changeReason == PasswordChangeReason.NewUser))
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login");
            }
            
            // Voluntary change was authenticate with a good password earlier.  continue without logout.
            if (changeReason == PasswordChangeReason.Voluntary)
            { 
                return RedirectToPage("/Account/Profile");
            }

            return Page();
        }

    }
}