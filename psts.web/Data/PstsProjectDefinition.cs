namespace Psts.Web.Data
{
    internal class PstsProjectDefinition
    {
        public Guid ProjectId { get; set; }
        public PstsClientProfile? Client {  get; set; }           // "Pointer" to PstsClientProfile. Fields in this class are related to fields in PstsClientProfile
        public Guid ClientId { get; set; }
        public AppUser? EmployeePOC { get; set; }                 // "Pointer" to AppUser. Fields in this class are related to fields in AppUser
        public string? EmployeePOCId { get; set; }                // UUID of Employee POC, must remain as string type as all ASP.NET userId
        public string ProjectName { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
    }
}