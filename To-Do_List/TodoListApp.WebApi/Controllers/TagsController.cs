using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Controller for managing tags and retrieving tasks by tag in the Web API.
/// </summary>
[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly ITodoTaskDatabaseService taskService;
    private readonly ILogger<TagsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagsController"/> class.
    /// </summary>
    /// <param name="taskService">The task database service.</param>
    /// <param name="logger">The logger instance.</param>
    public TagsController(ITodoTaskDatabaseService taskService, ILogger<TagsController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>
    /// Gets a list of tags attached to tasks in to-do lists the requesting user has access to (owned or shared).
    /// </summary>
    /// <param name="userId">The identifier of the requesting user.</param>
    /// <returns>An action result containing a list of tag names.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTags([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return this.BadRequest("userId is required.");
        }

        this.logger.LogInformation("Fetching tags accessible to user {UserId} from API.", userId);
        var tags = await this.taskService.GetAllTagsAsync(userId);
        return this.Ok(tags);
    }

    /// <summary>
    /// Gets a list of tasks that have the specified tag attached, restricted to to-do lists the requesting user has access to.
    /// </summary>
    /// <param name="tagName">The name of the tag.</param>
    /// <param name="userId">The identifier of the requesting user.</param>
    /// <returns>An action result containing a list of tasks.</returns>
    [HttpGet("{tagName}/tasks")]
    public async Task<IActionResult> GetTasksByTag(string tagName, [FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return this.BadRequest("userId is required.");
        }

        this.logger.LogInformation("Fetching tasks for tag {TagName} accessible to user {UserId} from API.", tagName, userId);
        var tasks = await this.taskService.GetTasksByTagAsync(tagName, userId);
        var models = tasks.Select(t => new TodoTaskModel
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            Status = t.Status,
            AssignedTo = t.AssignedTo,
            TodoListId = t.TodoListId,
            Tags = t.Tags,
        });
        return this.Ok(models);
    }
}
