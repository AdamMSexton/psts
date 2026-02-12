using psts.web.Domain.Enums;

namespace psts.web.Dto
{
    public class ShortCodeDecodeResultDto
    {
        public ShortCodeType Type { get; set; }
        public Guid? Id { get; set; }
    }
}
