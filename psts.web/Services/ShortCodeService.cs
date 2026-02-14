using Microsoft.EntityFrameworkCore;
using psts.web.Domain.Enums;
using psts.web.Dto;
using Psts.Web.Data;


namespace psts.web.Services
{
    public class ShortCodeService : IShortCodeService
    {
        private readonly PstsDbContext _db;

        public ShortCodeService(PstsDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResult<ShortCodeDecodeResultDto>> DecodeShortCode(string _shortCode)
        {
            try     
            {
                if (string.IsNullOrEmpty(_shortCode))
                {
                    // exit immediately
                    return ServiceResult<ShortCodeDecodeResultDto>.Fail("Short code cannot be empty");
                }

                // Go into DB to find shortcode.  Ordered task & up as most likely task would be the code used as its most specific.
                // Check Task
                var taskShortCode = await _db.PstsTaskDefinitions.SingleOrDefaultAsync(te => te.ShortCode == _shortCode);
                if (taskShortCode != null)
                {
                    return ServiceResult<ShortCodeDecodeResultDto>.Ok(new ShortCodeDecodeResultDto
                    {
                        Type = ShortCodeType.Task,
                        Id = taskShortCode.TaskId
                    });
                }

                // Check Project
                var projectShortCode = await _db.PstsProjectDefinitions.SingleOrDefaultAsync(te => te.ShortCode == _shortCode);
                if (projectShortCode != null)
                {
                    return ServiceResult<ShortCodeDecodeResultDto>.Ok(new ShortCodeDecodeResultDto
                    {
                        Type = ShortCodeType.Project,
                        Id = projectShortCode.ProjectId
                    });
                }

                // Check Client
                var clientShortCode = await _db.PstsClientProfiles.SingleOrDefaultAsync(te => te.ShortCode == _shortCode);
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
    
        public async Task<ServiceResult<bool>> ChangeShortCode(string _requestorId, RoleTypes _requestorRole, ShortCodeType _type, Guid _entityId, string? _newShortCode)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<bool>.Fail("Invalid Requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<bool>.Fail("Invalid Role.");
                }

                if ((_requestorRole != RoleTypes.Manager) || (_requestorRole != RoleTypes.Admin))
                {
                    return ServiceResult<bool>.Fail("Insufficient Privileges.");
                }


                if ((!Enum.IsDefined(typeof(ShortCodeType), _type)) || (_type == ShortCodeType.NotFound))
                {
                    return ServiceResult<bool>.Fail("Invalid ShortCode Type.");
                }

                var IdCheck = await _db.PstsUserProfiles.SingleOrDefaultAsync(te => te.EmployeeId == _requestorId);
                if (IdCheck == null)
                {
                    return ServiceResult<bool>.Fail("Invalid Employee ID.");
                }

                // Branch by type then calidate there is a matching Id, then change and write teh shortcode.
                switch (_type)
                {
                    case ShortCodeType.Client:
                        var clientIdCheck = await _db.PstsClientProfiles.SingleOrDefaultAsync(te => te.ClientId == _entityId);
                        if (clientIdCheck == null)
                        {
                            return ServiceResult<bool>.Fail("Client ID not found.");
                        }
                        clientIdCheck.ShortCode = _newShortCode;
                        await _db.SaveChangesAsync();
                        return ServiceResult<bool>.Ok(true);

                    case ShortCodeType.Project:
                        var projectIdCheck = await _db.PstsProjectDefinitions.SingleOrDefaultAsync(te => te.ProjectId == _entityId);
                        if (projectIdCheck == null)
                        {
                            return ServiceResult<bool>.Fail("Project ID not found.");
                        }
                        projectIdCheck.ShortCode = _newShortCode;
                        await _db.SaveChangesAsync();
                        return ServiceResult<bool>.Ok(true);
                    
                    case ShortCodeType.Task:
                        var taskIdCheck = await _db.PstsTaskDefinitions.SingleOrDefaultAsync(te => te.TaskId == _entityId);
                        if (taskIdCheck == null)
                        {
                            return ServiceResult<bool>.Fail("Task ID not found.");
                        }
                        taskIdCheck.ShortCode = _newShortCode;
                        await _db.SaveChangesAsync();
                        return ServiceResult<bool>.Ok(true);

                    default:
                        return ServiceResult<bool>.Fail("Could not match ShortCode Type.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }
    }
}
