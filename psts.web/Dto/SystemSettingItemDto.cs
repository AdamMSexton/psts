using psts.web.Domain.Enums;

namespace psts.web.Dto
{
    public class SystemSettingItemDto
    {
        public SystemSettings Setting { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Descriptor { get; set; } = string.Empty;
    }
}
