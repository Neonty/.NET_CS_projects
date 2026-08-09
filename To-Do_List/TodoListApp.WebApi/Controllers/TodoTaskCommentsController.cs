using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST API endpoints for managing task comments.
/// </summary>
[ApiController]
[Route("api/todolists/{todoListId:int}/tasks/{taskId:int}/comments")]
public class TodoTaskCommentsController : ControllerBase
{
    private readonly ITodoTaskDatabaseService taskService;
    private readonly ILogger<TodoTaskCommentsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskCommentsController"/> class.
    /// </summary>
    /// <param name="taskService">The task database service.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoTaskCommentsController(ITodoTaskDatabaseService taskService, ILogger<TodoTaskCommentsController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>Adds a comment to a task.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="model">The comment data.</param>
    /// <returns>The created comment.</returns>
    [HttpPost]
    public async Task<IActionResult> AddComment(int todoListId, int taskId, [FromBody] TodoTaskComment model)
    {
        _ = todoListId;
        this.logger.LogInformation("Adding comment to task {TaskId} in list {TodoListId}.", taskId, todoListId);
        model.TodoTaskId = taskId;
        var created = await this.taskService.AddCommentAsync(model);
        return this.Ok(created);
    }

    /// <summary>Edits a comment.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="model">The updated comment data.</param>
    /// <returns>The updated comment.</returns>
    [HttpPut("{commentId:int}")]
    public async Task<IActionResult> EditComment(int todoListId, int taskId, int commentId, [FromBody] TodoTaskComment model)
    {
        _ = todoListId;
        _ = taskId;
        this.logger.LogInformation("Editing comment {CommentId} for task {TaskId}.", commentId, taskId);
        var updated = await this.taskService.UpdateCommentAsync(commentId, model.Text);
        if (updated == null)
        {
            return this.NotFound();
        }

        return this.Ok(updated);
    }

    /// <summary>Deletes a comment.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int todoListId, int taskId, int commentId)
    {
        _ = todoListId;
        _ = taskId;
        this.logger.LogInformation("Deleting comment {CommentId} from task {TaskId}.", commentId, taskId);
        var success = await this.taskService.DeleteCommentAsync(commentId);
        if (!success)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}
