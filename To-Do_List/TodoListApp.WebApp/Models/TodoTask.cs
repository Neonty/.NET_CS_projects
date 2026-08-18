namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents a to-do task data object used by the web app service layer.
/// </summary>
public class TodoTask
{
    /// <summary>Gets or sets the unique identifier of the task.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the title of the task.</summary>
    public string? Title { get; set; }

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

    /// <summary>Gets or sets the identifier of the parent to-do list.</summary>
    public int TodoListId { get; set; }

    /// <summary>Gets a value indicating whether the task is overdue.</summary>
    public bool IsOverdue => this.DueDate.HasValue
        && this.DueDate.Value < DateTime.UtcNow
        && this.Status != TodoTaskStatus.Completed;

    /// <summary>Gets or sets list of tag names attached to this task.</summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>Helper property to render and bind comma-separated tags in HTML forms.</summary>
    public string TagsString
    {
        get => string.Join(", ", this.Tags);
        set => this.Tags = string.IsNullOrWhiteSpace(value) ? new List<string>() : value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
    }

    /// <summary>Gets or sets list of comments attached to this task.</summary>
    public List<TodoTaskComment> Comments { get; set; } = new List<TodoTaskComment>();

    /// <summary>Gets or sets assigned email to this task.</summary>
    public string? AssigneeEmail { get; set; }
}
