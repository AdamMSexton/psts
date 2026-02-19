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
using Microsoft.IdentityModel.Tokens;

namespace psts.web.Pages.Manage
{
    [Authorize(Roles = nameof(RoleTypes.Manager))] // Restrict access to only Manager users
    
    
    
    public class ManageRolesModel : PageModel
    {
        private readonly PstsDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IManagementService _management;

        public string? Query { get; set; }
        public IList<PstsUserProfile>? SearchResults { get; set; } = new List<PstsUserProfile>();
        public IList<PstsUserProfile>? PendingUsersProfiles { get; set; }

        // bind posted form fields
        [BindProperty] public string? SelectedUserId { get; set; }
        [BindProperty] public string? TargetRole { get; set; } // "Client" or "Employee"
        public string? CurrentRole { get; set; }



        public ManageRolesModel(PstsDbContext db, UserManager<AppUser> userManager, IManagementService management)
        {
            _db = db;
            _userManager = userManager;
            _management = management;
        }

        public async Task OnGetAsync(string? q)
        {
            // Get list of pending users
            var pendingUserList = await _userManager.GetUsersInRoleAsync(nameof(RoleTypes.Pending));

            var pendingIds = pendingUserList.Select(u => u.Id).ToList();

            PendingUsersProfiles = await _db.PstsUserProfiles.Where(p => pendingIds.Contains(p.EmployeeId)).ToListAsync();

            if (q != null)
            {
                q = q.ToLower();

                var terms = q.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var query = _db.PstsUserProfiles.AsQueryable();

                foreach (var term in terms)
                {
                    query = query.Where(p =>
                        p.FName.ToLower().StartsWith(term) ||
                        p.LName.ToLower().StartsWith(term));
                }

                var searchRresults = await query
                    .Take(50)
                    .ToListAsync();

                //SearchResults = results;

                var roleRows = await (
                    from ur in _db.UserRoles
                    join r in _db.Roles on ur.RoleId equals r.Id
                    where SearchResults.Contains(ur.UserId)
                    select new { ur.UserId, RoleName = r.Name }
                ).ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // validate a user was selected

            if (string.IsNullOrWhiteSpace(SelectedUserId))
            {
                ModelState.AddModelError(string.Empty, "No user selected.");
                await OnGetAsync(null); // repopulate lists
                return Page();
            }

            if (string.IsNullOrWhiteSpace(TargetRole))
            {
                ModelState.AddModelError(string.Empty, "Select a target role.");
                await OnGetAsync(null);
                return Page();
            }

            var SelectedUser = await _userManager.FindByIdAsync(SelectedUserId);
            if (SelectedUser == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to find selected user.");
                await OnGetAsync(null);
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
                    await OnGetAsync(null);
                    return Page();
                }
            }


            return RedirectToPage(); // refresh page after success
        }
    }
}
