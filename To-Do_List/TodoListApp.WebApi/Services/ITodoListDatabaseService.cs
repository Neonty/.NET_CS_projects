using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Defines the methods for managing to-do lists in the database.
/// </summary>
public interface ITodoListDatabaseService
{
    /// <summary>Gets all to-do lists for a user.</summary>
    Task<IEnumerable<TodoList>> GetAllTodoListsAsync(string userId);

    /// <summary>Gets a to-do list by its ID.</summary>
    Task<TodoList?> GetTodoListByIdAsync(int id, string userId);

    /// <summary>Creates a new to-do list.</summary>
    Task<TodoList> CreateTodoListAsync(TodoList todoList, string userId);

    /// <summary>Updates an existing to-do list.</summary>
    Task<TodoList?> UpdateTodoListAsync(TodoList todoList, string userId);

    /// <summary>Deletes a to-do list.</summary>
    Task<bool> DeleteTodoListAsync(int id, string userId);
}
