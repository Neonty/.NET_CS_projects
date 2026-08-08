namespace TodoListApp.Data;

/// <summary>
/// Represents a user's shared access to a specific to-do list.
/// </summary>
public class TodoListAccessEntity
{
    /// <summary>Gets or sets the unique identifier of the access record.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the identifier of the to-do list.</summary>
    public int TodoListId { get; set; }

    /// <summary>Gets or sets the identifier of the user the list is shared with.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Gets or sets the role granted to the user.</summary>
    public UserRole Role { get; set; }

    /// <summary>Gets or sets the navigation property for the to-do list.</summary>
    public TodoListEntity TodoList { get; set; } = null!;
}
