using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Psts.Web.Data;

namespace Psts.Web.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public LogoutModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }

        // Optional: prevent accidental GET requests from showing a blank page
        public IActionResult OnGet()
        {
            return RedirectToPage("/Account/Login");
        }
    }
}