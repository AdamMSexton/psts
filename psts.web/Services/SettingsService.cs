using psts.web.Data;
using psts.web.Domain.Enums;
using psts.web.Dto;
using Psts.Web.Data;

namespace psts.web.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly PstsDbContext _db;
        public SettingsService(PstsDbContext db) 
        {
            _db = db;
        }

        public async Task<T> GetSetting<T>(SystemSettings key)
        {
            // Get row from settings table based on provided key
            var row = await _db.AppSettingss.FindAsync(key);
            if (row == null)
            {
                //Setting not found
                throw new InvalidOperationException($"Setting {key} not found.");
            }

            // Take type provided in template to parse out the type value from the stored string.
            object value = typeof(T) switch
            {
                var t when t == typeof(bool) => bool.Parse(row.Value),
                var t when t == typeof(int) => int.Parse(row.Value),
                _ => throw new NotSupportedException($"Type {typeof(T).Name} not supported.")
            };

            // Return the typed value
            return (T) value;
            
        }

    }
}
