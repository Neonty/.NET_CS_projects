using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// API Controller for managing sharing of to-do lists.
/// </summary>
[ApiController]
[Route("api/todolists/{todoListId:int}/share")]
public class TodoListShareController : ControllerBase
{
    private readonly TodoListDbContext _context;
    private readonly ILogger<TodoListShareController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListShareController"/> class.
    /// </summary>
    public TodoListShareController(TodoListDbContext context, ILogger<TodoListShareController> logger)
    {
        _context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Returns the list of users the to-do list is shared with.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetShares(int todoListId, [FromQuery] string ownerId)
    {
        this.logger.LogInformation("Fetching shares for list {TodoListId}", todoListId);

        var list = await _context.TodoLists.FindAsync(todoListId);
        if (list == null)
        {
            return NotFound();
        }

        if (list.OwnerId != ownerId)
        {
            return Forbid();
        }

        var shares = await _context.TodoListAccess
            .Where(a => a.TodoListId == todoListId)
            .Select(a => new ShareResponse { UserId = a.UserId, Role = a.Role })
            .ToListAsync();

        return Ok(shares);
    }

    /// <summary>
    /// Adds or updates a user's shared access to a to-do list.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddShare(int todoListId, [FromQuery] string ownerId, [FromBody] ShareRequest request)
    {
        this.logger.LogInformation($"Updating the {request.UserId} user's access level to list {todoListId}");

        var list = await _context.TodoLists.FindAsync(todoListId);
        if (list == null)
        {
            return NotFound();
        }

        if (list.OwnerId != ownerId)
        {
            return Forbid();
        }

        var access = await _context.TodoListAccess.FirstOrDefaultAsync(a => a.TodoListId == todoListId && a.UserId == request.UserId);
        if (access != null)
        {
            access.Role = request.Role;
        }
        else
        {
            _context.TodoListAccess.Add(new TodoListAccessEntity { TodoListId = todoListId, UserId = request.UserId, Role = request.Role });
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Removes a user's shared access from a to-do list.
    /// </summary>
    [HttpDelete("{targetUserId}")]
    public async Task<IActionResult> RemoveShare(int todoListId, string targetUserId, [FromQuery] string ownerId)
    {
        this.logger.LogInformation($"Removing the {targetUserId} user's access level from list {todoListId}");

        var list = await _context.TodoLists.FindAsync(todoListId);
        if (list == null)
        {
            return NotFound();
        }

        if (list.OwnerId != ownerId)
        {
            return Forbid();
        }

        var access = await _context.TodoListAccess.FirstOrDefaultAsync(a => a.TodoListId == todoListId && a.UserId == targetUserId);
        if (access != null)
        {
            _context.TodoListAccess.Remove(access);
            await _context.SaveChangesAsync();
        }

        return Ok();
    }
}
