using psts.web.Data;
using psts.web.Domain.Enums;

namespace psts.web.Dto
{
    public class ApprovalDecisionDto
    {
        public Guid SubjectTransactionId { get; set; }
        public ApprovalDecision Decision { get; set; }
        public string? Notes { get; set; }
    }
}
