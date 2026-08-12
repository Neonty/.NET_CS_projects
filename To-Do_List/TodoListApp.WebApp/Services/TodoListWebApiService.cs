using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Communicates with the TodoListApp.WebApi to manage to-do lists over HTTP.
/// </summary>
public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<TodoListWebApiService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The preconfigured HTTP client.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoListWebApiService(HttpClient httpClient, ILogger<TodoListWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoListWebApiModel>> GetTodoListsAsync(string userId)
    {
        this.logger.LogInformation("Getting all todo lists from the web API.");

        return await this.httpClient.GetFromJsonAsync<IEnumerable<TodoListWebApiModel>>($"api/todolists") ?? Array.Empty<TodoListWebApiModel>();
    }

    /// <inheritdoc/>
    public async Task<TodoListWebApiModel?> GetTodoListByIdAsync(int id, string userId)
    {
        return await this.httpClient.GetFromJsonAsync<TodoListWebApiModel>($"api/todolists/{id}");
    }

    /// <inheritdoc/>
    public async Task<TodoListWebApiModel> CreateTodoListAsync(TodoListWebApiModel todoList, string userId)
    {
        var response = await this.httpClient.PostAsJsonAsync($"api/todolists", todoList);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoListWebApiModel>() ?? throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task<TodoListWebApiModel?> UpdateTodoListAsync(TodoListWebApiModel todoList, string userId)
    {
        var response = await this.httpClient.PutAsJsonAsync($"api/todolists/{todoList.Id}", todoList);
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoListWebApiModel>();
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTodoListAsync(int id, string userId)
    {
        var response = await this.httpClient.DeleteAsync($"api/todolists/{id}");
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc/>
    public async Task<bool> AddShareAsync(int todoListId, string ownerId, string targetUserId, TodoListApp.Data.UserRole role)
    {
        this.logger.LogInformation("Adding share for list {ListId}: user {TargetUserId} as {Role}.", todoListId, targetUserId, role);
        var payload = new { UserId = targetUserId, Role = role };
        var response = await this.httpClient.PostAsJsonAsync($"api/todolists/{todoListId}/share", payload);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveShareAsync(int todoListId, string ownerId, string targetUserId)
    {
        this.logger.LogInformation("Removing share for list {ListId}: user {TargetUserId}.", todoListId, targetUserId);
        var response = await this.httpClient.DeleteAsync($"api/todolists/{todoListId}/share/{targetUserId}");
        return response.IsSuccessStatusCode;
    }
}
