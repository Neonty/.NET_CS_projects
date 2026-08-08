using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser UI requests for managing assigned to-do tasks within a to-do list.
/// </summary>
[Authorize]
public class AssignedTasksController : Controller
{
    private readonly ITodoTaskWebApiService taskService;
    private readonly ILogger<AssignedTasksController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignedTasksController"/> class.
    /// </summary>
    /// <param name="taskService">The Web API task service.</param>
    /// <param name="logger">The logger instance.</param>
    public AssignedTasksController(ITodoTaskWebApiService taskService, ILogger<AssignedTasksController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string status = "Active", [FromQuery] string sortBy = "DueDate")
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return this.Unauthorized();
        }

        this.logger.LogInformation("Fetching assigned tasks for user ID {UserId}.", userId);

        var tasks = await this.taskService.GetAssignedTasksAsync(userId, status, sortBy);

        this.ViewBag.StatusFilter = status;
        this.ViewBag.SortBy = sortBy;

        return this.View(tasks);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int todoListId, int taskId, TodoTaskStatus newStatus, string statusFilter, string sortBy)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        var task = await this.taskService.GetTaskByIdAsync(todoListId, taskId);

        if (task == null)
        {
            return this.NotFound();
        }

        task.Status = newStatus;

        await this.taskService.UpdateTaskAsync(todoListId, task, userId);

        return this.RedirectToAction(nameof(this.Index), new { status = statusFilter, sortBy = sortBy });
    }
}
