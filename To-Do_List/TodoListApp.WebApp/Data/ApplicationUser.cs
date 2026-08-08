using Microsoft.AspNetCore.Identity;

namespace TodoListApp.WebApp.Data;

/// <summary>
/// Represents an application user, extending the default Identity user.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>Gets or sets the display name of the user.</summary>
    public string? DisplayName { get; set; }
}
