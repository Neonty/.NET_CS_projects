namespace TodoListApp.WebApi.Models;

/// <summary>
/// Represents a comment on a to-do task data object.
/// </summary>
public class TodoTaskComment
{
    /// <summary>Gets or sets the comment identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the comment text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the date and time the comment was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the creator of the comment.</summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>Gets or sets the associated to-do task identifier.</summary>
    public int TodoTaskId { get; set; }
}
