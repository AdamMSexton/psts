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
        public DateTime EnterdTimeStamp { get; set; } = DateTime.UtcNow;
        public DateOnly WorkCompletedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public enum TransactionType { NewEntry,Adjustment };         // Entry type
        public PstsTimeTransactions? RelatedTransaction { get; set; }
        public Guid? RelatedId { get; set; }
        public decimal WorkCompletedHours { get; set; } = 0;
        public string Notes {  get; set; } = string.Empty;






    }
}
