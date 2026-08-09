using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST API endpoints for fetching assigned tasks.
/// </summary>
[ApiController]
[Route("api/assigned-tasks")]
public class AssignedTasksController : ControllerBase
{
    private readonly ITodoTaskDatabaseService taskService;
    private readonly ILogger<AssignedTasksController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignedTasksController"/> class.
    /// </summary>
    /// <param name="taskService">The task database service.</param>
    /// <param name="logger">The logger instance.</param>
    public AssignedTasksController(ITodoTaskDatabaseService taskService, ILogger<AssignedTasksController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>Returns tasks assigned to a specific user.</summary>
    /// <param name="userId">The assigned user ID.</param>
    /// <param name="status">Optional status filter ("Active", "Completed", "All").</param>
    /// <param name="sortBy">Optional sort parameter ("Name", "DueDate").</param>
    /// <returns>A JSON array of <see cref="TodoTask"/>.</returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(IEnumerable<TodoTask>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignedTasks(string userId, [FromQuery] string? status, [FromQuery] string? sortBy)
    {
        var tasks = await this.taskService.GetAssignedTasksAsync(userId, status, sortBy);

        var models = tasks.Select(t => new TodoTask
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            Status = t.Status,
            AssignedTo = t.AssignedTo,
            TodoListId = t.TodoListId,
        });

        return this.Ok(models);
    }
}
