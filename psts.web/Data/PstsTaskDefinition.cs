namespace Psts.Web.Data
{
    public class PstsTaskDefinition
    {
        public Guid TaskId { get; set; }
        public PstsProjectDefinition? Project { get; set; }
        public Guid ProjectId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
    }
}