using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Psts.Web.Data;
using psts.web.Data;
using System.ComponentModel.DataAnnotations;


namespace Psts.Web.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PstsDbContext _db;
        private readonly SignInManager<AppUser> _signInManager;

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

            return Page();
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
                return Page();

            // Verify current user exists
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Verify both new passowrds entered match
            if (PasswordInput.NewPassword != PasswordInput.VerifyPassword)          // Ensure New and Verify passwords match
            {
                ModelState.AddModelError("PasswordInput.VerifyPassword", "Passwords do not match.");
                return Page();
            }

            // If not a new account, verify password is not a rerun 
            if ((!user.ResetPassOnLogin) || (!string.IsNullOrEmpty(user.PasswordHash)))
            {
                // Test if new password is same as old
                var recycledPassword = await _userManager.CheckPasswordAsync(user, PasswordInput.NewPassword);
                if (recycledPassword)
                {
                    ModelState.AddModelError("PasswordInput.VerifyPassword", "Password must be new.");
                    return Page();
                }
            }
            
            bool forceSignout = true;       // Voluntary Password changes can remain logged in, set flag to force signout until otherwise determined
            
            if ((!user.ResetPassOnLogin) && (string.IsNullOrEmpty(token)))      // Voluntary password change
            {
                forceSignout = false;
            }


                // All password tests passed and no clear to be set/changed
                // Validate received token is valid, not expired, and for user.  Only for forced password changes
            if ((user.ResetPassOnLogin) && (!string.IsNullOrEmpty(token)))
            {
                var isValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
                if (!isValid)
                {
                    return Unauthorized();
                }
            }

            

            // If user existed before current session and used valid credentials this branch
            if ((!user.ResetPassOnLogin) || (!string.IsNullOrEmpty(user.PasswordHash)))
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
            else    // Not logged in with valid credentials, most likely new user.
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

            // Password changed, release locks
            var updateResult = await _userManager.SetLockoutEnabledAsync(user, false);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            
            user.ResetPassOnLogin = false;
            updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) 
            { 
                foreach (var error in updateResult.Errors)
                ModelState.AddModelError("", error.Description);
            }

            if (forceSignout)       // Flagged for signout, should be anybody but voluntary changes.
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login");
            }
            else
            { 
                return RedirectToPage("/Account/Profile");
            }
        }

    }
}