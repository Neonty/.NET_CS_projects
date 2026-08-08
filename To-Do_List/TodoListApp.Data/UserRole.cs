namespace TodoListApp.Data;

/// <summary>
/// Defines the 4 roles a user can have for a to-do list.
/// </summary>
public enum UserRole
{
    /// <summary>User has no access and cannot view the list.</summary>
    NoAccess,

    /// <summary>Owner of the to-do list (can edit, delete, and share).</summary>
    Owner,

    /// <summary>Editor with full access to modify the list and its tasks.</summary>
    Editor,

    /// <summary>Viewer with read-only access to the list.</summary>
    Viewer
}
