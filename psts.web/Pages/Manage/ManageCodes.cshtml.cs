using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using psts.web.Domain.Enums;
using psts.web.Services;

namespace psts.web.Pages.Manage
{
    [Authorize(Roles = nameof(RoleTypes.Manager))] // Restrict access to only Manager users


    public class ManageCodesModel : PageModel
    {
        private readonly IShortCodeService _scs;
        private readonly IManagementService _management;

        [BindProperty(SupportsGet = true)]
        public string? Shortcode { get; set; }

        [BindProperty]
        public string? RecordType { get; set; }

        [BindProperty]
        public string? ClientName { get; set; }

        [BindProperty]
        public string? ProjectName { get; set; }

        [BindProperty]
        public string? TaskName { get; set; }

        public string? Message { get; set; }


        public ManageCodesModel(IShortCodeService scs, IManagementService management)
        {
            _scs = scs;
            _management = management;
        }


        public void OnGet()
        {
            if (Shortcode != null)
            {
                var decodeShortCode = _scs.DecodeShortCode(Shortcode);
                if (decodeShortCode.Result.Success)
                {
                    if (decodeShortCode.Result.Data.Type == ShortCodeType.Client)
                    {
                        // Code here if type client
                    }
                }
                else
                {
                    //Failed for some reason.  Not because short code does not exist.  if code does not exist is will succeeed with a type of not defined
                }
            }
        }

        public IActionResult OnGetCreate()
        {
            Message = $"Shortcode '{Shortcode}' created (not yet implemented).";
            return Page();
        }

        public IActionResult OnPostAsync()
        {
            Message = $"'{RecordType}' submitted/updated (not yet implemented).";
            return Page();
        }
    }
}