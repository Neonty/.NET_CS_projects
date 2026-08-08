namespace TodoListApp.Data;

/// <summary>
/// Represents the to-do list database entity.
/// </summary>
public class TodoListEntity
{
    /// <summary>Gets or sets the unique identifier of the to-do list.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the title of the to-do list.</summary>
    public string? Title { get; set; }

    /// <summary>Gets or sets the description of the to-do list.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the collection of tasks in this to-do list.</summary>
    public ICollection<TodoTaskEntity> Tasks { get; set; } = new List<TodoTaskEntity>();

    /// <summary>Gets or sets the owner of the to-do list.</summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>Gets or sets the collection of access entitys in this to-do list.</summary>
    public ICollection<TodoListAccessEntity> SharedAccess { get; set; } = new List<TodoListAccessEntity>();
}
