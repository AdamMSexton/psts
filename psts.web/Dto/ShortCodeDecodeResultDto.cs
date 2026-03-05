using psts.web.Domain.Enums;

namespace psts.web.Dto
{
    public class ShortCodeDecodeResultDto
    {
        public WorkItemType Type { get; set; }
        public Guid? Id { get; set; }
    }
}
