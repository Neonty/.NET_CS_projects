namespace TodoListApp.WebApi.Models;

/// <summary>
/// Represents a shared access entry returned by the API.
/// </summary>
public class ShareResponse
{
    /// <summary>Gets or sets the user identifier the list is shared with.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Gets or sets the user email.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the role granted to the user.</summary>
    public TodoListApp.Data.UserRole Role { get; set; }
}
