namespace TodoListApp.Data;

/// <summary>
/// Represents a comment on a to-do task in the database.
/// </summary>
public class TodoTaskCommentEntity
{
    /// <summary>Gets or sets the unique identifier of the comment.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the comment text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the date and time when the comment was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the username or identifier of the user who created the comment.</summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>Gets or sets the associated to-do task identifier.</summary>
    public int TodoTaskId { get; set; }

    /// <summary>Gets or sets the associated to-do task entity.</summary>
    public TodoTaskEntity TodoTask { get; set; } = null!;
}
