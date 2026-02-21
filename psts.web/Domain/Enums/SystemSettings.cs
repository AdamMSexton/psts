namespace psts.web.Domain.Enums
{
    public enum SystemSettings
    {
        // ***** System Security ******

        /// <summary>
        /// Make OIDC services available.  True=Yes
        /// </summary>
        MakeOIDCAvailable,

        /// <summary>
        /// Make OIDC enabled for users by default.  True=Yes by default, False=No, Admin must grant.
        /// <br></br>
        /// This setting does not override to MakeOIDCAvailable
        /// </summary>
        OIDCEnabledByDefault,

        /// <summary>
        /// Account locked after # days specified without login activity.  Any value <= 0 disables this feature.
        /// </summary>
        DisableAccountAfterXDaysStale,



        // ***** Business Rules ******


        /// <summary>
        /// Enable/Disable Manager approvals for time transaction adjustments.
        /// </summary>
        ManagerApprovalForAdjustments,

        /// <summary>
        /// Number of days in the past that entries can be made.
        /// </summary>
        MaxDaysInPastForEntry,

        /// <summary>
        /// Number of days into the future that entries can be made.
        /// </summary>
        MaxDaysInFutureForEntry,

        /// <summary>
        /// Maximum number of hours an employee can log in a day.
        /// </summary>
        MaxHoursByEmployeePerDay

    }
}








