using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using psts.web.Data;
using psts.web.Domain.Enums;
using Psts.Web.Data;
using System.Security.Claims;
using Psts.Web.Pages.Account;
using System.Threading.Tasks;
using psts.web.Services;
using psts.web.Dto;
using Microsoft.IdentityModel.Tokens;

namespace psts.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))] // Restrict access to only Admin users
    public class AdminUserToolsModel : SearchPageModel
    {
        private readonly PstsDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public string? Query { get; set; }
        public IList<UserListItemDto>? SearchResults { get; set; } = new List<UserListItemDto>();

        // bind posted form fields
        [BindProperty] public string? SelectedUserId { get; set; }
        [BindProperty] public string? TargetRole { get; set; } // "Client" or "Employee"
        public string? CurrentRole { get; set; }



        public AdminUserToolsModel(PstsDbContext db, UserManager<AppUser> userManager, IManagementService management) : base(management)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // validate a user was selected

            if (string.IsNullOrWhiteSpace(SelectedUserId))
            {
                ModelState.AddModelError(string.Empty, "No user selected.");
                await OnGetAsync(); // repopulate lists
                return Page();
            }

            if (string.IsNullOrWhiteSpace(TargetRole))
            {
                ModelState.AddModelError(string.Empty, "Select a target role.");
                await OnGetAsync();
                return Page();
            }

            var SelectedUser = await _userManager.FindByIdAsync(SelectedUserId);
            if (SelectedUser == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to find selected user.");
                await OnGetAsync();
                return Page();
            }


            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if ((!string.IsNullOrEmpty(user.Id)) && (!roles.IsNullOrEmpty()))
            {
                var result = await _management.ChangeUserRole(user.Id.ToString(), Enum.Parse<RoleTypes>(roles[0]), SelectedUserId, Enum.Parse<RoleTypes>(TargetRole));
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to change role. " + result.Error);
                    await OnGetAsync();
                    return Page();
                }
            }


            return RedirectToPage(); // refresh page after success
        }
    }
}
