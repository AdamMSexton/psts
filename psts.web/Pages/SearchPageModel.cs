using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psts.web.Services;
using psts.web.Dto;

namespace psts.web.Pages
{
    [Authorize]
    public class SearchPageModel : AppPageModel
    {
        public readonly IManagementService _management;


        public SearchPageModel(IManagementService management)
        {
            _management = management;            
        }

        public async Task<IActionResult> OnGetSearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
                return new JsonResult(Array.Empty<UserListItemDto>());

            var results = await _management.SearchUsers(term);
            return new JsonResult(results);
        }
    }
}
