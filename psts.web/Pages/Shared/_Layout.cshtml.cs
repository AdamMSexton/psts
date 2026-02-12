using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Psts.Web.Data;

namespace psts.web.Pages.Shared
{
    public class _LayoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public _LayoutModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // Expose a small helper property so the layout can show user-specific UI if desired.
        public bool IsSignedIn => User?.Identity?.IsAuthenticated ?? false;
        public string? UserName => User?.Identity?.Name;

        public void OnGet()
        {
        }
          
    }
}

