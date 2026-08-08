using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TodoListApp.Data;
using TodoListApp.WebApi.Services;
using Xunit;

namespace TodoListApp.WebApi.Tests.Services;

public class TodoTaskDatabaseServiceTests : IDisposable
{
    private readonly TodoListDbContext context;
    private readonly TodoTaskDatabaseService service;
    private readonly ILogger<TodoTaskDatabaseService> logger;

    public TodoTaskDatabaseServiceTests()
    {
        var options = new DbContextOptionsBuilder<TodoListDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new TodoListDbContext(options);
        logger = new NullLogger<TodoTaskDatabaseService>();
        service = new TodoTaskDatabaseService(context, logger);
    }

    public void Dispose()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Fact]
    public async Task CreateTaskAsync_ViewerRole_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var ownerId = "owner-guid";
        var viewerId = "viewer-guid";
        var todoListId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Test List"
        };

        var viewerAccess = new TodoListAccessEntity
        {
            TodoListId = todoListId,
            UserId = viewerId,
            Role = UserRole.Viewer
        };

        context.TodoLists.Add(list);
        context.TodoListAccess.Add(viewerAccess);
        context.SaveChanges();

        var task = new WebApi.Models.TodoTask
        {
            Title = "Test Task",
            TodoListId = todoListId
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CreateTaskAsync(task, viewerId));
    }

    [Fact]
    public async Task CreateTaskAsync_EditorRole_Succeeds()
    {
        // Arrange
        var ownerId = "owner-guid";
        var editorId = "editor-guid";
        var todoListId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Test List"
        };

        var editorAccess = new TodoListAccessEntity
        {
            TodoListId = todoListId,
            UserId = editorId,
            Role = UserRole.Editor
        };

        context.TodoLists.Add(list);
        context.TodoListAccess.Add(editorAccess);
        context.SaveChanges();

        var task = new WebApi.Models.TodoTask
        {
            Title = "Test Task",
            TodoListId = todoListId
        };

        // Act
        var result = await service.CreateTaskAsync(task, editorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
    }

    [Fact]
    public async Task CreateTaskAsync_OwnerRole_Succeeds()
    {
        // Arrange
        var ownerId = "owner-guid";
        var todoListId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Test List"
        };

        context.TodoLists.Add(list);
        context.SaveChanges();

        var task = new WebApi.Models.TodoTask
        {
            Title = "Test Task",
            TodoListId = todoListId
        };

        // Act
        var result = await service.CreateTaskAsync(task, ownerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_ViewerRole_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var ownerId = "owner-guid";
        var viewerId = "viewer-guid";
        var todoListId = 1;
        var taskId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Test List"
        };

        var task = new TodoTaskEntity
        {
            Id = taskId,
            TodoListId = todoListId,
            Title = "Original Task"
        };

        var viewerAccess = new TodoListAccessEntity
        {
            TodoListId = todoListId,
            UserId = viewerId,
            Role = UserRole.Viewer
        };

        context.TodoLists.Add(list);
        context.TodoTasks.Add(task);
        context.TodoListAccess.Add(viewerAccess);
        context.SaveChanges();

        var updateTask = new WebApi.Models.TodoTask
        {
            Id = taskId,
            Title = "Updated Task",
            TodoListId = todoListId
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UpdateTaskAsync(updateTask, viewerId));
    }

    [Fact]
    public async Task DeleteTaskAsync_ViewerRole_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var ownerId = "owner-guid";
        var viewerId = "viewer-guid";
        var todoListId = 1;
        var taskId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Test List"
        };

        var task = new TodoTaskEntity
        {
            Id = taskId,
            TodoListId = todoListId,
            Title = "Test Task"
        };

        var viewerAccess = new TodoListAccessEntity
        {
            TodoListId = todoListId,
            UserId = viewerId,
            Role = UserRole.Viewer
        };

        context.TodoLists.Add(list);
        context.TodoTasks.Add(task);
        context.TodoListAccess.Add(viewerAccess);
        context.SaveChanges();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.DeleteTaskAsync(taskId, viewerId));
    }

    [Fact]
    public async Task UpdateTaskAsync_EditorRole_Succeeds()
    {
        // Arrange
        var ownerId = "owner-guid";
        var editorId = "editor-guid";
        var todoListId = 1;
        var taskId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Test List"
        };

        var task = new TodoTaskEntity
        {
            Id = taskId,
            TodoListId = todoListId,
            Title = "Original Task"
        };

        var editorAccess = new TodoListAccessEntity
        {
            TodoListId = todoListId,
            UserId = editorId,
            Role = UserRole.Editor
        };

        context.TodoLists.Add(list);
        context.TodoTasks.Add(task);
        context.TodoListAccess.Add(editorAccess);
        context.SaveChanges();

        var updateTask = new WebApi.Models.TodoTask
        {
            Id = taskId,
            Title = "Updated Task",
            TodoListId = todoListId
        };

        // Act
        var result = await service.UpdateTaskAsync(updateTask, editorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Task", result.Title);
    }

    [Fact]
    public async Task DeleteTaskAsync_EditorRole_Succeeds()
    {
        // Arrange
        var ownerId = "owner-guid";
        var editorId = "editor-guid";
        var todoListId = 1;
        var taskId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Test List"
        };

        var task = new TodoTaskEntity
        {
            Id = taskId,
            TodoListId = todoListId,
            Title = "Test Task"
        };

        var editorAccess = new TodoListAccessEntity
        {
            TodoListId = todoListId,
            UserId = editorId,
            Role = UserRole.Editor
        };

        context.TodoLists.Add(list);
        context.TodoTasks.Add(task);
        context.TodoListAccess.Add(editorAccess);
        context.SaveChanges();

        // Act
        var result = await service.DeleteTaskAsync(taskId, editorId);

        // Assert
        Assert.True(result);
    }
}
