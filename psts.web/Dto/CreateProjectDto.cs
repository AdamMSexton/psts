using Psts.Web.Data;

namespace psts.web.Dto
{
    public class CreateProjectDto
    {
        public Guid ClientId { get; set; }
        public string? EmployeePOCId { get; set; }                // UUID of Employee POC, must remain as string type as all ASP.NET userId
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
    }
}
