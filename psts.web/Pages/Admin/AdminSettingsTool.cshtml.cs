using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using psts.web.Domain.Enums;
using psts.web.Dto;
using psts.web.Services;

namespace psts.web.Pages.Admin
{
    public class AdminSettingsToolModel : AppPageModel
    {
        public readonly ISettingsService _settingsService;
        public IList<SystemSettingItemDto>? SettingsList { get; set; } = new List<SystemSettingItemDto>();
        
        public AdminSettingsToolModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void OnGet()
        {
            foreach (var settingItem in Enum.GetValues<SystemSettings>())
            {
                var temp = _settingsService.GetSettingDetail(settingItem);
                if (temp.Result.Data != null)
                {
                    SettingsList.Add(temp.Result.Data);
                }
            }
        }




    }
}
