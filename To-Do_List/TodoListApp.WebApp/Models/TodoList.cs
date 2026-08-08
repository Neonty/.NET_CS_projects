namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents a to-do list data object used by the web app service layer.
/// </summary>
public class TodoList
{
    /// <summary>Gets or sets the unique identifier of the to-do list.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the title of the to-do list.</summary>
    public string? Title { get; set; }

    /// <summary>Gets or sets the description of the to-do list.</summary>
    public string? Description { get; set; }
}
