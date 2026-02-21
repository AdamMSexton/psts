using Psts.Web.Data;
using System.Drawing;

namespace psts.web.Data
{
    public class PstsTimeTransactions
    {
        public long TransactionNum { get; set; }
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public PstsTaskDefinition? Task {  get; set; }
        public Guid TaskId { get; set; }
        public AppUser? EnteredEmployee { get; set; }
        public string EnteredBy { get; set; } = string.Empty;
        public AppUser? WorkCompletedEmployee { get; set; }
        public string WorkCompletedBy { get; set; } = string.Empty;
        public DateTime EnteredTimeStamp { get; set; } = DateTime.UtcNow;
        public DateOnly WorkCompletedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public decimal WorkCompletedHours { get; set; } = 0;
        public string Notes {  get; set; } = string.Empty;
        
        // ***** Adjustment related variables *****
        public bool IsAdjustment { get; set; } = false;                 // Entry is an adjustment to another transaction
        public PstsTimeTransactions? RelatedTransaction { get; set; }
        public PstsTimeAdjustmentApprovalLedger? RelatedApproval { get; set; } // null = unapproved
        public Guid? RelatedId { get; set; }
    }
}
