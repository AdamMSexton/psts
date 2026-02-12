using psts.web.Dto;

namespace psts.web.Services
{
    public interface ITimeEntryService
    {
        Task<ServiceResult<ShortCodeDecodeResultDto>> DecodeShortCode(string shortCode);
        Task<ServiceResult<bool>> ChangeShortCode(string _employeeId, string _employeeRole, string newShortCode);
    }
}
