using Microsoft.EntityFrameworkCore;
using psts.web.Domain.Enums;
using psts.web.Dto;
using Psts.Web.Data;

namespace psts.web.Services
{
    public class TimeEntryService : ITimeEntryService
    {
        private readonly PstsDbContext _db;

        public TimeEntryService(PstsDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResult<ShortCodeDecodeResultDto>> DecodeShortCode(string shortCode)
        {
            try     
            {
                if (string.IsNullOrEmpty(shortCode))
                {
                    // exit immediately
                    return ServiceResult<ShortCodeDecodeResultDto>.Fail("Short code cannot be empty");
                }

                // Go into DB to find shortcode.  Ordered task & up as most likely task would be the code used as its most specific.
                // Check Task
                var taskShortCode = await _db.PstsTaskDefinitions.SingleOrDefaultAsync(te => te.ShortCode == shortCode);
                if (taskShortCode != null)
                {
                    return ServiceResult<ShortCodeDecodeResultDto>.Ok(new ShortCodeDecodeResultDto
                    {
                        Type = ShortCodeType.Task,
                        Id = taskShortCode.TaskId
                    });
                }

                // Check Project
                var projectShortCode = await _db.PstsProjectDefinitions.SingleOrDefaultAsync(te => te.ShortCode == shortCode);
                if (projectShortCode != null)
                {
                    return ServiceResult<ShortCodeDecodeResultDto>.Ok(new ShortCodeDecodeResultDto
                    {
                        Type = ShortCodeType.Project,
                        Id = projectShortCode.ProjectId
                    });
                }

                // Check Client
                var clientShortCode = await _db.PstsClientProfiles.SingleOrDefaultAsync(te => te.ShortCode == shortCode);
                if (clientShortCode != null)
                {
                    return ServiceResult<ShortCodeDecodeResultDto>.Ok(new ShortCodeDecodeResultDto
                    {
                        Type = ShortCodeType.Client,
                        Id = clientShortCode.ClientId
                    });
                }

                // Short code not found
                return ServiceResult<ShortCodeDecodeResultDto>.Ok(new ShortCodeDecodeResultDto
                {
                    Type = ShortCodeType.NotFound,
                    Id = null
                });

            }
            catch (Exception ex)
            {
                    return ServiceResult<ShortCodeDecodeResultDto>.Fail(ex.Message);
            }
            
        }
    
        public async Task<ServiceResult<bool>> ChangeShortCode(string _employeeId, string _employeeRole, ShortCodeType type, Guid entityId, string? newShortCode)
        {

        }
    }
}
