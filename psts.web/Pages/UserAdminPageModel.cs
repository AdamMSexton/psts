using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using psts.web.Dto;
using psts.web.Services;
using Psts.Web.Data;

namespace psts.web.Pages
{
    [Authorize]
    public class UserAdminPageModel : SearchPageModel
    {
        public readonly UserManager<AppUser> _userManager;
        public UserAdminPageModel(IManagementService management, UserManager<AppUser> userManager) : base(management)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetUserSettingsAsync(string selectedUser)
        {
            // Find target employee
            var targetUser = await _userManager.FindByIdAsync(selectedUser);

            if ((string.IsNullOrWhiteSpace(selectedUser)) || (targetUser == null))
                return new JsonResult(new UserSettingsListItemDTO { });

            var results = await _management.GetUserSettings(LoggedInUser.UserId, (Domain.Enums.RoleTypes)LoggedInUser.Role, selectedUser);
            return new JsonResult(results.Data);
        }
    }
}
