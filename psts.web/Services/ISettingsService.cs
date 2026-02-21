using psts.web.Domain.Enums;

namespace psts.web.Services
{
    public interface ISettingsService
    {
        Task<T> GetSetting<T>(SystemSettings key);
    }
}
