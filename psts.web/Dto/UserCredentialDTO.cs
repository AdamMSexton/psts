using psts.web.Domain.Enums;

namespace psts.web.Dto
{
    public sealed class UserCredentialDTO
    {
        public string? UserId { get; set; } = string.Empty;
        public RoleTypes? Role { get; set; }
    }
}
