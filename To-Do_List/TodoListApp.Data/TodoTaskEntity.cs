namespace TodoListApp.Data;

/// <summary>
/// Represents the to-do task database entity.
/// </summary>
public class TodoTaskEntity
{
    /// <summary>Gets or sets the unique identifier of the task.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the title of the task.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the description of the task.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the date when the task was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the task due date.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Gets or sets the task status.</summary>
    public TodoTaskStatus Status { get; set; }

    /// <summary>Gets or sets the user identifier the task is assigned to.</summary>
    public string? AssignedTo { get; set; }

    /// <summary>Gets or sets the foreign key to the parent to-do list.</summary>
    public int TodoListId { get; set; }

    /// <summary>Gets or sets the navigation property to the parent to-do list.</summary>
    public TodoListEntity TodoList { get; set; } = null!;

    /// <summary>Gets or sets the tags associated with this task.</summary>
    public ICollection<TagEntity> Tags { get; set; } = new List<TagEntity>();

    /// <summary>Gets or sets the comments associated with this task.</summary>
    public ICollection<TodoTaskCommentEntity> Comments { get; set; } = new List<TodoTaskCommentEntity>();
}
