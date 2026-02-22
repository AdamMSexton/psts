using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace psts.web.Pages.Manage
{
    public class IndexModel : PageModel
    {
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

        public void OnGet()
        {
        }

        public IActionResult OnGetCreate()
        {
            Message = $"Shortcode '{Shortcode}' created (not yet implemented).";
            return Page();
        }

        public IActionResult OnPost()
        {
            Message = $"'{RecordType}' submitted/updated (not yet implemented).";
            return Page();
        }
    }
}