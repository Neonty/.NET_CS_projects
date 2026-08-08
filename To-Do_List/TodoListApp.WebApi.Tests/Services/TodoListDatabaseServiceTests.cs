using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TodoListApp.Data;
using TodoListApp.WebApi.Services;
using Xunit;

namespace TodoListApp.WebApi.Tests.Services;

public class TodoListDatabaseServiceTests : IDisposable
{
    private readonly TodoListDbContext context;
    private readonly TodoListDatabaseService service;
    private readonly ILogger<TodoListDatabaseService> logger;

    public TodoListDatabaseServiceTests()
    {
        var options = new DbContextOptionsBuilder<TodoListDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new TodoListDbContext(options);
        logger = new NullLogger<TodoListDatabaseService>();
        service = new TodoListDatabaseService(context, logger);
    }

    public void Dispose()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Fact]
    public async Task GetAllTodoListsAsync_ReturnsOwnedAndSharedLists()
    {
        // Arrange
        var userId = "user-guid";
        var todoListId = 1;

        var lists = new List<TodoListEntity>
        {
            new TodoListEntity { Id = 1, OwnerId = userId, Title = "My List" },
            new TodoListEntity { Id = 2, OwnerId = "other-guid", Title = "Shared List" }
        };

        var access = new List<TodoListAccessEntity>
        {
            new TodoListAccessEntity { TodoListId = 2, UserId = userId, Role = UserRole.Editor }
        };

        context.TodoLists.AddRange(lists);
        context.TodoListAccess.AddRange(access);
        context.SaveChanges();

        // Act
        var result = await service.GetAllTodoListsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateTodoListAsync_ViewerRole_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var ownerId = "owner-guid";
        var viewerId = "viewer-guid";
        var todoListId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Original Title"
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

        var updateList = new WebApi.Models.TodoList
        {
            Id = todoListId,
            Title = "Updated Title"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UpdateTodoListAsync(updateList, viewerId));
    }

    [Fact]
    public async Task DeleteTodoListAsync_ViewerRole_ThrowsUnauthorizedAccessException()
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

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.DeleteTodoListAsync(todoListId, viewerId));
    }

    [Fact]
    public async Task UpdateTodoListAsync_EditorRole_Succeeds()
    {
        // Arrange
        var ownerId = "owner-guid";
        var editorId = "editor-guid";
        var todoListId = 1;

        var list = new TodoListEntity
        {
            Id = todoListId,
            OwnerId = ownerId,
            Title = "Original Title"
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

        var updateList = new WebApi.Models.TodoList
        {
            Id = todoListId,
            Title = "Updated Title"
        };

        // Act
        var result = await service.UpdateTodoListAsync(updateList, editorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
    }
}
