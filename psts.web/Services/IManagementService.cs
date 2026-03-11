using psts.web.Domain.Enums;
using psts.web.Dto;
using Psts.Web.Data;

namespace psts.web.Services
{
    public interface IManagementService
    {
        Task<ServiceResult<Guid>> AddNewClient(string _requestorId, RoleTypes _requestorRole, CreateClientDto _newClient);
        Task<ServiceResult<bool>> UpdateClient(string _requestorId, RoleTypes _requestorRole, UpdateClientDto _newClientData);
        Task<ServiceResult<Guid>> AddNewProject(string _requestorId, RoleTypes _requestorRole, CreateProjectDto _newProject);
        Task<ServiceResult<bool>> UpdateProject(string _requestorId, RoleTypes _requestorRole, UpdateProjectDto _newProjectData);
        Task<ServiceResult<Guid>> AddNewTask(string _requestorId, RoleTypes _requestorRole, CreateTaskDto _newTask);
        Task<ServiceResult<bool>> UpdateTask(string _requestorId, RoleTypes _requestorRole, UpdateTaskDto _newTaskData);
        Task<ServiceResult<bool>> ChangeUserRole(string _requestorId, RoleTypes _requestorRole, string _targetEmployee, RoleTypes _newRole);

        // Get methods for fetching existing records by ID with their children
        Task<ServiceResult<PstsClientProfile>> GetClient(Guid clientId);
        Task<ServiceResult<PstsProjectDefinition>> GetProject(Guid projectId);
        Task<ServiceResult<PstsTaskDefinition>> GetTask(Guid taskId);

        Task<ServiceResult<List<UserListItemDto>>> SearchUsers(string _searchString);
        Task<ServiceResult<UserSettingsListItemDTO>> GetUserSettings(string _requestorId, RoleTypes _requestorRole, string _targetUser);
    }
}