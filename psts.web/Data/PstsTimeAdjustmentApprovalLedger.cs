using psts.web.Domain.Enums;
using Psts.Web.Data;

namespace psts.web.Data
{
    public class PstsTimeAdjustmentApprovalLedger
    {
        public long TransactionNum { get; set; }
        public Guid SubjectTransactionId { get; set; } = Guid.NewGuid();
        public PstsTimeTransactions SubjectTransaction { get; set; } = default!;
        public string ApprovalAuthority { get; set; } = string.Empty;
        public ApprovalDecision Decision { get; set; }
        public string? Notes {  get; set; } = string.Empty;
        public DateTime DecisionTimeStamp { get; set; } = DateTime.UtcNow;
        public AppUser? ApprovingUser { get; set; }
    }
}
