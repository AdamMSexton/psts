using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using psts.web.Domain.Enums;
using psts.web.Dto;
using psts.web.Services;
using Psts.Web.Data;

namespace psts.web.Pages.Manage
{
    [Authorize(Roles = nameof(RoleTypes.Manager))] // Restrict access to only Manager users
    public class ManageCodesModel : PageModel
    {
        private readonly IShortCodeService _scs;
        private readonly IManagementService _management;
        private readonly UserManager<AppUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public string? Shortcode { get; set; }

        // Record type — comes from POST create form dropdown, or set from decoded shortcode on GET
        [BindProperty(SupportsGet = true)]
        public string? RecordTypeSearch { get; set; }

        // Hidden fields to carry IDs through to POST for update operations
        [BindProperty]
        public Guid? RecordId { get; set; }
        [BindProperty]
        public Guid? ParentId { get; set; }

        // Parent short code entered by user during create for Project or Task
        [BindProperty]
        public string? ParentShortCode { get; set; }

        // Client fields
        [BindProperty]
        public string? ClientName { get; set; }
        [BindProperty]
        public string? ClientPOCfName { get; set; }
        [BindProperty]
        public string? ClientPOClName { get; set; }
        [BindProperty]
        public string? ClientPOCeMail { get; set; }
        [BindProperty]
        public string? ClientPOCtPhone { get; set; }

        // Project fields
        [BindProperty]
        public string? ProjectName { get; set; }
        [BindProperty]
        public string? ProjectDescription { get; set; }

        // Task fields
        [BindProperty]
        public string? TaskName { get; set; }
        [BindProperty]
        public string? TaskDescription { get; set; }

        // Editable inline list fields — indexed by position for bulk save
        [BindProperty]
        public List<EditableProject> EditableProjects { get; set; } = new();
        [BindProperty]
        public List<EditableTask> EditableTasks { get; set; } = new();

        // Used to tell the view what type was found after a shortcode lookup
        public WorkItemType? FoundType { get; set; }

        public string? Message { get; set; }

