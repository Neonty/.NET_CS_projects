using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// API Controller for managing sharing of to-do lists.
/// </summary>
[Authorize]
[ApiController]
[Route("api/todolists/{todoListId:int}/share")]
public class TodoListShareController : ControllerBase
{
    private readonly TodoListDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<TodoListShareController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListShareController"/> class.
    /// </summary>
    public TodoListShareController(TodoListDbContext context, UserManager<IdentityUser> userManager, ILogger<TodoListShareController> logger)
    {
        _context = context;
        _userManager = userManager;
        this.logger = logger;
    }

    /// <summary>
    /// Returns the list of users the to-do list is shared with.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetShares(int todoListId)
    {
        this.logger.LogInformation("Fetching shares for list {TodoListId}", todoListId);

        var ownerId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ownerId == null)
        {
            return this.Unauthorized();
        }

        var list = await this._context.TodoLists.FindAsync(todoListId);
        if (list == null)
        {
            return this.NotFound();
        }

        // Check if user is owner or has editor access
        var hasAccess = list.OwnerId == ownerId ||
                       await this._context.TodoListAccess.AnyAsync(a => a.TodoListId == todoListId && a.UserId == ownerId && a.Role == TodoListApp.Data.UserRole.Editor);

        if (!hasAccess)
        {
            return this.Forbid();
        }

        var shares = await this._context.TodoListAccess
            .Where(a => a.TodoListId == todoListId)
            .Select(a => new ShareResponse { UserId = a.UserId, Email = a.UserId, Role = a.Role })
            .ToListAsync();

        // Fetch actual emails for users
        foreach (var share in shares)
        {
            var user = await this._userManager.FindByIdAsync(share.UserId);
            if (user != null)
            {
                share.Email = user.Email ?? share.UserId;
            }
        }

        return this.Ok(shares);
    }

    /// <summary>
    /// Adds or updates a user's shared access to a to-do list.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddShare(int todoListId, [FromBody] ShareRequest request)
    {
        this.logger.LogInformation("Updating the {UserId} user's access level to list {TodoListId}", request.UserId, todoListId);

        var ownerId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ownerId == null)
        {
            return this.Unauthorized();
        }

        var list = await this._context.TodoLists.FindAsync(todoListId);
        if (list == null)
        {
            return this.NotFound();
        }

        // Check if user is owner or has editor access
        var hasAccess = list.OwnerId == ownerId ||
                       await this._context.TodoListAccess.AnyAsync(a => a.TodoListId == todoListId && a.UserId == ownerId && a.Role == TodoListApp.Data.UserRole.Editor);

        if (!hasAccess)
        {
            return this.Forbid();
        }

        var access = await this._context.TodoListAccess.FirstOrDefaultAsync(a => a.TodoListId == todoListId && a.UserId == request.UserId);
        if (access != null)
        {
            access.Role = request.Role;
        }
        else
        {
            this._context.TodoListAccess.Add(new TodoListAccessEntity { TodoListId = todoListId, UserId = request.UserId, Role = request.Role });
        }

        await this._context.SaveChangesAsync();
        return this.Ok();
    }

    /// <summary>
    /// Removes a user's shared access from a to-do list.
    /// </summary>
    [HttpDelete("{targetUserId}")]
    public async Task<IActionResult> RemoveShare(int todoListId, string targetUserId)
    {
        this.logger.LogInformation("Removing the {TargetUserId} user's access level from list {TodoListId}", targetUserId, todoListId);

        var ownerId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ownerId == null)
        {
            return this.Unauthorized();
        }

        var list = await this._context.TodoLists.FindAsync(todoListId);
        if (list == null)
        {
            return this.NotFound();
        }

        // Check if user is owner or has editor access
        var hasAccess = list.OwnerId == ownerId ||
                       await this._context.TodoListAccess.AnyAsync(a => a.TodoListId == todoListId && a.UserId == ownerId && a.Role == TodoListApp.Data.UserRole.Editor);

        if (!hasAccess)
        {
            return this.Forbid();
        }

        var access = await this._context.TodoListAccess.FirstOrDefaultAsync(a => a.TodoListId == todoListId && a.UserId == targetUserId);
        if (access != null)
        {
            this._context.TodoListAccess.Remove(access);
            await this._context.SaveChangesAsync();
        }

        return this.Ok();
    }
}
