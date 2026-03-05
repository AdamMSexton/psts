using psts.web.Data;
using Psts.Web.Data;

namespace psts.web.Dto
{
    public class NewTimeTransactionDto
    {
        public Guid TaskId { get; set; }
        public string EnteredBy { get; set; } = string.Empty;
        public string WorkCompletedBy { get; set; } = string.Empty;
        public DateOnly WorkCompletedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public decimal WorkCompletedHours { get; set; } = 0;
        public string Notes { get; set; } = string.Empty;
    }
}
