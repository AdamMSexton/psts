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

        AdminSettingsToolModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public List<SystemSettingItemDto> SettingsList = new();
        public void OnGet()
        {
            foreach (var settingItem in SystemSettings.GetValues<SystemSettings>())
            {
                SettingsList.Add(_settingsService.GetSettingDetail(Enum.Parse<SystemSettings>(settingItem));
            }

        }




    }
}
