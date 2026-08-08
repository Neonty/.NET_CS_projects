using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Controller for viewing tags and retrieving tasks by tag in the Web UI.
/// </summary>
[Authorize]
public class TagsController : Controller
{
    private readonly ITodoTaskWebApiService taskService;
    private readonly ILogger<TagsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagsController"/> class.
    /// </summary>
    /// <param name="taskService">The task web api service.</param>
    /// <param name="logger">The logger instance.</param>
    public TagsController(ITodoTaskWebApiService taskService, ILogger<TagsController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>
    /// Displays a view containing a list of all distinct tags.
    /// </summary>
    /// <returns>The index view with the list of tags.</returns>
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        this.logger.LogInformation("Requesting list of all tags.");
        var tags = await this.taskService.GetAllTagsAsync(userId);
        return this.View(tags);
    }

    /// <summary>
    /// Displays a view containing a list of tasks that have the specified tag attached.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <returns>The view with the list of tasks, or redirects to index if name is empty.</returns>
    public async Task<IActionResult> Tasks(string name)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(name))
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        this.logger.LogInformation("Requesting tasks for tag {TagName}.", name);
        var tasks = await this.taskService.GetTasksByTagAsync(name, userId);
        this.ViewBag.TagName = name;
        return this.View(tasks);
    }
}
