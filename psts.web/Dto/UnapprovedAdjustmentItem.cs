namespace psts.web.Dto
{
    public class UnapprovedAdjustmentItem
    {
        public Guid OriginalTransaction {get;set;}
        public DateTime OriginalDate {get;set;}
        public Guid AdjustmentTransaction {get;set;}
        public DateTime AdjustmentDate {get; set;}
    }
}
