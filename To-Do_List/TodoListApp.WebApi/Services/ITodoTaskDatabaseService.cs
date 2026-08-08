using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Defines the contract for managing to-do tasks in the database.
/// </summary>
public interface ITodoTaskDatabaseService
{
    /// <summary>Returns all tasks for a given to-do list.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <returns>A collection of <see cref="TodoTask"/> objects.</returns>
    Task<IEnumerable<TodoTask>> GetAllTodoTasksAsync(int todoListId);

    /// <summary>Returns a single task by its identifier.</summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The <see cref="TodoTask"/>, or <c>null</c> if not found.</returns>
    Task<TodoTask?> GetTaskByIdAsync(int id);

    /// <summary>Creates a new task.</summary>
    /// <param name="task">The task to create.</param>
    /// <param name="userId">The user identifier creating the task.</param>
    /// <returns>The created <see cref="TodoTask"/>.</returns>
    Task<TodoTask> CreateTaskAsync(TodoTask task, string userId);

    /// <summary>Updates an existing task.</summary>
    /// <param name="task">The task with updated values.</param>
    /// <param name="userId">The user identifier updating the task.</param>
    /// <returns>The updated <see cref="TodoTask"/>.</returns>
    Task<TodoTask?> UpdateTaskAsync(TodoTask task, string userId);

    /// <summary>Deletes a task by its identifier.</summary>
    /// <param name="id">The identifier of the task to delete.</param>
    /// <param name="userId">The user identifier deleting the task.</param>
    /// <returns><c>true</c> if deleted; <c>false</c> if not found.</returns>
    Task<bool> DeleteTaskAsync(int id, string userId);

    /// <summary>Returns all tasks assigned to a specific user, with optional filtering and sorting.</summary>
    /// <param name="userId">The assigned user ID.</param>
    /// <param name="statusFilter">Optional status filter ("Active", "Completed", or "All").</param>
    /// <param name="sortBy">Optional sort field ("DueDate" or "Name").</param>
    /// <returns>A collection of <see cref="TodoTask"/> objects.</returns>
    Task<IEnumerable<TodoTask>> GetAssignedTasksAsync(string userId, string? statusFilter, string? sortBy);

    /// <summary>Searches for tasks based on specific criteria.</summary>
    /// <param name="title">Optional part of the task title.</param>
    /// <param name="dateFrom">Optional start of date range filter (applied to CreatedAt).</param>
    /// <param name="dateTo">Optional end of date range filter (applied to CreatedAt).</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="sortBy">Optional sort field ("Title", "CreatedAt", "DueDate").</param>
    /// <returns>A collection of matching <see cref="TodoTask"/> objects.</returns>
    Task<IEnumerable<TodoTask>> SearchTasksAsync(string? title, DateTime? dateFrom, DateTime? dateTo, Models.TodoTaskStatus? status, string? sortBy);

    /// <summary>Returns all tags attached to tasks in to-do lists the given user can access (owned or shared).</summary>
    /// <param name="userId">The identifier of the user requesting the tags.</param>
    /// <returns>A collection of <see cref="string"/> objects.</returns>
    Task<IEnumerable<string>> GetAllTagsAsync(string userId);

    /// <summary>Returns all tasks for a given tag, restricted to to-do lists the given user can access (owned or shared).</summary>
    /// <param name="tagName">The name of the tag.</param>
    /// <param name="userId">The identifier of the user requesting the tasks.</param>
    /// <returns>A collection of <see cref="TodoTask"/> objects.</returns>
    Task<IEnumerable<TodoTask>> GetTasksByTagAsync(string tagName, string userId);

    /// <summary>Adds a new comment to a task.</summary>
    /// <param name="comment">The comment to add.</param>
    /// <returns>The created comment model.</returns>
    Task<TodoTaskCommentModel> AddCommentAsync(TodoTaskCommentModel comment);

    /// <summary>Updates the text of an existing comment.</summary>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="text">The new comment text.</param>
    /// <returns>The updated comment, or null if not found.</returns>
    Task<TodoTaskCommentModel?> UpdateCommentAsync(int commentId, string text);

    /// <summary>Deletes a comment.</summary>
    /// <param name="commentId">The comment identifier.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteCommentAsync(int commentId);
}
