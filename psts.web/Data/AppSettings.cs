namespace psts.web.Data
{
    public class AppSettings
    {
        public int Id { get; set; } = 1;
        public bool OIDCEnabledByDefault { get; set; }
        public bool ManagerApprovalForAdjustments { get; set; }
    }
}
