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

        // Record type selected in the left panel search dropdown
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

        // Used to tell the view what type was found after a shortcode lookup
        public ShortCodeType? FoundType { get; set; }

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

                    if (data.Type == ShortCodeType.NotFound)
                    {
                        // Short code does not exist, prompt user to create
                        Message = $"Short code '{Shortcode}' does not exist. You can create it using the Create New button.";
                        return;
                    }

                    // Valid short code found, set the type and populate fields
                    FoundType = data.Type;
                    RecordTypeSearch = data.Type.ToString();
                    RecordId = data.Id;

                    if (data.Type == ShortCodeType.Client && data.Id.HasValue)
                    {
                        // Fetch the full client record by ID, includes child project and its child task
                        var clientResult = await _management.GetClient(data.Id.Value);
                        if (clientResult.Success)
                        {
                            ClientName = clientResult.Data.ClientName;
                            ClientPOCfName = clientResult.Data.ClientPOCfName;
                            ClientPOClName = clientResult.Data.ClientPOClName;
                            ClientPOCeMail = clientResult.Data.ClientPOCeMail;
                            ClientPOCtPhone = clientResult.Data.ClientPOCtPhone;

                            //// Populate child project fields if one exists (read-only in view)
                            //if (clientResult.Data.Project != null)
                            //{
                            //    ProjectName = clientResult.Data.Project.ProjectName;
                            //    ProjectDescription = clientResult.Data.Project.ProjectDescription;

                            //    // Populate grandchild task fields if one exists (read-only in view)
                            //    if (clientResult.Data.Project.Task != null)
                            //    {
                            //        TaskName = clientResult.Data.Project.Task.TaskName;
                            //        TaskDescription = clientResult.Data.Project.Task.TaskDescription;
                            //    }
                            //}
                        }
                        else
                        {
                            Message = "Short code found but failed to load client record.";
                        }
                    }
                    else if (data.Type == ShortCodeType.Project && data.Id.HasValue)
                    {
                        // Fetch the full project record by ID, includes parent Client and child task
                        var projectResult = await _management.GetProject(data.Id.Value);
                        if (projectResult.Success)
                        {
                            ParentId = projectResult.Data.ClientId;
                            ProjectName = projectResult.Data.ProjectName;
                            ProjectDescription = projectResult.Data.ProjectDescription;

                            //// Populate child task fields if one exists (read-only in view)
                            //if (projectResult.Data.Tasks != null)
                            //{
                            //    TaskName = projectResult.Data.Tasks.TaskName;
                            //    TaskDescription = projectResult.Data.Tasks.TaskDescription;
                            //}
                        }
                        else
                        {
                            Message = "Short code found but failed to load project record.";
                        }
                    }
                    else if (data.Type == ShortCodeType.Task && data.Id.HasValue)
                    {
                        // Fetch the full task record by ID, includes parent Project and grandparent Client
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
                        ShortCode = Shortcode ?? string.Empty
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
                    if (string.IsNullOrEmpty(ParentShortCode))
                    {
                        Message = "Please enter the parent client short code.";
                        return Page();
                    }

                    var parentDecode = await _scs.DecodeShortCode(ParentShortCode);
                    if (!parentDecode.Success || parentDecode.Data.Type != ShortCodeType.Client || !parentDecode.Data.Id.HasValue)
                    {
                        Message = "Parent client short code is invalid or not a client.";
                        return Page();
                    }

                    var dto = new CreateProjectDto
                    {
                        ClientId = parentDecode.Data.Id.Value,
                        ProjectName = ProjectName ?? string.Empty,
                        ProjectDescription = ProjectDescription,
                        ShortCode = Shortcode ?? string.Empty
                    };
                    var result = await _management.AddNewProject(user.Id, roleType, dto);
                    Message = result.Success ? "Project created successfully." : (result.Error ?? "An error occurred while creating the project.");
                }
                else
                {
                    // Update — requires RecordId and ParentId
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
                    Message = result.Success ? "Project updated successfully." : (result.Error ?? "An error occurred while updating the project.");
                }
            }
            else if (recordType == "Task")
            {
                if (mode == "create")
                {
                    // Resolve parent project short code to get ProjectId
                    if (string.IsNullOrEmpty(ParentShortCode))
                    {
                        Message = "Please enter the parent project short code.";
                        return Page();
                    }

                    var parentDecode = await _scs.DecodeShortCode(ParentShortCode);
                    if (!parentDecode.Success || parentDecode.Data.Type != ShortCodeType.Project || !parentDecode.Data.Id.HasValue)
                    {
                        Message = "Parent project short code is invalid or not a project.";
                        return Page();
                    }

                    var dto = new CreateTaskDto
                    {
                        ProjectId = parentDecode.Data.Id.Value,
                        TaskName = TaskName ?? string.Empty,
                        TaskDescription = TaskDescription,
                        ShortCode = Shortcode ?? string.Empty
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
}