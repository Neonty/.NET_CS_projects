using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Data;
using TodoListApp.WebApp.Data;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Controller for managing the sharing of to-do lists.
/// </summary>
[Authorize]
public class ShareController : Controller
{
    private readonly ITodoListWebApiService _apiService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ShareController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShareController"/> class.
    /// </summary>
    public ShareController(ITodoListWebApiService apiService, UserManager<ApplicationUser> userManager, ILogger<ShareController> logger)
    {
        _apiService = apiService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Adds a user to the shared access list for a specific to-do list.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(int todoListId, string userEmail, UserRole role)
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ownerId == null)
        {
            return Unauthorized();
        }

        var todoList = await _apiService.GetTodoListByIdAsync(todoListId, ownerId);
        if (todoList == null || (todoList.AccessLevel != TodoListApp.Data.UserRole.Owner && todoList.AccessLevel != TodoListApp.Data.UserRole.Editor))
        {
            return Forbid();
        }

        var targetUser = await _userManager.FindByEmailAsync(userEmail);
        if (targetUser != null)
        {
            _logger.LogInformation("User {OwnerId} is granting {Role} access to {TargetUserId} on list {ListId}", ownerId, role, targetUser.Id, todoListId);
            await _apiService.AddShareAsync(todoListId, ownerId, targetUser.Id, role);
        }
        else
        {
            _logger.LogWarning("Attempted to share list {ListId} with non-existent user {Email}", todoListId, userEmail);
        }

        return RedirectToAction("Manage", "TodoList", new { id = todoListId });
    }

    /// <summary>
    /// Removes a user from the shared access list for a specific to-do list.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveUser(int todoListId, string targetUserId)
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ownerId == null)
        {
            return Unauthorized();
        }

        var todoList = await _apiService.GetTodoListByIdAsync(todoListId, ownerId);
        if (todoList == null || (todoList.AccessLevel != TodoListApp.Data.UserRole.Owner && todoList.AccessLevel != TodoListApp.Data.UserRole.Editor))
        {
            return Forbid();
        }

        _logger.LogInformation("User {OwnerId} is revoking access from {TargetUserId} on list {ListId}", ownerId, targetUserId, todoListId);
        await _apiService.RemoveShareAsync(todoListId, ownerId, targetUserId);
        return RedirectToAction("Manage", "TodoList", new { id = todoListId });
    }
}
