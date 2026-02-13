using psts.web.Domain.Enums;
using psts.web.Dto;

namespace psts.web.Services
{
    public interface IShortCodeService
    {
        Task<ServiceResult<ShortCodeDecodeResultDto>> DecodeShortCode(string _shortCode);
        Task<ServiceResult<bool>> ChangeShortCode(string _requestorId, RoleTypes _requestorRole, ShortCodeType _type, Guid _entityId, string? _newShortCode);
    }
}
