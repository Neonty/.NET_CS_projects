namespace TodoListApp.Data;

/// <summary>
/// Represents a tag that can be attached to multiple tasks.
/// </summary>
public class TagEntity
{
    /// <summary>Gets or sets the unique identifier of the tag.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the name of the tag.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the collection of tasks with this tag.</summary>
    public ICollection<TodoTaskEntity> Tasks { get; set; } = new List<TodoTaskEntity>();
}
