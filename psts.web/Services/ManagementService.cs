using Microsoft.EntityFrameworkCore;
using psts.web.Domain.Enums;
using psts.web.Dto;
using Psts.Web.Data;

namespace psts.web.Services
{
    public class ManagementService : IManagementService
    {

        private readonly PstsDbContext _db;
        private readonly IShortCodeService _scs;

        public ManagementService(PstsDbContext db, IShortCodeService scs)
        {
            _db = db;
            _scs = scs;
        }

        public async Task<ServiceResult<Guid>> AddNewClient(string _requestorId, RoleTypes _requestorRole, CreateClientDto _newClient)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<Guid>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<Guid>.Fail("Invalid role.");
                }

                if ((_requestorRole != RoleTypes.Manager) || (_requestorRole != RoleTypes.Admin))
                {
                    return ServiceResult<Guid>.Fail("Insufficient privileges.");
                }

                
                // Check short code to make sure its not in use.
                var shortCodeCheck = await _scs.DecodeShortCode(_newClient.ShortCode);

                if (shortCodeCheck.Success)
                {
                    if (shortCodeCheck.Data.Type != ShortCodeType.NotFound)
                    {
                        return ServiceResult<Guid>.Fail("Short code in use.");
                    }
                }
                else
                {
                    return ServiceResult<Guid>.Fail("Short code check failed.");
                }

                // Create new profile
                var newClient = new PstsClientProfile
                {
                    ClientId = new Guid(),
                    EmployeePOCId = _newClient.EmployeePOCId,
                    ClientName = _newClient.ClientName,
                    ClientPOCfName = _newClient.ClientPOCfName,
                    ClientPOClName = _newClient.ClientPOClName,
                    ClientPOCeMail = _newClient.ClientPOCeMail,
                    ClientPOCtPhone = _newClient.ClientPOCtPhone,
                    ShortCode = _newClient.ShortCode
                };

                // Write profile to db
                _db.PstsClientProfiles.Add(newClient);
                await _db.SaveChangesAsync();

                // Return new client Id
                return ServiceResult<Guid>.Ok(newClient.ClientId);
            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateClient(string _requestorId, RoleTypes _requestorRole, UpdateClientDto _newClientData)
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




            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<Guid>> AddNewProject(string _requestorId, RoleTypes _requestorRole, CreateProjectDto _newProject)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<Guid>.Fail("Invalid Requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<Guid>.Fail("Invalid Role.");
                }

                if ((_requestorRole != RoleTypes.Manager) || (_requestorRole != RoleTypes.Admin))
                {
                    return ServiceResult<Guid>.Fail("Insufficient Privileges.");
                }


                // Check short code to make sure its not in use.
                var shortCodeCheck = await _scs.DecodeShortCode(_newProject.ShortCode);

                if (shortCodeCheck.Success)
                {
                    if (shortCodeCheck.Data.Type != ShortCodeType.NotFound)
                    {
                        return ServiceResult<Guid>.Fail("Short code in use.");
                    }
                }
                else
                {
                    return ServiceResult<Guid>.Fail("Short code check failed.");
                }

                // Verify the ClientID exists
                var verifyClient = await _db.PstsClientProfiles.AnyAsync(x => x.ClientId == _newProject.ClientId);
                if (!verifyClient)
                {
                    return ServiceResult<Guid>.Fail("Client Id does not exist.");
                }

                // Create new profile
                var newProject = new PstsProjectDefinition
                {
                    ProjectId = new Guid(),
                    ClientId = _newProject.ClientId,
                    EmployeePOCId = _newProject.EmployeePOCId,
                    ProjectName = _newProject.ProjectName,
                    ProjectDescription = _newProject.ProjectDescription,
                    ShortCode = _newProject.ShortCode
                };

                // Write profile to db
                _db.PstsProjectDefinitions.Add(newProject);
                await _db.SaveChangesAsync();

                // Return new client Id
                return ServiceResult<Guid>.Ok(newProject.ClientId);
            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateProject(string _requestorId, RoleTypes _requestorRole, UpdateProjectDto _newClientData)
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




            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<Guid>> AddNewTask(string _requestorId, RoleTypes _requestorRole, CreateTaskDto _newClient)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<Guid>.Fail("Invalid Requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<Guid>.Fail("Invalid Role.");
                }

                if ((_requestorRole != RoleTypes.Manager) || (_requestorRole != RoleTypes.Admin))
                {
                    return ServiceResult<Guid>.Fail("Insufficient Privileges.");
                }




            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateTask(string _requestorId, RoleTypes _requestorRole, UpdateTaskDto _newClientData)
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



            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> ChangeUserRole(string _requestorId, RoleTypes _requestorRole, string _targetEmployee, RoleTypes _newRole)
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



            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }
    }
}
