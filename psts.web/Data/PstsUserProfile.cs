namespace psts.web.Data
{
    public class PstsUserProfile
    {
        public Guid Id { get; set; } = Guid.Empty;          // Users UUID as assigned in AspNetUsers
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;     // UUID of Manager


    }
}