using psts.web.Domain.Enums;

namespace psts.web.Dto
{
    public class TimeTransactionLineItemDto
    {
        public WorkItemType LineType { get; set; }
        public Guid ItemId { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public string ItemDescriptor { get; set; } = string.Empty;      // Client/project/task Name
        public string? RelatedEmployee { get; set; } = string.Empty;
        public decimal? HoursBilled { get; set; }
        public decimal? HourlyRate { get; set; }
    }
}
