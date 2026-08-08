namespace TodoListApp.Data;

/// <summary>
/// Represents the status of a to-do task in the data layer.
/// </summary>
public enum TodoTaskStatus
{
    /// <summary>The task has not been started.</summary>
    NotStarted = 0,

    /// <summary>The task is in progress.</summary>
    InProgress = 1,

    /// <summary>The task has been completed.</summary>
    Completed = 2,
}
