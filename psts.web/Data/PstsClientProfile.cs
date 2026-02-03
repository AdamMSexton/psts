namespace Psts.Web.Data
{
    public class PstsClientProfile
    {
        public Guid ClientId { get; set; }
        public AppUser? EmployeePOC { get; set; }                           // "Pointer" to AppUser. Fields in this class are related to fields in AppUser
        public string? EmployeePOCId { get; set; }                          // UUID of Employee Point of Contact, must remain as string type as all ASP.NET userId
        public string ClientName { get; set; } = string.Empty;              // Client company name
        public string ClientPOCfName { get; set; } = string.Empty;          // Client POC First Name
        public string ClientPOClName { get; set; } = string.Empty;          // Client POC Last Name
        public string ClientPOCeMail { get; set; } = string.Empty;          // Client POC E-Mail
        public string ClientPOCtPhone { get; set; } = string.Empty;         // Client POC Phone
        public string ShortCode { get; set; } = string.Empty;               // Short code, 4 character limit
    }
}