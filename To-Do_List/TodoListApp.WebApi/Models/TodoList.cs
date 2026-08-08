namespace TodoListApp.WebApi.Models;

/// <summary>
/// Represents a to-do list data object used by the service layer.
/// </summary>
public class TodoList
{
    /// <summary>Gets or sets the unique identifier of the list.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the title of the list.</summary>
    public string? Title { get; set; }

    /// <summary>Gets or sets the description of the list.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the user's access level to this list.</summary>
    public TodoListApp.Data.UserRole AccessLevel { get; set; } = TodoListApp.Data.UserRole.NoAccess;
}
