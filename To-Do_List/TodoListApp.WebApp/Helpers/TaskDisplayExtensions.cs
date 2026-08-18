using System.Globalization;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Helpers;

public static class TaskDisplayExtensions
{
    private static readonly CultureInfo DisplayCulture = CultureInfo.GetCultureInfo("en-US");

    public static string FormatDueDate(this DateTime? dueDate)
    {
        if (!dueDate.HasValue)
        {
            return string.Empty;
        }

        return dueDate.Value.ToLocalTime().ToString("MMM dd, yyyy", DisplayCulture);
    }

    public static string FormatShortDate(this DateTime date)
    {
        return date.ToLocalTime().ToString("MMM dd", DisplayCulture);
    }

    public static string GetStatusLabel(this TodoTaskStatus status) => status switch
    {
        TodoTaskStatus.NotStarted => "Not Started",
        TodoTaskStatus.InProgress => "In Progress",
        TodoTaskStatus.Completed => "Completed",
        _ => status.ToString(),
    };

    public static string GetStatusCssClass(this TodoTaskStatus status) => status switch
    {
        TodoTaskStatus.NotStarted => "status-notstarted",
        TodoTaskStatus.InProgress => "status-inprogress",
        TodoTaskStatus.Completed => "status-completed",
        _ => "status-notstarted",
    };
}
