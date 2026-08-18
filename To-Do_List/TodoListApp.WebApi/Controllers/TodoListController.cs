using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST API endpoints for managing to-do lists.
/// </summary>
[Authorize]
[ApiController]
[Route("api/todolists")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListDatabaseService todoListService;
    private readonly ILogger<TodoListController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="todoListService">The to-do list database service.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoListController(ITodoListDatabaseService todoListService, ILogger<TodoListController> logger)
    {
        this.todoListService = todoListService;
        this.logger = logger;
    }

    /// <summary>Returns the list of all to-do lists for the authenticated user.</summary>
    /// <returns>A JSON array of <see cref="TodoList"/>.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoList>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        this.logger.LogInformation("API request received to fetch all todo lists.");

        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        var tasks = await this.todoListService.GetAllTodoListsAsync(userId);
        return this.Ok(tasks);
    }

    /// <summary>Returns a specific to-do list if the user has access to it.</summary>
    /// <param name="id">The identifier of the list.</param>
    /// <returns><see cref="TodoList"/></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        this.logger.LogInformation("API request received to fetch the todo list.");

        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        var list = await this.todoListService.GetTodoListByIdAsync(id, userId);
        if (list == null)
        {
            return this.NotFound();
        }

        if (list.AccessLevel == Data.UserRole.NoAccess)
        {
            return this.Forbid();
        }

        return this.Ok(list);
    }

    /// <summary>Creates a new to-do list.</summary>
    /// <param name="todoList">The to-do list data.</param>
    /// <returns>The created to-do list with status 201.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TodoList), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TodoList todoList)
    {
        this.logger.LogInformation("API request received to create a new todo list titled '{Title}'.", todoList.Title);

        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        var created = await this.todoListService.CreateTodoListAsync(todoList, userId);
        return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates an existing to-do list.</summary>
    /// <param name="id">The identifier of the list to update.</param>
    /// <param name="todoList">The updated to-do list data.</param>
    /// <returns>The updated to-do list.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TodoList), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);
        this.logger.LogInformation("API request received to update todo list with ID {Id}.", id);

        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        if (id != todoList.Id)
        {
            return this.BadRequest();
        }

        try
        {
            var updated = await this.todoListService.UpdateTodoListAsync(todoList, userId);
            if (updated == null)
            {
                return this.NotFound();
            }

            return this.Ok(updated);
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }
    }

    /// <summary>Deletes a to-do list by its identifier.</summary>
    /// <param name="id">The identifier of the list to delete.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        this.logger.LogInformation("API request received to delete todo list with ID {Id}.", id);

        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        try
        {
            var deleted = await this.todoListService.DeleteTodoListAsync(id, userId);
            if (!deleted)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }
    }
}
