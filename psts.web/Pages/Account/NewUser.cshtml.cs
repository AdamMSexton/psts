using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Psts.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace Psts.Web.Pages.Account;

public class NewUserModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public NewUserModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? ManagerName { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Build new user from form inputs
        var user = new AppUser
        {
            UserName = Input.FirstName + "." + Input.LastName,
            Email = Input.Email,
            PhoneNumber = Input.PhoneNumber,
            LoginPassAllowed = true,
            OIDCAllowed = false,
            ResetPassOnLogin = true,
            LockoutEnabled = true,
        };
        
        // Create user
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            // Problem creating user
        }

        // New user assigned pending role while awaiting full onboard.        
        await _userManager.AddToRoleAsync(user, "Pending");

        // New user is a user, generate token and forward to user profile page to setup password.
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return RedirectToPage("/Account/Profile", new {userId = user.Id, token });
    }
}
