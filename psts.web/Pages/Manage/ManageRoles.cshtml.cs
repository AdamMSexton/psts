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
        [BindProperty] public string? SelectedPendingUserId { get; set; }
        [BindProperty] public string? SelectedSearchUserId { get; set; }
        [BindProperty] public string? TargetRole { get; set; } // "Client" or "Employee"



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

                var results = await query
                    .Take(50)
                    .ToListAsync();

                SearchResults = results;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // validate selection: exactly one user selected
            var pendingSelected = !string.IsNullOrWhiteSpace(SelectedPendingUserId);
            var searchSelected = !string.IsNullOrWhiteSpace(SelectedSearchUserId);

            if (pendingSelected == searchSelected) // both true OR both false
            {
                ModelState.AddModelError(string.Empty, "Select exactly one user (Pending OR Search Results).");
                await OnGetAsync(null); // repopulate lists
                return Page();
            }

            if (string.IsNullOrWhiteSpace(TargetRole))
            {
                ModelState.AddModelError(string.Empty, "Select a target role.");
                await OnGetAsync(null);
                return Page();
            }

            var selectedUserId = pendingSelected ? SelectedPendingUserId! : SelectedSearchUserId!;

            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);


            var result = await _management.ChangeUserRole(User.FindFirstValue(ClaimTypes.NameIdentifier), Enum.Parse<RoleTypes>(roles[0]), selectedUserId, Enum.Parse<RoleTypes>(TargetRole));
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, "Unable to change role. " + result.Error);
                await OnGetAsync(null);
                return Page();
            }


            return RedirectToPage(); // refresh page after success
        }
    }
}
