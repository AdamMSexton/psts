using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using psts.web.Domain.Enums;
using psts.web.Dto;
using Psts.Web.Data;

namespace psts.web.Services
{
    public class ManagementService : IManagementService
    {

        private readonly PstsDbContext _db;
        private readonly IShortCodeService _scs;
        private readonly UserManager<AppUser> _userManager;

        public ManagementService(PstsDbContext db, IShortCodeService scs, UserManager<AppUser> userManager)
        {
            _db = db;
            _scs = scs;
            _userManager = userManager;
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

                // Use newCLientData ClientId to find existing record
                var currentRecord = await _db.PstsClientProfiles.FindAsync(_newClientData.ClientId);
                if (currentRecord == null)
                {
                    return ServiceResult<bool>.Fail("ClientId not found.");
                }

                // Check if short code changed, and if it did, make sure new shortcode is not in use.
                if (_newClientData.ShortCode != currentRecord.ShortCode)
                {
                    // Check new short code to make sure its not in use.
                    var shortCodeCheck = await _scs.DecodeShortCode(_newClientData.ShortCode);

                    if (shortCodeCheck.Success)
                    {
                        if (shortCodeCheck.Data.Type != ShortCodeType.NotFound)
                        {
                            return ServiceResult<bool>.Fail("Short code in use.");
                        }
                    }
                    else
                    {
                        return ServiceResult<bool>.Fail("Short code check failed.");
                    }
                }

                // Update current record vaues to new provided ones
                currentRecord.EmployeePOCId = _newClientData.EmployeePOCId;
                currentRecord.ClientName = _newClientData.ClientName;
                currentRecord.ClientPOCfName = _newClientData.ClientPOCfName;
                currentRecord.ClientPOClName = _newClientData.ClientPOClName;
                currentRecord.ClientPOCeMail = _newClientData.ClientPOCeMail;
                currentRecord.ClientPOCtPhone = _newClientData.ClientPOCtPhone;
                currentRecord.ShortCode = _newClientData.ShortCode;

                // Write record back to DB
                await _db.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
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

        public async Task<ServiceResult<bool>> UpdateProject(string _requestorId, RoleTypes _requestorRole, UpdateProjectDto _newProjectData)
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

                // Use newCLientData ClientId to find existing record
                var currentRecord = await _db.PstsProjectDefinitions.FindAsync(_newProjectData.ProjectId);
                if (currentRecord == null)
                {
                    return ServiceResult<bool>.Fail("ProjectId not found.");
                }

                // Check if short code changed, and if it did, make sure new shortcode is not in use.
                if (_newProjectData.ShortCode != currentRecord.ShortCode)
                {
                    // Check new short code to make sure its not in use.
                    var shortCodeCheck = await _scs.DecodeShortCode(_newProjectData.ShortCode);

                    if (shortCodeCheck.Success)
                    {
                        if (shortCodeCheck.Data.Type != ShortCodeType.NotFound)
                        {
                            return ServiceResult<bool>.Fail("Short code in use.");
                        }
                    }
                    else
                    {
                        return ServiceResult<bool>.Fail("Short code check failed.");
                    }
                }

                // Update current record vaues to new provided ones
                currentRecord.ClientId = _newProjectData.ClientId;
                currentRecord.EmployeePOCId = _newProjectData.EmployeePOCId;
                currentRecord.ProjectName = _newProjectData.ProjectName;
                currentRecord.ProjectDescription = _newProjectData.ProjectDescription;
                currentRecord.ShortCode = _newProjectData.ShortCode;

                // Write record back to DB
                await _db.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<Guid>> AddNewTask(string _requestorId, RoleTypes _requestorRole, CreateTaskDto _newTask)
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
                var shortCodeCheck = await _scs.DecodeShortCode(_newTask.ShortCode);

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

                // Verify the ProjectID exists
                var verifyProject = await _db.PstsProjectDefinitions.AnyAsync(x => x.ProjectId == _newTask.ProjectId);
                if (!verifyProject)
                {
                    return ServiceResult<Guid>.Fail("Project Id does not exist.");
                }

                // Create new profile
                var newTask = new PstsTaskDefinition
                {
                    TaskId = new Guid(),
                    ProjectId = _newTask.ProjectId,
                    TaskName = _newTask.TaskName,
                    TaskDescription = _newTask.TaskDescription,
                    ShortCode = _newTask.ShortCode
                };

                // Write profile to db
                _db.PstsTaskDefinitions.Add(newTask);
                await _db.SaveChangesAsync();

                // Return new client Id
                return ServiceResult<Guid>.Ok(newTask.TaskId);


            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateTask(string _requestorId, RoleTypes _requestorRole, UpdateTaskDto _newTaskData)
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

                // Use newCLientData ClientId to find existing record
                var currentRecord = await _db.PstsTaskDefinitions.FindAsync(_newTaskData.ProjectId);
                if (currentRecord == null)
                {
                    return ServiceResult<bool>.Fail("TaskId not found.");
                }

                // Check if short code changed, and if it did, make sure new shortcode is not in use.
                if (_newTaskData.ShortCode != currentRecord.ShortCode)
                {
                    // Check new short code to make sure its not in use.
                    var shortCodeCheck = await _scs.DecodeShortCode(_newTaskData.ShortCode);

                    if (shortCodeCheck.Success)
                    {
                        if (shortCodeCheck.Data.Type != ShortCodeType.NotFound)
                        {
                            return ServiceResult<bool>.Fail("Short code in use.");
                        }
                    }
                    else
                    {
                        return ServiceResult<bool>.Fail("Short code check failed.");
                    }
                }

                // Update current record vaues to new provided ones
                currentRecord.ProjectId = _newTaskData.ProjectId;
                currentRecord.TaskName = _newTaskData.TaskName;
                currentRecord.TaskDescription = _newTaskData.TaskDescription;
                currentRecord.ShortCode = _newTaskData.ShortCode;

                // Write record back to DB
                await _db.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);

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

                // This function is for managers only, Admins will have their own tool in AdminServices
                if (_requestorRole != RoleTypes.Manager)
                {
                    return ServiceResult<bool>.Fail("Insufficient Privileges.");
                }

                // Managers cannot create admins or managers
                if ((_newRole == RoleTypes.Manager) || (_newRole == RoleTypes.Admin))
                {
                    return ServiceResult<bool>.Fail("Only Admin can create Manager or Admin");
                }
                
                // Find target employee
                var targetUser = await _userManager.FindByIdAsync(_targetEmployee);
                if (targetUser == null)
                {
                    return ServiceResult<bool>.Fail("Employee Id not found.");
                }

                // Get current toles
                var roles = await _userManager.GetRolesAsync(targetUser);

                // Remove all roles if any
                if (roles.Any())
                {
                    var roleResult = await _userManager.RemoveFromRolesAsync(targetUser, roles);
                    if (!roleResult.Succeeded)
                    {
                        return ServiceResult<bool>.Fail("Unable to remove existing role(s).");
                    }
                }

                // Set new role as provided
                var result = await _userManager.AddToRoleAsync(targetUser, _newRole.ToString());
                if (!result.Succeeded)
                {
                    return ServiceResult<bool>.Fail("Failed to add role.");
                }


                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }
    }
}
