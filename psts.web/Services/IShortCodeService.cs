using psts.web.Domain.Enums;
using psts.web.Dto;

namespace psts.web.Services
{
    public interface IShortCodeService
    {
        Task<ServiceResult<ShortCodeDecodeResultDto>> DecodeShortCode(string _shortCode);
        Task<ServiceResult<ShortCodeDecodeResultDto>> DecodeShortCode(Guid _EntityId);
        Task<ServiceResult<bool>> ChangeShortCode(string _requestorId, RoleTypes _requestorRole, WorkItemType _type, Guid _entityId, string? _newShortCode);
    }
}
