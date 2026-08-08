using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the model used to communicate with the Web API for to-do task data.
/// </summary>
public class TodoTaskWebApiModel
{
    /// <summary>Gets or sets the unique identifier of the task.</summary>
    public int? Id { get; set; }

    /// <summary>Gets or sets the title of the task.</summary>
    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    /// <summary>Gets or sets the description of the task.</summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the date when the task was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the task due date.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Gets or sets the task status.</summary>
    public int Status { get; set; }

    /// <summary>Gets or sets the user identifier the task is assigned to.</summary>
    public string? AssignedTo { get; set; }

    /// <summary>Gets or sets the identifier of the parent to-do list.</summary>
    public int TodoListId { get; set; }

    /// <summary>Gets or sets the list of the task's tags.</summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>Gets or sets the list of the task's comments.</summary>
    public List<TodoTaskComment> Comments { get; set; } = new List<TodoTaskComment>();
}
