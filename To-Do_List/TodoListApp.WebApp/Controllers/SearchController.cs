using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser UI requests for searching to-do tasks within a to-do list.
/// </summary>
[Authorize]
public class SearchController : Controller
{
    private readonly ITodoTaskWebApiService taskService;
    private readonly ILogger<SearchController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// </summary>
    /// <param name="taskService">The Web API task service.</param>
    /// <param name="logger">The logger instance.</param>
    public SearchController(ITodoTaskWebApiService taskService, ILogger<SearchController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string? title, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo, [FromQuery] string? status, [FromQuery] string? sortBy)
    {
        this.logger.LogInformation("Searching for tasks.");

        var hasSearchCriteria = !string.IsNullOrWhiteSpace(title) || dateFrom.HasValue || dateTo.HasValue || !string.IsNullOrWhiteSpace(status) || !string.IsNullOrWhiteSpace(sortBy);

        IEnumerable<TodoTask> tasks = new List<TodoTask>();
        if (hasSearchCriteria)
        {
            tasks = await this.taskService.SearchTasksAsync(title, dateFrom, dateTo, status, sortBy);
        }

        this.ViewBag.TitleFilter = title;
        this.ViewBag.DateFromFilter = dateFrom?.ToString("yyyy-MM-dd");
        this.ViewBag.DateToFilter = dateTo?.ToString("yyyy-MM-dd");
        this.ViewBag.StatusFilter = status;
        this.ViewBag.SortByFilter = sortBy;

        return this.View(tasks);
    }
}