        public ManageCodesModel(IShortCodeService scs, IManagementService management, UserManager<AppUser> userManager)
        {
            _scs = scs;
            _management = management;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(Shortcode))
            {
                var decodeShortCode = await _scs.DecodeShortCode(Shortcode);

                if (decodeShortCode.Success)
                {
                    var data = decodeShortCode.Data;

                    if (data.Type == WorkItemType.NotFound)
                    {
                        // Short code does not exist, prompt user to create
                        Message = $"Short code '{Shortcode}' does not exist. Use Create New to add it.";
                        return;
                    }

                    // Valid short code found, set the type and populate fields
                    FoundType = data.Type;
                    RecordTypeSearch = data.Type.ToString();
                    RecordId = data.Id;

                    if (data.Type == WorkItemType.Client && data.Id.HasValue)
                    {
                        // Fetch the full client record by ID, includes all child projects and their tasks
                        var clientResult = await _management.GetClient(data.Id.Value);
                        if (clientResult.Success)
                        {
                            ClientName = clientResult.Data.ClientName;
                            ClientPOCfName = clientResult.Data.ClientPOCfName;
                            ClientPOClName = clientResult.Data.ClientPOClName;
                            ClientPOCeMail = clientResult.Data.ClientPOCeMail;
                            ClientPOCtPhone = clientResult.Data.ClientPOCtPhone;

                            // Populate editable project list with their tasks (read-only in view at client level)
                            EditableProjects = clientResult.Data.Projects.Select(p => new EditableProject
                            {
                                ProjectId = p.ProjectId,
                                ProjectName = p.ProjectName,
                                ProjectDescription = p.ProjectDescription,
                                ShortCode = p.ShortCode,
                                Tasks = p.Tasks.Select(t => new EditableTask
                                {
                                    TaskId = t.TaskId,
                                    TaskName = t.TaskName,
                                    TaskDescription = t.TaskDescription,
                                    ShortCode = t.ShortCode
                                }).ToList()
                            }).ToList();
                        }
                        else
                        {
                            Message = "Short code found but failed to load client record.";
                        }
                    }
                    else if (data.Type == WorkItemType.Project && data.Id.HasValue)
                    {
                        // Fetch the full project record by ID, includes parent client and all child tasks
                        var projectResult = await _management.GetProject(data.Id.Value);
                        if (projectResult.Success)
                        {
                            ParentId = projectResult.Data.ClientId;
                            ProjectName = projectResult.Data.ProjectName;
                            ProjectDescription = projectResult.Data.ProjectDescription;

                            // Populate editable task list (editable when looking up project)
                            EditableTasks = projectResult.Data.Tasks.Select(t => new EditableTask
                            {
                                TaskId = t.TaskId,
                                TaskName = t.TaskName,
                                TaskDescription = t.TaskDescription,
                                ShortCode = t.ShortCode
                            }).ToList();
                        }
                        else
                        {
                            Message = "Short code found but failed to load project record.";
                        }
                    }
                    else if (data.Type == WorkItemType.Task && data.Id.HasValue)
                    {
                        // Fetch the full task record by ID, includes parent project and grandparent client
                        var taskResult = await _management.GetTask(data.Id.Value);
                        if (taskResult.Success)
                        {
                            ParentId = taskResult.Data.ProjectId;
                            TaskName = taskResult.Data.TaskName;
                            TaskDescription = taskResult.Data.TaskDescription;
                        }
                        else
                        {
                            Message = "Short code found but failed to load task record.";
                        }
                    }
                }
                else
                {
                    // Failed for some reason. Not because short code does not exist. If code does not exist it will succeed with a type of NotFound
                    Message = "An error occurred while looking up the short code. Please try again.";
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the current user's ID and role for service calls
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Message = "Unable to identify current user.";
                return Page();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (!Enum.TryParse<RoleTypes>(role, out var roleType))
            {
                Message = "Unable to determine user role.";
                return Page();
            }

            var mode = Request.Form["mode"].ToString().ToLower();
            var recordType = Request.Form["recordType"].ToString();

            // Enforce short code format — must be 4 characters and uppercase
            Shortcode = Shortcode?.Trim().ToUpper();
            if (string.IsNullOrEmpty(Shortcode) || Shortcode.Length != 4)
            {
                Message = "Short code must be exactly 4 characters.";
                return Page();
            }

            if (recordType == "Client")
            {
                if (mode == "create")
                {
                    var dto = new CreateClientDto
                    {
                        ClientName = ClientName ?? string.Empty,
                        ClientPOCfName = ClientPOCfName ?? string.Empty,
                        ClientPOClName = ClientPOClName ?? string.Empty,
                        ClientPOCeMail = ClientPOCeMail ?? string.Empty,
                        ClientPOCtPhone = ClientPOCtPhone ?? string.Empty,
                        ShortCode = Shortcode
                    };
                    var result = await _management.AddNewClient(user.Id, roleType, dto);
                    Message = result.Success ? "Client created successfully." : (result.Error ?? "An error occurred while creating the client.");
                }
                else
                {
                    // Update — requires RecordId
                    if (!RecordId.HasValue)
                    {
                        Message = "Missing client ID for update.";
                        return Page();
                    }
                    var dto = new UpdateClientDto
                    {
                        ClientId = RecordId.Value,
                        ClientName = ClientName,
                        ClientPOCfName = ClientPOCfName,
                        ClientPOClName = ClientPOClName,
                        ClientPOCeMail = ClientPOCeMail,
                        ClientPOCtPhone = ClientPOCtPhone,
                        ShortCode = Shortcode
                    };
                    var result = await _management.UpdateClient(user.Id, roleType, dto);
                    Message = result.Success ? "Client updated successfully." : (result.Error ?? "An error occurred while updating the client.");
                }
            }
            else if (recordType == "Project")
            {
                if (mode == "create")
                {
                    // Resolve parent client short code to get ClientId
                    ParentShortCode = ParentShortCode?.Trim().ToUpper();
                    if (string.IsNullOrEmpty(ParentShortCode))
                    {
                        Message = "Please enter the parent client short code.";
                        return Page();
                    }

                    var parentDecode = await _scs.DecodeShortCode(ParentShortCode);
                    if (!parentDecode.Success || parentDecode.Data.Type != WorkItemType.Client || !parentDecode.Data.Id.HasValue)
                    {
                        Message = "Parent client short code is invalid or not a client.";
                        return Page();
                    }

                    var dto = new CreateProjectDto
                    {
                        ClientId = parentDecode.Data.Id.Value,
                        ProjectName = ProjectName ?? string.Empty,
                        ProjectDescription = ProjectDescription,
                        ShortCode = Shortcode
                    };
                    var result = await _management.AddNewProject(user.Id, roleType, dto);
                    Message = result.Success ? "Project created successfully." : (result.Error ?? "An error occurred while creating the project.");
                }
                else
                {
                    // Update project and all its editable tasks in one submit
                    if (!RecordId.HasValue || !ParentId.HasValue)
                    {
                        Message = "Missing project or client ID for update.";
                        return Page();
                    }

                    var dto = new UpdateProjectDto
                    {
                        ProjectId = RecordId.Value,
                        ClientId = ParentId.Value,
                        ProjectName = ProjectName,
                        ProjectDescription = ProjectDescription,
                        ShortCode = Shortcode
                    };
                    var result = await _management.UpdateProject(user.Id, roleType, dto);
                    if (!result.Success)
                    {
                        Message = result.Error ?? "An error occurred while updating the project.";
                        return Page();
                    }

                    // Save all editable tasks in the inline list
                    foreach (var task in EditableTasks)
                    {
                        task.ShortCode = task.ShortCode?.Trim().ToUpper();
                        var taskDto = new UpdateTaskDto
                        {
                            TaskId = task.TaskId,
                            ProjectId = RecordId.Value,
                            TaskName = task.TaskName,
                            TaskDescription = task.TaskDescription,
                            ShortCode = task.ShortCode
                        };
                        var taskResult = await _management.UpdateTask(user.Id, roleType, taskDto);
                        if (!taskResult.Success)
                        {
                            Message = $"Project updated but failed to update task '{task.TaskName}': {taskResult.Error}";
                            return Page();
                        }
                    }

                    Message = "Project and tasks updated successfully.";
                }
            }
            else if (recordType == "Task")
            {
                if (mode == "create")
                {
                    // Resolve parent project short code to get ProjectId
                    ParentShortCode = ParentShortCode?.Trim().ToUpper();
                    if (string.IsNullOrEmpty(ParentShortCode))
                    {
                        Message = "Please enter the parent project short code.";
                        return Page();
                    }

                    var parentDecode = await _scs.DecodeShortCode(ParentShortCode);
                    if (!parentDecode.Success || parentDecode.Data.Type != WorkItemType.Project || !parentDecode.Data.Id.HasValue)
                    {
                        Message = "Parent project short code is invalid or not a project.";
                        return Page();
                    }

                    var dto = new CreateTaskDto
                    {
                        ProjectId = parentDecode.Data.Id.Value,
                        TaskName = TaskName ?? string.Empty,
                        TaskDescription = TaskDescription,
                        ShortCode = Shortcode
                    };
                    var result = await _management.AddNewTask(user.Id, roleType, dto);
                    Message = result.Success ? "Task created successfully." : (result.Error ?? "An error occurred while creating the task.");
                }
                else
                {
                    // Update — requires RecordId and ParentId
                    if (!RecordId.HasValue || !ParentId.HasValue)
                    {
                        Message = "Missing task or project ID for update.";
                        return Page();
                    }
                    var dto = new UpdateTaskDto
                    {
                        TaskId = RecordId.Value,
                        ProjectId = ParentId.Value,
                        TaskName = TaskName,
                        TaskDescription = TaskDescription,
                        ShortCode = Shortcode
                    };
                    var result = await _management.UpdateTask(user.Id, roleType, dto);
                    Message = result.Success ? "Task updated successfully." : (result.Error ?? "An error occurred while updating the task.");
                }
            }
            else
            {
                Message = "Please select a valid record type.";
            }

            return Page();
        }
    }

    // Helper classes for editable inline lists
    public class EditableProject
    {
        public Guid ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? ShortCode { get; set; }
        public List<EditableTask> Tasks { get; set; } = new();
    }

    public class EditableTask
    {
        public Guid TaskId { get; set; }
        public string? TaskName { get; set; }
        public string? TaskDescription { get; set; }
        public string? ShortCode { get; set; }
    }
}