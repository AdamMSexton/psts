using Psts.Web.Data;

namespace psts.web.Dto
{
    public class UpdateTaskDto
    {
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public string? TaskName { get; set; } = string.Empty;
        public string? TaskDescription { get; set; } = string.Empty;
        public string? ShortCode { get; set; } = string.Empty;
    }
}
