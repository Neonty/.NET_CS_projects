using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the model used to communicate with the Web API for to-do list data.
/// </summary>
public class TodoListWebApiModel
{
    /// <summary>Gets or sets the unique identifier of the to-do list.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the title of the to-do list.</summary>
    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    /// <summary>Gets or sets the description of the to-do list.</summary>
    [StringLength(200)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the user's access level to this list.</summary>
    public TodoListApp.Data.UserRole AccessLevel { get; set; } = TodoListApp.Data.UserRole.NoAccess;
}
