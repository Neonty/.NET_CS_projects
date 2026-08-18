using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using TodoListApp.Data;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Manages to-do tasks in the database using Entity Framework Core.
/// </summary>
public class TodoTaskDatabaseService : ITodoTaskDatabaseService
{
    private readonly TodoListDbContext context;
    private readonly ILogger<TodoTaskDatabaseService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public TodoTaskDatabaseService(TodoListDbContext context, ILogger<TodoTaskDatabaseService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoTask>> GetAllTodoTasksAsync(int todoListId)
    {
        this.logger.LogInformation("Retrieving tasks for todo list ID {TodoListId}.", todoListId);

        return await this.context.TodoTasks
            .Include(t => t.Tags).Include(t => t.Comments)
            .Where(t => t.TodoListId == todoListId)
            .Select(t => MapToDomain(t))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<TodoTask?> GetTaskByIdAsync(int id)
    {
        this.logger.LogInformation("Retrieving task with ID {TaskId}.", id);

        var entity = await this.context.TodoTasks
            .Include(t => t.Tags).Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (entity is null)
        {
            return null;
        }

        return MapToDomain(entity);
    }

    /// <inheritdoc/>
    public async Task<TodoTask> CreateTaskAsync(TodoTask task, string userId)
    {
        ArgumentNullException.ThrowIfNull(task);

        var list = await this.context.TodoLists.FindAsync(task.TodoListId);
        if (list == null)
        {
            throw new KeyNotFoundException($"Todo list with ID {task.TodoListId} not found.");
        }

        bool isOwner = list.OwnerId == userId;
        bool isEditor = await this.context.TodoListAccess.AnyAsync(a => a.TodoListId == task.TodoListId && a.UserId == userId && a.Role == UserRole.Editor);

        if (!isOwner && !isEditor)
        {
            throw new UnauthorizedAccessException("Only owners and editors can create tasks.");
        }

        var entity = new TodoTaskEntity
        {
            Title = task.Title ?? string.Empty,
            Description = task.Description,
            CreatedAt = DateTime.UtcNow,
            DueDate = task.DueDate,
            Status = (Data.TodoTaskStatus)task.Status,
            AssignedTo = task.AssignedTo,
            TodoListId = task.TodoListId,
        };

        await this.SyncTagsAsync(entity, task.Tags);

        this.context.TodoTasks.Add(entity);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Created a new task with ID {Id} in todo list {TodoListId}.", entity.Id, entity.TodoListId);

        task.Id = entity.Id;
        task.CreatedAt = entity.CreatedAt;
        return task;
    }

    /// <inheritdoc/>
    public async Task<TodoTask?> UpdateTaskAsync(TodoTask task, string userId)
    {
        ArgumentNullException.ThrowIfNull(task);

        var entity = await this.context.TodoTasks
            .Include(t => t.Tags).Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        if (entity is null)
        {
            throw new KeyNotFoundException($"Task with ID {task.Id} not found.");
        }

        var list = await this.context.TodoLists.FindAsync(entity.TodoListId);
        if (list == null)
        {
            throw new KeyNotFoundException($"Todo list with ID {entity.TodoListId} not found.");
        }

        bool isOwner = list.OwnerId == userId;
        bool isEditor = await this.context.TodoListAccess.AnyAsync(a => a.TodoListId == entity.TodoListId && a.UserId == userId && a.Role == UserRole.Editor);

        if (!isOwner && !isEditor)
        {
            throw new UnauthorizedAccessException("Only owners and editors can update tasks.");
        }

        entity.Title = task.Title ?? string.Empty;
        entity.Description = task.Description;
        entity.DueDate = task.DueDate;
        entity.Status = (Data.TodoTaskStatus)task.Status;
        entity.AssignedTo = task.AssignedTo;

        await this.SyncTagsAsync(entity, task.Tags);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Updated task with ID {Id} in todo list {TodoListId}.", entity.Id, entity.TodoListId);

        return task;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTaskAsync(int id, string userId)
    {
        var entity = await this.context.TodoTasks.FindAsync(id);

        if (entity is null)
        {
            return false;
        }

        var list = await this.context.TodoLists.FindAsync(entity.TodoListId);
        if (list == null)
        {
            throw new KeyNotFoundException($"Todo list with ID {entity.TodoListId} not found.");
        }

        bool isOwner = list.OwnerId == userId;
        bool isEditor = await this.context.TodoListAccess.AnyAsync(a => a.TodoListId == entity.TodoListId && a.UserId == userId && a.Role == UserRole.Editor);

        if (!isOwner && !isEditor)
        {
            throw new UnauthorizedAccessException("Only owners and editors can delete tasks.");
        }

        this.context.TodoTasks.Remove(entity);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Deleted task with ID {Id} from todo list {TodoListId}.", entity.Id, entity.TodoListId);

        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoTask>> GetAssignedTasksAsync(string userId, string? statusFilter, string? sortBy)
    {
        this.logger.LogInformation("Retrieving tasks assigned to user ID {UserId}.", userId);

        var query = this.context.TodoTasks.Include(t => t.Tags).Include(t => t.Comments).Where(t => t.AssignedTo == userId);

        if (!string.IsNullOrEmpty(statusFilter))
        {
            if (statusFilter.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.Status == Data.TodoTaskStatus.NotStarted || t.Status == Data.TodoTaskStatus.InProgress);
            }
            else if (statusFilter.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.Status == Data.TodoTaskStatus.Completed);
            }
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(t => t.Title);
            }
            else if (sortBy.Equals("DueDate", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(t => t.DueDate);
            }
        }

        var list = await query.ToListAsync();
        return list.Select(MapToDomain);
    }

    public async Task<IEnumerable<TodoTask>> SearchTasksAsync(string userId, string? title, DateTime? dateFrom, DateTime? dateTo, Models.TodoTaskStatus? status, string? sortBy)
    {
        var accessibleListIds = await this.GetAccessibleListIdsAsync(userId);
        var query = this.context.TodoTasks
            .Include(t => t.Tags).Include(t => t.Comments)
            .Where(t => accessibleListIds.Contains(t.TodoListId))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(t => t.Title.Contains(title));
        }

        if (dateFrom.HasValue && dateTo.HasValue)
        {
            var endDate = dateTo.Value.Date.AddDays(1);
            query = query.Where(t =>
                (t.CreatedAt >= dateFrom.Value.Date && t.CreatedAt < endDate) ||
                (t.DueDate >= dateFrom.Value.Date && t.DueDate < endDate)
            );
        }
        else if (dateFrom.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= dateFrom.Value.Date || t.DueDate >= dateFrom.Value.Date);
        }
        else if (dateTo.HasValue)
        {
            var endDate = dateTo.Value.Date.AddDays(1);
            query = query.Where(t => t.CreatedAt < endDate || t.DueDate < endDate);
        }

        if (status.HasValue)
        {
            var dbStatus = (Data.TodoTaskStatus)status.Value;
            query = query.Where(t => t.Status == dbStatus);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.ToLowerInvariant())
            {
                case "title":
                    query = query.OrderBy(t => t.Title);
                    break;
                case "titledesc":
                    query = query.OrderByDescending(t => t.Title);
                    break;
                case "duedate":
                    query = query.OrderBy(t => t.DueDate);
                    break;
                case "duedatedesc":
                    query = query.OrderByDescending(t => t.DueDate);
                    break;
                case "createdatasc":
                    query = query.OrderBy(t => t.CreatedAt);
                    break;
                case "status":
                    query = query.OrderBy(t => t.Status);
                    break;
                case "statusdesc":
                    query = query.OrderByDescending(t => t.Status);
                    break;
                case "createdat":
                default:
                    query = query.OrderByDescending(t => t.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(t => t.CreatedAt);
        }

        var list = await query.ToListAsync();
        return list.Select(MapToDomain);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetAllTagsAsync(string userId)
    {
        this.logger.LogInformation("Retrieving tags accessible to user {UserId}.", userId);

        var accessibleListIds = await this.GetAccessibleListIdsAsync(userId);

        return await this.context.Tags
            .Where(t => t.Tasks.Any(task => accessibleListIds.Contains(task.TodoListId)))
            .OrderBy(t => t.Name)
            .Select(t => t.Name)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodoTask>> GetTasksByTagAsync(string tagName, string userId)
    {
        this.logger.LogInformation("Retrieving tasks for tag {TagName} accessible to user {UserId}.", tagName, userId);

        var accessibleListIds = await this.GetAccessibleListIdsAsync(userId);

        var tasks = await this.context.TodoTasks
            .Include(t => t.Tags).Include(t => t.Comments)
            .Where(t => t.Tags.Any(tag => tag.Name == tagName) && accessibleListIds.Contains(t.TodoListId))
            .ToListAsync();

        return tasks.Select(MapToDomain);
    }

    /// <summary>
    /// Returns the IDs of the to-do lists the given user can access, either as owner or as a shared user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A set of accessible to-do list IDs.</returns>
    private async Task<HashSet<int>> GetAccessibleListIdsAsync(string userId)
    {
        var ownedIds = await this.context.TodoLists
            .Where(l => l.OwnerId == userId)
            .Select(l => l.Id)
            .ToListAsync();

        var sharedIds = await this.context.TodoListAccess
            .Where(a => a.UserId == userId)
            .Select(a => a.TodoListId)
            .ToListAsync();

        return ownedIds.Concat(sharedIds).ToHashSet();
    }

    /// <summary>
    /// Synchronizes the tags for a task entity by looking up existing tags or creating new ones.
    /// </summary>
    /// <param name="entity">The task entity to update.</param>
    /// <param name="tags">The list of tag names to synchronize.</param>
    private async Task SyncTagsAsync(TodoTaskEntity entity, List<string> tags)
    {
        var uniqueTags = tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var tagsToRemove = entity.Tags
            .Where(t => !uniqueTags.Contains(t.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        foreach (var t in tagsToRemove)
        {
            entity.Tags.Remove(t);
        }

        var existingLinkedNames = entity.Tags.Select(t => t.Name).ToList();
        var tagsToAdd = uniqueTags
            .Where(name => !existingLinkedNames.Contains(name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        foreach (var tagName in tagsToAdd)
        {
            var existingDbTag = await this.context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (existingDbTag != null)
            {
                entity.Tags.Add(existingDbTag);
            }
            else
            {
                entity.Tags.Add(new TagEntity { Name = tagName });
            }
        }
    }

    /// <inheritdoc/>
    public async Task<TodoTaskComment> AddCommentAsync(TodoTaskComment comment)
    {
        this.logger.LogInformation("Adding a new comment to task ID {TaskId}.", comment.TodoTaskId);
        var entity = new TodoTaskCommentEntity
        {
            Text = comment.Text,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = comment.CreatedBy,
            TodoTaskId = comment.TodoTaskId
        };

        this.context.Comments.Add(entity);
        await this.context.SaveChangesAsync();

        comment.Id = entity.Id;
        comment.CreatedAt = entity.CreatedAt;
        return comment;
    }

    /// <inheritdoc/>
    public async Task<TodoTaskComment?> UpdateCommentAsync(int commentId, string text)
    {
        this.logger.LogInformation("Updating comment ID {CommentId}.", commentId);
        var entity = await this.context.Comments.FindAsync(commentId);
        if (entity == null)
        {
            return null;
        }

        entity.Text = text;
        await this.context.SaveChangesAsync();
        return new TodoTaskComment { Id = entity.Id, Text = entity.Text, CreatedAt = entity.CreatedAt, CreatedBy = entity.CreatedBy, TodoTaskId = entity.TodoTaskId };
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        this.logger.LogInformation("Deleting comment ID {CommentId}.", commentId);
        var entity = await this.context.Comments.FindAsync(commentId);
        if (entity == null)
        {
            return false;
        }

        this.context.Comments.Remove(entity);
        await this.context.SaveChangesAsync();
        return true;
    }

    private static TodoTask MapToDomain(TodoTaskEntity entity)
    {
        return new TodoTask
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            DueDate = entity.DueDate,
            Status = (Models.TodoTaskStatus)entity.Status,
            AssignedTo = entity.AssignedTo,
            TodoListId = entity.TodoListId,
            Tags = entity.Tags.Select(t => t.Name).ToList(),
            Comments = entity.Comments.Select(c => new TodoTaskComment
            {
                Id = c.Id,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                TodoTaskId = c.TodoTaskId,
            }).OrderByDescending(c => c.CreatedAt).ToList(),
        };
    }
}
