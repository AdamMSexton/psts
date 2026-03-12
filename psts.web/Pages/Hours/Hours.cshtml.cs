using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using psts.web.Domain.Enums;
using psts.web.Dto;
using psts.web.Services;
using Psts.Web.Data;

namespace psts.web.Pages.Hours
{
    [Authorize] 
    public class HoursModel : PageModel
    {
        private readonly IShortCodeService _scs;
        private readonly IManagementService _management;
        private readonly ITimeServices _time;
        private readonly UserManager<AppUser> _userManager;

        // Short code entered in the lookup panel
        [BindProperty(SupportsGet = true)]
        public string? Shortcode { get; set; }

        // Selected task ID — set when user clicks a task from the list
        [BindProperty(SupportsGet = true)]
        public Guid? SelectedTaskId { get; set; }

        // What type was found after short code lookup
        public WorkItemType? FoundType { get; set; }

        // Client fields
        public string? ClientName { get; set; }

        // Project fields
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }

        // Task fields
        public string? TaskName { get; set; }
        public string? TaskDescription { get; set; }

        // Selected task details for the time entry panel
        public string? SelectedTaskName { get; set; }
        public string? SelectedTaskDescription { get; set; }

        // Child lists for clicking through
        public List<PstsProjectDefinition> ChildProjects { get; set; } = new();
        public List<PstsTaskDefinition> ChildTasks { get; set; } = new();

        // Time entry fields
        [BindProperty]
        public Guid TaskId { get; set; }
        [BindProperty]
        public DateOnly WorkCompletedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [BindProperty]
        public decimal WorkCompletedHours { get; set; }
        [BindProperty]
        public string? Notes { get; set; }

        // Messages
        public string? Message { get; set; }
        public string? SubmitMessage { get; set; }

        public HoursModel(IShortCodeService scs, IManagementService management, ITimeServices time, UserManager<AppUser> userManager)
        {
            _scs = scs;
            _management = management;
            _time = time;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            // If a task was selected from the list, load its details for the time entry panel
            if (SelectedTaskId.HasValue)
            {
                var taskResult = await _management.GetTask(SelectedTaskId.Value);
                if (taskResult.Success)
                {
                    SelectedTaskName = taskResult.Data.TaskName;
                    SelectedTaskDescription = taskResult.Data.TaskDescription;
                }
            }

            if (!string.IsNullOrEmpty(Shortcode))
            {
                var decodeShortCode = await _scs.DecodeShortCode(Shortcode);

                if (decodeShortCode.Success)
                {
                    var data = decodeShortCode.Data;

                    if (data.Type == WorkItemType.NotFound)
                    {
                        // Short code does not exist
                        Message = $"Short code '{Shortcode}' was not found.";
                        return;
                    }

                    // Valid short code found, set the type
                    FoundType = data.Type;

                    if (data.Type == WorkItemType.Client && data.Id.HasValue)
                    {
                        // Fetch client and its child projects
                        var clientResult = await _management.GetClient(data.Id.Value);
                        if (clientResult.Success)
                        {
                            ClientName = clientResult.Data.ClientName;
                            ChildProjects = clientResult.Data.Projects.ToList();
                        }
                        else
                        {
                            Message = "Short code found but failed to load client record.";
                        }
                    }
                    else if (data.Type == WorkItemType.Project && data.Id.HasValue)
                    {
                        // Fetch project and its child tasks
                        var projectResult = await _management.GetProject(data.Id.Value);
                        if (projectResult.Success)
                        {
                            ProjectName = projectResult.Data.ProjectName;
                            ProjectDescription = projectResult.Data.ProjectDescription;
                            ChildTasks = projectResult.Data.Tasks.ToList();
                        }
                        else
                        {
                            Message = "Short code found but failed to load project record.";
                        }
                    }
                    else if (data.Type == WorkItemType.Task && data.Id.HasValue)
                    {
                        // Task found directly — go straight to time entry
                        var taskResult = await _management.GetTask(data.Id.Value);
                        if (taskResult.Success)
                        {
                            TaskName = taskResult.Data.TaskName;
                            TaskDescription = taskResult.Data.TaskDescription;

                            // Auto-set the selected task ID so panel 3 appears
                            SelectedTaskId = data.Id;
                            SelectedTaskName = taskResult.Data.TaskName;
                            SelectedTaskDescription = taskResult.Data.TaskDescription;
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
            // Get the current user for EnteredBy and WorkCompletedBy
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                SubmitMessage = "Unable to identify current user.";
                return Page();
            }

            // Restore the shortcode from the hidden field so panels stay visible after POST
            Shortcode = Request.Form["shortcode"].ToString();

            // Re-run the GET logic to repopulate the page after POST
            await OnGetAsync();

            var dto = new NewTimeTransactionDto
            {
                TaskId = TaskId,
                EnteredBy = user.Id,
                WorkCompletedBy = user.Id,
                WorkCompletedDate = WorkCompletedDate,
                WorkCompletedHours = WorkCompletedHours,
                Notes = Notes ?? string.Empty
            };

            var result = await _time.CreateNewTimeTransaction(user.Id, RoleTypes.Employee, dto);
            SubmitMessage = result.Success ? "Hours submitted successfully." : (result.Error ?? "An error occurred while submitting hours.");

            return Page();
        }
    }
}