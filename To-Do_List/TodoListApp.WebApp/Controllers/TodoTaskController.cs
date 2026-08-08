using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Data;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser UI requests for managing to-do tasks within a to-do list.
/// </summary>
[Authorize]
public class TodoTaskController : Controller
{
    private readonly ITodoTaskWebApiService taskService;
    private readonly ILogger<TodoTaskController> logger;
    private readonly UserManager<ApplicationUser> userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskController"/> class.
    /// </summary>
    /// <param name="taskService">The Web API task service.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoTaskController(ITodoTaskWebApiService taskService, ILogger<TodoTaskController> logger, UserManager<ApplicationUser> userManager)
    {
        this.taskService = taskService;
        this.logger = logger;
        this.userManager = userManager;
    }

    /// <summary>Displays the list of tasks for a given to-do list.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <returns>The index view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(int todoListId)
    {
        this.logger.LogInformation("Requesting tasks for todo list ID {TodoListId}.", todoListId);

        var tasks = await this.taskService.GetTasksByTodoListIdAsync(todoListId);

        this.ViewBag.TodoListId = todoListId;
        return this.View(tasks);
    }

    /// <summary>Displays the task details page.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="id">The task identifier.</param>
    /// <returns>The details view.</returns>
    public async Task<IActionResult> Details(int todoListId, int id)
    {
        this.logger.LogInformation("Viewing details for task ID {TaskId} in todo list {TodoListId}.", id, todoListId);

        var task = await this.taskService.GetTaskByIdAsync(todoListId, id);

        if (task is null)
        {
            return this.NotFound();
        }

        if (!string.IsNullOrEmpty(task.AssignedTo))
        {
            var assignee = await this.userManager.FindByIdAsync(task.AssignedTo);
            task.AssigneeEmail = assignee?.Email;
        }

        this.ViewBag.TodoListId = todoListId;
        return this.View(task);
    }

    /// <summary>Displays the create task form.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <returns>The create view.</returns>
    [HttpGet]
    public IActionResult Create(int todoListId)
    {
        this.logger.LogInformation("Navigating to create task form for todo list {TodoListId}.", todoListId);
        this.ViewBag.TodoListId = todoListId;
        return this.View(new TodoTask { TodoListId = todoListId });
    }

    /// <summary>Handles the create task form submission.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="task">The new task data.</param>
    /// <returns>Redirects to Index on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int todoListId, TodoTask task)
    {
        if (task is null)
        {
            return this.BadRequest();
        }

        if (!this.ModelState.IsValid)
        {
            this.ViewBag.TodoListId = todoListId;
            return this.View(task);
        }

        if (this.User.Identity?.IsAuthenticated == true)
        {
            task.AssignedTo = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        this.logger.LogInformation("Creating task in todo list {TodoListId}.", todoListId);

        try
        {
            await this.taskService.CreateTaskAsync(todoListId, task, userId);
            return this.RedirectToAction(nameof(this.Index), new { todoListId });
        }
        catch (UnauthorizedAccessException)
        {
            this.ModelState.AddModelError(string.Empty, "You don't have permission to create tasks in this list. Only Owners and Editors can create tasks.");
            this.ViewBag.TodoListId = todoListId;
            return this.View(task);
        }
    }

    /// <summary>Displays the edit task form.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="id">The task identifier.</param>
    /// <returns>The edit view.</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int todoListId, int id)
    {
        this.logger.LogInformation("Fetching task ID {TaskId} for editing in todo list {TodoListId}.", id, todoListId);

        var task = await this.taskService.GetTaskByIdAsync(todoListId, id);

        if (task is null)
        {
            return this.NotFound();
        }

        if (!string.IsNullOrEmpty(task.AssignedTo))
        {
            var assignee = await this.userManager.FindByIdAsync(task.AssignedTo);
            task.AssigneeEmail = assignee?.Email;
        }

        this.ViewBag.TodoListId = todoListId;
        return this.View(task);
    }

    /// <summary>Handles the edit task form submission.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="id">The task identifier.</param>
    /// <param name="task">The updated task data.</param>
    /// <returns>Redirects to Index on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int todoListId, int id, TodoTask task)
    {
        if (task is null)
        {
            return this.BadRequest();
        }

        if (id != task.Id || !this.ModelState.IsValid)
        {
            this.ViewBag.TodoListId = todoListId;
            return this.View(task);
        }

        if (!string.IsNullOrWhiteSpace(task.AssigneeEmail))
        {
            var assignee = await this.userManager.FindByEmailAsync(task.AssigneeEmail);
            if (assignee == null)
            {
                this.ModelState.AddModelError(nameof(task.AssigneeEmail), "Пользователь с таким email не найден.");
                this.ViewBag.TodoListId = todoListId;
                return this.View(task);
            }
            task.AssignedTo = assignee.Id;
        }
        else
        {
            task.AssignedTo = null;
        }

        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        this.logger.LogInformation("Updating task ID {TaskId} in todo list {TodoListId}.", id, todoListId);

        try
        {
            await this.taskService.UpdateTaskAsync(todoListId, task, userId);
            return this.RedirectToAction(nameof(this.Index), new { todoListId });
        }
        catch (UnauthorizedAccessException)
        {
            this.ModelState.AddModelError(string.Empty, "You don't have permission to modify this task. Only Owners and Editors can modify tasks.");
            this.ViewBag.TodoListId = todoListId;
            return this.View(task);
        }
    }

    /// <summary>Displays the delete task confirmation page.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="id">The task identifier.</param>
    /// <returns>The delete view.</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int todoListId, int id)
    {
        this.logger.LogInformation("Fetching task ID {TaskId} to confirm deletion in todo list {TodoListId}.", id, todoListId);

        var task = await this.taskService.GetTaskByIdAsync(todoListId, id);

        if (task is null)
        {
            return this.NotFound();
        }

        this.ViewBag.TodoListId = todoListId;
        return this.View(task);
    }

    /// <summary>Handles the delete task confirmation.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="id">The task identifier.</param>
    /// <returns>Redirects to Index.</returns>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int todoListId, int id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        this.logger.LogInformation("Deleting task ID {TaskId} from todo list {TodoListId}.", id, todoListId);

        try
        {
            await this.taskService.DeleteTaskAsync(todoListId, id, userId);
            return this.RedirectToAction(nameof(this.Index), new { todoListId });
        }
        catch (UnauthorizedAccessException)
        {
            this.ModelState.AddModelError(string.Empty, "You don't have permission to delete this task. Only Owners and Editors can delete tasks.");
            var task = await this.taskService.GetTaskByIdAsync(todoListId, id);
            this.ViewBag.TodoListId = todoListId;
            return this.View(task);
        }
    }
}
