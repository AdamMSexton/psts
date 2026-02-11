namespace psts.web.Data
{
    public class AppSettings
    {
        public int Id { get; set; } = 1;                                        // Single record for settings.


        // ***** System Security ******
        public bool MakeOIDCAvailable { get; set; }                             // OIDC services available true=Yes     
        public bool OIDCEnabledByDefault { get; set; }                          // OIDC enabled for users by default  true=Yes by default, false=no, admin must grant.  This setting is subject to MakeOIDCAvailable
        public int DisableAccountAfterXDaysStale { get; set; } = 0;             // Account locked after # days specified without login activity.  Any value <= 0 disables this feature



        // ***** Business Rules ******
        public bool ManagerApprovalForAdjustments { get; set; }                 // Enable/Disable Manager approvals for time transaction adjustments.
    }
}
