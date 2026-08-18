using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST API endpoints for managing to-do tasks within a to-do list.
/// </summary>
[Authorize]
[ApiController]
[Route("api/todolists/{todoListId:int}/tasks")]
public class TodoTaskController : ControllerBase
{
    private readonly ITodoTaskDatabaseService taskService;
    private readonly ILogger<TodoTaskController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskController"/> class.
    /// </summary>
    /// <param name="taskService">The task database service.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoTaskController(ITodoTaskDatabaseService taskService, ILogger<TodoTaskController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>Returns all tasks for a given to-do list.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <returns>A JSON array of <see cref="TodoTask"/>.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoTask>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int todoListId)
    {
        this.logger.LogInformation("Fetching all tasks for list {TodoListId}.", todoListId);

        var tasks = await this.taskService.GetAllTodoTasksAsync(todoListId);
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
            Tags = t.Tags,
            Comments = t.Comments,
        });
        return this.Ok(models);
    }

    /// <summary>Returns a single task by its identifier.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task model.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int todoListId, int id)
    {
        this.logger.LogInformation("Fetching task {TaskId} from list {TodoListId}.", id, todoListId);

        var task = await this.taskService.GetTaskByIdAsync(id);
        if (task is null || task.TodoListId != todoListId)
        {
            return this.NotFound();
        }

        return this.Ok(new TodoTask
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status,
            AssignedTo = task.AssignedTo,
            TodoListId = task.TodoListId,
            Tags = task.Tags,
            Comments = task.Comments,
        });
    }

    /// <summary>Creates a new task in the specified to-do list.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="model">The task data.</param>
    /// <returns>The created task with status 201.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(int todoListId, [FromBody] TodoTask model)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        if (model is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        this.logger.LogInformation("Creating new task '{Title}' in list {TodoListId}.", model.Title, todoListId);

        var created = await this.taskService.CreateTaskAsync(new TodoTask
        {
            Title = model.Title,
            Description = model.Description,
            CreatedAt = model.CreatedAt,
            DueDate = model.DueDate,
            Status = model.Status,
            AssignedTo = model.AssignedTo,
            TodoListId = todoListId,
            Tags = model.Tags,
        }, userId);

        model.Id = created.Id;
        model.CreatedAt = created.CreatedAt;
        model.TodoListId = created.TodoListId;

        return this.CreatedAtAction(nameof(this.GetById), new { todoListId, id = model.Id }, model);
    }

    /// <summary>Updates an existing task.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="id">The task identifier.</param>
    /// <param name="model">The updated task data.</param>
    /// <returns>The updated task model.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TodoTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int todoListId, int id, [FromBody] TodoTask model)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        if (model is null || id != model.Id || !this.ModelState.IsValid)
        {
            return this.BadRequest(model);
        }

        this.logger.LogInformation("Updating task {TaskId} in list {TodoListId}.", id, todoListId);

        try
        {
            var updated = await this.taskService.UpdateTaskAsync(new TodoTask
            {
                Id = id,
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = model.Status,
                AssignedTo = model.AssignedTo,
                TodoListId = todoListId,
                Tags = model.Tags,
            }, userId);

            return this.Ok(new TodoTask
            {
                Id = updated!.Id,
                Title = updated.Title,
                Description = updated.Description,
                CreatedAt = updated.CreatedAt,
                DueDate = updated.DueDate,
                Status = updated.Status,
                AssignedTo = updated.AssignedTo,
                TodoListId = updated.TodoListId,
                Tags = updated.Tags,
                Comments = updated.Comments,
            });
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }
    }

    /// <summary>Deletes a task by its identifier.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="id">The task identifier.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int todoListId, int id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }

        var task = await this.taskService.GetTaskByIdAsync(id);
        if (task is null || task.TodoListId != todoListId)
        {
            return this.NotFound();
        }

        this.logger.LogInformation("Deleting task {TaskId} from list {TodoListId}.", id, todoListId);

        try
        {
            await this.taskService.DeleteTaskAsync(id, userId);
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }

        return this.NoContent();
    }
}
