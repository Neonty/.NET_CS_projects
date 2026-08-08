using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Defines the contract for communicating with the TodoListApp.WebApi application.
/// </summary>
public interface ITodoListWebApiService
{
    /// <summary>Returns all to-do lists from the Web API.</summary>
    /// <returns>A collection of <see cref="TodoList"/> objects.</returns>
    Task<IEnumerable<TodoListWebApiModel>> GetTodoListsAsync(string userId);

    /// <summary>Returns specific to-do list from the Web API.</summary>
    /// <returns><see cref="TodoListWebApiModel"/></returns>
    Task<TodoListWebApiModel?> GetTodoListByIdAsync(int id, string userId);

    /// <summary>Creates a new to-do list via the Web API.</summary>
    /// <param name="todoList">The to-do list to create.</param>
    /// <returns>The created <see cref="TodoListWebApiModel"/>.</returns>
    Task<TodoListWebApiModel> CreateTodoListAsync(TodoListWebApiModel todoList, string userId);

    /// <summary>Updates an existing to-do list via the Web API.</summary>
    /// <param name="todoList">The to-do list with updated values.</param>
    /// <returns>The updated <see cref="TodoListWebApiModel"/>.</returns>
    Task<TodoListWebApiModel?> UpdateTodoListAsync(TodoListWebApiModel todoList, string userId);

    /// <summary>Deletes a to-do list by its identifier via the Web API.</summary>
    /// <param name="id">The identifier of the to-do list to delete.</param>
    /// <returns>True if deleted, otherwise false</returns>
    Task<bool> DeleteTodoListAsync(int id, string userId);

    /// <summary>Adds a share  the Web API.</summary>
    /// <param name="todoListId">The identifier of the to-do list to add.</param>
    /// <returns>True if added, otherwise false.</returns>
    Task<bool> AddShareAsync(int todoListId, string ownerId, string targetUserId, TodoListApp.Data.UserRole role);

    /// <summary>Removes a share  the Web API.</summary>
    /// <param name="todoListId">The identifier of the to-do list to remove from.</param>
    /// <returns>True if removed, otherwise false.</returns>
    Task<bool> RemoveShareAsync(int todoListId, string ownerId, string targetUserId);
}
