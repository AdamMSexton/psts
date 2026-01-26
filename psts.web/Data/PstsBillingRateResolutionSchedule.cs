using Psts.Web.Data;

namespace psts.web.Data
{
    public class PstsBillingRateResolutionSchedule
    {
        public long BillingRateNum { get; set; }
        public PstsClientProfile? Client { get; set; }
        public Guid? ClientId { get; set; }
        public PstsProjectDefinition? Project { get; set; }
        public Guid? ProjectId { get; set; }
        public PstsTaskDefinition? Task { get; set; }
        public Guid? TaskId { get; set; }
        public AppUser? Employee { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime EffectiveAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndAt { get; set; }
        public decimal HourlyRate { get; set; }
        public AppUser? ChangedByEmployee { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;



    }
}
