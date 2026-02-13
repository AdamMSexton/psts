using Microsoft.AspNetCore.Mvc.RazorPages;
using psts.web.Dto;
using psts.web.Services;
using System.Threading.Tasks;

namespace Psts.Web.Pages.Account;

public class HomeModel : PageModel
{
    private readonly IShortCodeService _timeEntryService;

    public ShortCodeDecodeResultDto? testResult { get; private set; }

    public HomeModel (IShortCodeService timeEntryService)
    {
        _timeEntryService = timeEntryService;
    }
    public async Task OnGet()
    {
        var testRun = await _timeEntryService.DecodeShortCode("GOAT");
        if (testRun.Success)
        {
            testResult = testRun.Data;
        }
        
    }
}
