using psts.web.Domain.Enums;
using psts.web.Dto;

namespace psts.web.Services
{
    public interface IManagementService
    {
        Task<ServiceResult<Guid>> AddNewClient(string _requestorId, RoleTypes _requestorRole, CreateClientDto _newClient);
        Task<ServiceResult<bool>> UpdateClient(string _requestorId, RoleTypes _requestorRole, UpdateClientDto _newClientData);
        Task<ServiceResult<Guid>> AddNewProject(string _requestorId, RoleTypes _requestorRole, CreateProjectDto _newClient);
        Task<ServiceResult<bool>> UpdateProject(string _requestorId, RoleTypes _requestorRole, UpdateProjectDto _newClientData);
        Task<ServiceResult<Guid>> AddNewTask(string _requestorId, RoleTypes _requestorRole, CreateTaskDto _newClient);
        Task<ServiceResult<bool>> UpdateTask(string _requestorId, RoleTypes _requestorRole, UpdateTaskDto _newClientData);
        Task<ServiceResult<bool>> ChangeUserRole(string _requestorId, RoleTypes _requestorRole, string _targetEmployee, RoleTypes _newRole);
    }
}
