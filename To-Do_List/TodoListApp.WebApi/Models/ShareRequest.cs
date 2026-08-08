namespace TodoListApp.WebApi.Models;

/// <summary>
/// Request model for sharing a to-do list.
/// </summary>
public class ShareRequest
{
    /// <summary>Gets or sets the user ID to share with.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Gets or sets the role to grant.</summary>
    public TodoListApp.Data.UserRole Role { get; set; }
}
