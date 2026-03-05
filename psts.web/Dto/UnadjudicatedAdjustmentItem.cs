namespace psts.web.Dto
{
    public class UnadjudicatedAdjustmentItem
    {
        public Guid OriginalTransaction {get;set;}
        public DateTime OriginalDateEntered {get;set;}
        public Guid AdjustmentTransaction {get;set;}
        public DateTime AdjustmentDateEntered {get; set;}
    }
}
