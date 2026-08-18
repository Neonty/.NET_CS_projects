using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoListApp.WebApp.Services;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.ViewComponents;

/// <summary>
/// View model for the sidebar component.
/// </summary>
public class SidebarViewModel
{
    public int UpcomingCount { get; set; }

    public int TodayCount { get; set; }

    public IEnumerable<TodoListWebApiModel> Lists { get; set; } = new List<TodoListWebApiModel>();

    public IEnumerable<string> Tags { get; set; } = new List<string>();

    public bool IsAuthenticated { get; set; }

    public string? UserEmail { get; set; }
}

/// <summary>
/// Renders the sidebar navigation with lists, tags, and task counts.
/// </summary>
public class SidebarViewComponent : ViewComponent
{
    private readonly ITodoListWebApiService listService;
    private readonly ITodoTaskWebApiService taskService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SidebarViewComponent"/> class.
    /// </summary>
    /// <param name="listService">The list service.</param>
    /// <param name="taskService">The task service.</param>
    public SidebarViewComponent(ITodoListWebApiService listService, ITodoTaskWebApiService taskService)
    {
        this.listService = listService;
        this.taskService = taskService;
    }

    /// <summary>
    /// Invokes the sidebar view component and returns the rendered view.
    /// </summary>
    /// <returns>The sidebar view.</returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var vm = new SidebarViewModel { IsAuthenticated = false };

        if (this.UserClaimsPrincipal?.Identity?.IsAuthenticated == true)
        {
            var userId = this.UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                vm.IsAuthenticated = true;
                vm.UserEmail = this.UserClaimsPrincipal.Identity.Name;
                vm.Lists = await this.listService.GetTodoListsAsync(userId);
                vm.Tags = await this.taskService.GetAllTagsAsync(userId);

                var tasks = await this.taskService.GetAssignedTasksAsync(userId, "Active", null);
                var today = DateTime.Today;

                vm.TodayCount = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date == today);
                vm.UpcomingCount = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date > today);
            }
        }

        return this.View(vm);
    }
}
