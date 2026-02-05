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

        public ProfileModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, PstsDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
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

                // Validate received token is valid, not expired, and for user
                var isValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
                if (!isValid)
                {
                    return Unauthorized();
                }

                if (!user.ResetPassOnLogin)
                {
                    return Forbid();
                }
            }
            else        // Voluntary visit, normal user usage
            {
                user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

            }

            // We have ASP Identity profile in user
            // Get PSTS user profile
            var userProfile = await _db.FindAsync<PstsUserProfile>(user.Id);
            if (userProfile == null)
            {
                return NotFound();
            }

            Input.UserName = user.UserName;
            Input.FirstName = userProfile.FName;
            Input.LastName = userProfile.LName;
            Input.Email = user.Email;
            Input.PhoneNumber = user.PhoneNumber;

            var managerProfile = await _db.FindAsync<PstsUserProfile>(userProfile.ManagerId);
            if (managerProfile == null)
            {
                Input.ManagerName = string.Empty;
            }
            else
            {
                Input.ManagerName = managerProfile.LName + ", " + managerProfile.FName;
            }


                return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
             if (!ModelState.IsValid)
                return Page();


            return Page();
        }
    }
}