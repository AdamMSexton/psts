using Psts.Web.Data;

namespace psts.web.Dto
{
    public class CreateClientDto
    {
        public string? EmployeePOCId { get; set; }           // UUID of Employee Point of Contact, must remain as string type as all ASP.NET userId
        public string ClientName { get; set; }               // Client company name
        public string ClientPOCfName { get; set; }           // Client POC First Name
        public string ClientPOClName { get; set; }           // Client POC Last Name
        public string ClientPOCeMail { get; set; }           // Client POC E-Mail
        public string ClientPOCtPhone { get; set;  }         // Client POC Phone
        public string ShortCode { get; set; }                // Short code, 4 character limit
    }
}
