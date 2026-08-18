namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents information about a user who has access to a shared to-do list.
/// </summary>
public class SharedUserInfo
{
    /// <summary>Gets or sets the user identifier.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Gets or sets the user email.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's role on the list.</summary>
    public TodoListApp.Data.UserRole Role { get; set; }
}
