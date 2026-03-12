using psts.web.Domain.Enums;
using psts.web.Dto;

namespace psts.web.Services
{
    public interface ISettingsService
    {
        Task<T> GetSetting<T>(SystemSettings key);

        Task<ServiceResult<SystemSettingItemDto>> GetSettingDetail(SystemSettings setting);
    }
}
