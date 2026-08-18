using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Communicates with the TodoListApp.WebApi to manage to-do tasks over HTTP.
/// </summary>
public class TodoTaskWebApiService : ITodoTaskWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<TodoTaskWebApiService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The preconfigured HTTP client.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoTaskWebApiService(HttpClient httpClient, ILogger<TodoTaskWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoTask>> GetTasksByTodoListIdAsync(int todoListId)
    {
        this.logger.LogInformation("Sending GET request to retrieve tasks for todo list ID {TodoListId}.", todoListId);

        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<TodoTaskWebApiModel>>($"api/todolists/{todoListId}/tasks");

        if (models is null)
        {
            return Enumerable.Empty<TodoTask>();
        }

        return models.Select(MapToDomain) ?? Enumerable.Empty<TodoTask>();
    }

    /// <inheritdoc/>
    public async Task<TodoTask?> GetTaskByIdAsync(int todoListId, int taskId)
    {
        this.logger.LogInformation("Sending GET request to retrieve task ID {TaskId} in todo list {TodoListId}.", taskId, todoListId);

        var responce = await this.httpClient.GetAsync(new Uri($"api/todolists/{todoListId}/tasks/{taskId}", UriKind.Relative));

        if (responce.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        responce.EnsureSuccessStatusCode();

        var model = await responce.Content.ReadFromJsonAsync<TodoTaskWebApiModel>();

        return model is null ? null : MapToDomain(model);
    }

    /// <inheritdoc/>
    public async Task<TodoTask> CreateTaskAsync(int todoListId, TodoTask task, string userId)
    {
        ArgumentNullException.ThrowIfNull(task);
        this.logger.LogInformation("Sending POST request to create task in todo list {TodoListId}.", todoListId);

        var model = new TodoTaskWebApiModel
        {
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = (int)task.Status,
            AssignedTo = task.AssignedTo,
            Tags = task.Tags,
        };

        var response = await this.httpClient.PostAsJsonAsync($"api/todolists/{todoListId}/tasks", model);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException("You don't have permission to create tasks in this list.");
        }

        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<TodoTaskWebApiModel>();

        task.Id = created!.Id ?? 0;
        task.CreatedAt = created.CreatedAt;
        task.TodoListId = created.TodoListId;
        task.AssignedTo = created.AssignedTo;
        return task;
    }

    /// <inheritdoc/>
    public async Task<TodoTask> UpdateTaskAsync(int todoListId, TodoTask task, string userId)
    {
        ArgumentNullException.ThrowIfNull(task);
        this.logger.LogInformation("Sending PUT request to update task ID {TaskId} in todo list {TodoListId}.", task.Id, todoListId);

        var model = new TodoTaskWebApiModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = (int)task.Status,
            AssignedTo = task.AssignedTo,
            TodoListId = task.TodoListId,
            Tags = task.Tags,
        };

        var response = await this.httpClient.PutAsJsonAsync($"api/todolists/{todoListId}/tasks/{task.Id}", model);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            this.logger.LogError("API returned {StatusCode}: {Content}", response.StatusCode, errorContent);

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException("You don't have permission to modify this task.");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException($"API internal server error: {errorContent}");
            }

            response.EnsureSuccessStatusCode();
        }

        var updated = await response.Content.ReadFromJsonAsync<TodoTaskWebApiModel>();

        task.CreatedAt = updated!.CreatedAt;
        task.AssignedTo = updated.AssignedTo;
        return task;
    }

    /// <inheritdoc/>
    public async Task DeleteTaskAsync(int todoListId, int taskId, string userId)
    {
        this.logger.LogInformation("Sending DELETE request to remove task ID {TaskId} from todo list {TodoListId}.", taskId, todoListId);
        var response = await this.httpClient.DeleteAsync(new Uri($"api/todolists/{todoListId}/tasks/{taskId}", UriKind.Relative));

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this task.");
        }

        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoTask>> GetAssignedTasksAsync(string userId, string? statusFilter, string? sortBy)
    {
        this.logger.LogInformation("Sending GET request to retrieve tasks assigned to user ID {UserId}.", userId);

        var query = $"api/assigned-tasks/{userId}";

        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(statusFilter))
        {
            queryParams.Add($"status={statusFilter}");
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            queryParams.Add($"sortBy={sortBy}");
        }

        if (queryParams.Count > 0)
        {
            query += "?" + string.Join("&", queryParams);
        }

        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<TodoTaskWebApiModel>>(query);

        return models?.Select(MapToDomain) ?? Enumerable.Empty<TodoTask>();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoTask>> SearchTasksAsync(string? title, DateTime? dateFrom, DateTime? dateTo, string? status, string? sortBy)
    {
        this.logger.LogInformation("Sending GET request to search tasks.");
        var query = $"api/search/tasks";
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(title))
        {
            queryParams.Add($"title={Uri.EscapeDataString(title)}");
        }

        if (dateFrom.HasValue)
        {
            queryParams.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");
        }

        if (dateTo.HasValue)
        {
            queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");
        }

        if (!string.IsNullOrEmpty(status))
        {
            queryParams.Add($"status={Uri.EscapeDataString(status)}");
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
        }

        if (queryParams.Count > 0)
        {
            query += "?" + string.Join("&", queryParams);
        }

        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<TodoTaskWebApiModel>>(query);

        return models?.Select(MapToDomain) ?? Enumerable.Empty<TodoTask>();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetAllTagsAsync(string userId)
    {
        this.logger.LogInformation("Sending GET request to retrieve tags for user {UserId}.", userId);
        var tags = await this.httpClient.GetFromJsonAsync<IEnumerable<string>>($"api/tags");
        return tags ?? Enumerable.Empty<string>();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoTask>> GetTasksByTagAsync(string tagName, string userId)
    {
        this.logger.LogInformation("Sending GET request to retrieve tasks for tag {TagName} for user {UserId}.", tagName, userId);

        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<TodoTaskWebApiModel>>($"api/tags/{Uri.EscapeDataString(tagName)}/tasks");
        return models?.Select(MapToDomain) ?? Enumerable.Empty<TodoTask>();
    }

    /// <inheritdoc/>
    public async Task<TodoTaskComment> AddCommentAsync(int todoListId, int taskId, TodoTaskComment comment)
    {
        this.logger.LogInformation("Sending POST request to add comment to task ID {TaskId} in list {TodoListId}.", taskId, todoListId);
        var response = await this.httpClient.PostAsJsonAsync($"api/todolists/{todoListId}/tasks/{taskId}/comments", comment);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TodoTaskComment>();
        return created!;
    }

    /// <inheritdoc/>
    public async Task<TodoTaskComment> UpdateCommentAsync(int todoListId, int taskId, int commentId, string text)
    {
        this.logger.LogInformation("Sending PUT request to update comment ID {CommentId} for task ID {TaskId}.", commentId, taskId);
        var model = new TodoTaskComment { Text = text };
        var response = await this.httpClient.PutAsJsonAsync($"api/todolists/{todoListId}/tasks/{taskId}/comments/{commentId}", model);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<TodoTaskComment>();
        return updated!;
    }

    /// <inheritdoc/>
    public async Task DeleteCommentAsync(int todoListId, int taskId, int commentId)
    {
        this.logger.LogInformation("Sending DELETE request to delete comment ID {CommentId} from task ID {TaskId}.", commentId, taskId);
        var response = await this.httpClient.DeleteAsync(new Uri($"api/todolists/{todoListId}/tasks/{taskId}/comments/{commentId}", UriKind.Relative));
        response.EnsureSuccessStatusCode();
    }

    private static TodoTask MapToDomain(TodoTaskWebApiModel m)
    {
        return new TodoTask
        {
            Id = m.Id ?? 0,
            Title = m.Title,
            Description = m.Description,
            CreatedAt = m.CreatedAt,
            DueDate = m.DueDate,
            Status = (TodoTaskStatus)m.Status,
            AssignedTo = m.AssignedTo,
            TodoListId = m.TodoListId,
            Tags = m.Tags ?? new List<string>(),
            Comments = m.Comments ?? new List<TodoTaskComment>(),
        };
    }
}
