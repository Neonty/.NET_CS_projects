using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Manages to-do lists in the database using Entity Framework Core.
/// </summary>
public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext context;
    private readonly ILogger<TodoListDatabaseService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoListDatabaseService(TodoListDbContext context, ILogger<TodoListDatabaseService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoList>> GetAllTodoListsAsync(string userId)
    {
        this.logger.LogInformation("Retrieving all todo lists for user {UserId}", userId);

        var owned = await this.context.TodoLists.Where(l => l.OwnerId == userId)
            .Select(e => new TodoList { Id = e.Id, Title = e.Title, Description = e.Description, AccessLevel = UserRole.Owner })
            .ToListAsync();
        var shared = await this.context.TodoListAccess.Include(a => a.TodoList).Where(a => a.UserId == userId)
            .Select(a => new TodoList { Id = a.TodoList.Id, Title = a.TodoList.Title, Description = a.TodoList.Description, AccessLevel = a.Role })
            .ToListAsync();
        return owned.Concat(shared).OrderBy(l => l.Title);
    }


    /// <inheritdoc/>
    public async Task<TodoList?> GetTodoListByIdAsync(int id, string userId)
    {
        var list = await this.context.TodoLists.FindAsync(id);
        if (list == null)
        {
            return null;
        }

        if (list.OwnerId == userId)
        {
            return new TodoList { Id = list.Id, Title = list.Title, Description = list.Description, AccessLevel = UserRole.Owner };
        }

        var access = await this.context.TodoListAccess.FirstOrDefaultAsync(a => a.TodoListId == id && a.UserId == userId);
        if (access != null)
        {
            return new TodoList { Id = list.Id, Title = list.Title, Description = list.Description, AccessLevel = access.Role };
        }

        return new TodoList { Id = list.Id, Title = list.Title, Description = list.Description, AccessLevel = UserRole.NoAccess };
    }

    /// <inheritdoc/>
    public async Task<TodoList> CreateTodoListAsync(TodoList todoList, string userId)
    {
        var entity = new TodoListEntity
        {
            Title = todoList.Title,
            Description = todoList.Description,
            OwnerId = userId
        };

        this.context.TodoLists.Add(entity);
        await this.context.SaveChangesAsync();

        todoList.Id = entity.Id;
        todoList.AccessLevel = UserRole.Owner;
        return todoList;
    }

    /// <inheritdoc/>
    public async Task<TodoList?> UpdateTodoListAsync(TodoList todoList, string userId)
    {
        var entity = await this.context.TodoLists.FindAsync(todoList.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException();
        }

        bool isOwner = entity.OwnerId == userId;
        bool isEditor = await this.context.TodoListAccess.AnyAsync(a => a.TodoListId == todoList.Id && a.UserId == userId && a.Role == UserRole.Editor);

        if (!isOwner && !isEditor)
        {
            throw new UnauthorizedAccessException("Only owners and editors can update lists.");
        }

        entity.Title = todoList.Title;
        entity.Description = todoList.Description;
        await this.context.SaveChangesAsync();
        return todoList;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTodoListAsync(int id, string userId)
    {
        var entity = await this.context.TodoLists.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        if (entity.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Only the owner can delete a to-do list.");
        }

        this.context.TodoLists.Remove(entity);
        await this.context.SaveChangesAsync();
        return true;
    }
}
