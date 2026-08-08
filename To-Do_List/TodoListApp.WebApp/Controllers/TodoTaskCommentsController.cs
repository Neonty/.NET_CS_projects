using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Controller for managing comments in the Web UI.
/// </summary>
[Authorize]
public class TodoTaskCommentsController : Controller
{
    private readonly ITodoTaskWebApiService taskService;
    private readonly ILogger<TodoTaskCommentsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskCommentsController"/> class.
    /// </summary>
    /// <param name="taskService">The task web api service.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoTaskCommentsController(ITodoTaskWebApiService taskService, ILogger<TodoTaskCommentsController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>Handles the form submission for adding a comment.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="text">The comment text.</param>
    /// <param name="returnUrl">The URL to redirect back to.</param>
    /// <returns>A redirect result.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int todoListId, int taskId, string text, string returnUrl)
    {
        this.logger.LogInformation("Submitting new comment for task {TaskId}.", taskId);

        if (!string.IsNullOrEmpty(text))
        {
            var comment = new TodoTaskComment()
            {
                Text = text,
                CreatedBy = this.User.Identity?.Name ?? "Unknown",
            };
            await this.taskService.AddCommentAsync(todoListId, taskId, comment);
        }

        return !string.IsNullOrEmpty(returnUrl) ? this.Redirect(returnUrl) : this.RedirectToAction("Details", "TodoTask", new { todoListId, id = taskId });
    }

    /// <summary>Handles the form submission for editing a comment.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="text">The new comment text.</param>
    /// <param name="returnUrl">The URL to redirect back to.</param>
    /// <returns>A redirect result.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComment(int todoListId, int taskId, int commentId, string text, string returnUrl)
    {
        this.logger.LogInformation("Submitting edit for comment {CommentId}.", commentId);

        if (!string.IsNullOrWhiteSpace(text))
        {
            await this.taskService.UpdateCommentAsync(todoListId, taskId, commentId, text);
        }

        return !string.IsNullOrEmpty(returnUrl) ? this.Redirect(returnUrl) : this.RedirectToAction("Details", "TodoTask", new { todoListId, id = taskId });
    }

    /// <summary>Handles the form submission for deleting a comment.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="returnUrl">The URL to redirect back to.</param>
    /// <returns>A redirect result.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int todoListId, int taskId, int commentId, string returnUrl)
    {
        this.logger.LogInformation("Submitting deletion for comment {CommentId}.", commentId);

        await this.taskService.DeleteCommentAsync(todoListId, taskId, commentId);
        return !string.IsNullOrEmpty(returnUrl) ? this.Redirect(returnUrl) : this.RedirectToAction("Details", "TodoTask", new { todoListId, id = taskId });
    }
}
