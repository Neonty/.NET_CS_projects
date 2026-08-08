using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Defines the contract for communicating with the Web API for to-do task operations.
/// </summary>
public interface ITodoTaskWebApiService
{
    /// <summary>Returns all tasks for a given to-do list from the Web API.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <returns>A collection of <see cref="TodoTask"/> objects.</returns>
    Task<IEnumerable<TodoTask>> GetTasksByTodoListIdAsync(int todoListId);

    /// <summary>Returns a single task by its identifier from the Web API.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>The <see cref="TodoTask"/>, or <c>null</c> if not found.</returns>
    Task<TodoTask?> GetTaskByIdAsync(int todoListId, int taskId);

    /// <summary>Creates a new task via the Web API.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="task">The task to create.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The created <see cref="TodoTask"/>.</returns>
    Task<TodoTask> CreateTaskAsync(int todoListId, TodoTask task, string userId);

    /// <summary>Updates an existing task via the Web API.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="task">The task with updated values.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The updated <see cref="TodoTask"/>.</returns>
    Task<TodoTask> UpdateTaskAsync(int todoListId, TodoTask task, string userId);

    /// <summary>Deletes a task by its identifier via the Web API.</summary>
    /// <param name="todoListId">The to-do list identifier.</param>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteTaskAsync(int todoListId, int taskId, string userId);

    /// <summary>Returns all tasks assigned to a specific user.</summary>
    /// <param name="userId">The assigned user ID.</param>
    /// <param name="statusFilter">Optional status filter ("Active", "Completed", "All").</param>
    /// <param name="sortBy">Optional sort parameter ("Name", "DueDate").</param>
    /// <returns>A collection of <see cref="TodoTask"/> objects.</returns>
    Task<IEnumerable<TodoTask>> GetAssignedTasksAsync(string userId, string? statusFilter, string? sortBy);

    /// <summary>Searches for tasks globally across all to-do lists.</summary>
    /// <param name="title">Optional part of the task title.</param>
    /// <param name="dateFrom">Optional start of date range filter (applied to CreatedAt).</param>
    /// <param name="dateTo">Optional end of date range filter (applied to CreatedAt).</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="sortBy">Optional sort field ("Title", "CreatedAt", "DueDate").</param>
    /// <returns>A collection of matching <see cref="TodoTask"/> objects.</returns>
    Task<IEnumerable<TodoTask>> SearchTasksAsync(string? title, DateTime? dateFrom, DateTime? dateTo, string? status, string? sortBy);

    /// <summary>
    /// Retrieves a list of tags, scoped to to-do lists the given user can access, from the Web API.
    /// </summary>
    /// <param name="userId">The identifier of the requesting user.</param>
    /// <returns>A collection of tag names.</returns>
    Task<IEnumerable<string>> GetAllTagsAsync(string userId);

    /// <summary>
    /// Retrieves all tasks that are associated with the specified tag name, scoped to to-do lists the given user can access, from the Web API.
    /// </summary>
    /// <param name="tagName">The name of the tag.</param>
    /// <param name="userId">The identifier of the requesting user.</param>
    /// <returns>A collection of tasks containing the specified tag.</returns>
    Task<IEnumerable<TodoTask>> GetTasksByTagAsync(string tagName, string userId);

    /// <summary>Adds a comment to a task via the Web API.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="comment">The comment to add.</param>
    /// <returns>The created comment.</returns>
    Task<TodoTaskComment> AddCommentAsync(int todoListId, int taskId, TodoTaskComment comment);

    /// <summary>Updates a comment via the Web API.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="text">The new text.</param>
    /// <returns>The updated comment.</returns>
    Task<TodoTaskComment> UpdateCommentAsync(int todoListId, int taskId, int commentId, string text);

    /// <summary>Deletes a comment via the Web API.</summary>
    /// <param name="todoListId">The list ID.</param>
    /// <param name="taskId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    Task DeleteCommentAsync(int todoListId, int taskId, int commentId);
}
