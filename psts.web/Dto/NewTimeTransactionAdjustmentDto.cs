namespace psts.web.Dto
{
    public class NewTimeTransactionAdjustmentDto
    {
        public Guid TransactionToAdjust { get; set; }
        public decimal RevisedWorkCompletedHours { get; set; }
        public string AdjustmentNotes { get; set; } = string.Empty;
    }
}
