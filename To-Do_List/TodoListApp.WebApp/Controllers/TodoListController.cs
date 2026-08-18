using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser UI requests for managing to-do lists.
/// </summary>
[Authorize]
public class TodoListController : Controller
{
    private readonly ITodoListWebApiService todoListService;
    private readonly ILogger<TodoListController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="todoListService">The Web API service.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoListController(ITodoListWebApiService todoListService, ILogger<TodoListController> logger)
    {
        this.todoListService = todoListService;
        this.logger = logger;
    }

    /// <summary>Displays the list of to-do lists.</summary>
    /// <returns>The index view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        this.logger.LogInformation("Requesting the todo list index view.");

        var lists = await this.todoListService.GetTodoListsAsync(userId);
        return this.View(lists);
    }

    /// <summary>Displays the create form.</summary>
    /// <returns>The create view.</returns>
    [HttpGet]
    public IActionResult Create()
    {
        this.logger.LogInformation("Navigating to the create todo list form.");
        return this.View(new TodoListWebApiModel());
    }

    /// <summary>Handles the create form submission.</summary>
    /// <param name="todoList">The new to-do list data.</param>
    /// <returns>Redirects to Index on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoListWebApiModel todoList)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        if (todoList is null)
        {
            return this.BadRequest();
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(todoList);
        }

        this.logger.LogInformation("Submitting creation of a new todo list titled '{Title}'.", todoList.Title);
        await this.todoListService.CreateTodoListAsync(todoList, userId);
        return this.RedirectToAction(nameof(this.Index));
    }

    /// <summary>Displays the sharing management page. Available to Owner and Editor.</summary>
    /// <param name="id">The to-do list identifier.</param>
    /// <returns>The manage view.</returns>
    [HttpGet]
    public async Task<IActionResult> Manage(int id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        var list = await this.todoListService.GetTodoListByIdAsync(id, userId);
        if (list == null || (list.AccessLevel != TodoListApp.Data.UserRole.Owner && list.AccessLevel != TodoListApp.Data.UserRole.Editor))
        {
            return this.Forbid();
        }

        try
        {
            var sharedUsers = await this.todoListService.GetSharedUsersAsync(id, userId);
            ViewBag.SharedUsers = sharedUsers;
            this.logger.LogInformation("Successfully fetched {Count} shared users for list {ListId}", sharedUsers.Count(), id);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to fetch shared users for list {ListId}", id);
            ViewBag.SharedUsers = new List<SharedUserInfo>();
        }

        return this.View(list);
    }

    /// <summary>Displays the edit form.</summary>
    /// <param name="id">The to-do list identifier.</param>
    /// <returns>The edit view.</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        this.logger.LogInformation("Fetching todo list with ID {Id} for editing.", id);

        var todoList = await this.todoListService.GetTodoListByIdAsync(id, userId);

        if (todoList is null)
        {
            return this.NotFound();
        }

        return this.View(todoList);
    }

    /// <summary>Handles the edit form submission.</summary>
    /// <param name="id">The to-do list identifier.</param>
    /// <param name="todoList">The updated to-do list data.</param>
    /// <returns>Redirects to Index on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoListWebApiModel todoList)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        if (todoList is null)
        {
            return this.BadRequest();
        }

        if (id != todoList.Id || !this.ModelState.IsValid)
        {
            return this.View(todoList);
        }

        this.logger.LogInformation("Submitting update for todo list ID {Id}.", id);
        await this.todoListService.UpdateTodoListAsync(todoList, userId);
        return this.RedirectToAction(nameof(this.Index));
    }

    /// <summary>Displays the delete confirmation page.</summary>
    /// <param name="id">The to-do list identifier.</param>
    /// <returns>The delete view.</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        this.logger.LogInformation("Fetching todo list with ID {Id} to confirm deletion.", id);

        var todoList = await this.todoListService.GetTodoListByIdAsync(id, userId);

        if (todoList is null)
        {
            return this.NotFound();
        }

        return this.View(todoList);
    }

    /// <summary>Handles the delete confirmation.</summary>
    /// <param name="id">The to-do list identifier.</param>
    /// <returns>Redirects to Index.</returns>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        this.logger.LogInformation("Executing delete confirmation for todo list ID {Id}.", id);

        await this.todoListService.DeleteTodoListAsync(id, userId);
        return this.RedirectToAction(nameof(this.Index));
    }
}
