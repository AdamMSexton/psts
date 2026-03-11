namespace psts.web.Dto
{
    public class UserSettingsListItemDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool LoginPassAllowed { get; set; }
        public bool OIDCAllowed { get; set; }
        public bool ResetPassOnLogin { get; set; }
        public bool StaleAccountLockoutEnabled { get; set; }
    }
}
