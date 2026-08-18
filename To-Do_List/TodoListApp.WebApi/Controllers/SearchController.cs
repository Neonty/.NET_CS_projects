using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST API endpoints for searching tasks globally.
/// </summary>
[Authorize]
[ApiController]
[Route("api/search")]
public class SearchController : Controller
{
    private readonly ITodoTaskDatabaseService taskService;
    private readonly ILogger<SearchController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// </summary>
    /// <param name="taskService">The task database service.</param>
    /// <param name="logger">The logger instance.</param>
    public SearchController(ITodoTaskDatabaseService taskService, ILogger<SearchController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>Searches for tasks globally across all to-do lists.</summary>
    /// <param name="title">Optional part of the task title.</param>
    /// <param name="dateFrom">Optional start of date range filter.</param>
    /// <param name="dateTo">Optional end of date range filter.</param>
    /// <param name="status">Optional status filter ("NotStarted", "InProgress", "Completed").</param>
    /// <param name="sortBy">Optional sort field ("Title", "CreatedAt", "DueDate").</param>
    /// <returns>A JSON array of <see cref="TodoTask"/>.</returns>
    [HttpGet("tasks")]
    [ProducesResponseType(typeof(IEnumerable<TodoTask>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchTasks(
        [FromQuery] string? title,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string? status,
        [FromQuery] string? sortBy)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        Models.TodoTaskStatus? statusEnum = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<Models.TodoTaskStatus>(status, ignoreCase: true, out var parsed))
        {
            statusEnum = parsed;
        }

        var tasks = await this.taskService.SearchTasksAsync(userId, title, dateFrom, dateTo, statusEnum, sortBy);

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
