using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Data;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles user authentication: login, registration, and logout.
/// </summary>
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<AccountController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="userManager">The Identity user manager.</param>
    /// <param name="signInManager">The Identity sign-in manager.</param>
    /// <param name="logger">The logger instance.</param>
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
    }

    /// <summary>Displays the login page.</summary>
    /// <param name="returnUrl">The URL to redirect to after login.</param>
    /// <returns>The login view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (this.User.Identity?.IsAuthenticated == true)
        {
            return this.RedirectToAction("Index", "Home");
        }

        var model = new LoginViewModel { ReturnUrl = returnUrl };
        return this.View(model);
    }

    /// <summary>Processes the login form submission.</summary>
    /// <param name="model">The login form data.</param>
    /// <returns>Redirect on success, or the login view with errors.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var result = await this.signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            this.logger.LogInformation("User {Email} logged in.", model.Email);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && this.Url.IsLocalUrl(model.ReturnUrl))
            {
                return this.Redirect(model.ReturnUrl);
            }

            return this.RedirectToAction("Index", "Home");
        }

        this.ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return this.View(model);
    }

    /// <summary>Displays the registration page.</summary>
    /// <returns>The register view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (this.User.Identity?.IsAuthenticated == true)
        {
            return this.RedirectToAction("Index", "Home");
        }

        return this.View();
    }

    /// <summary>Processes the registration form submission.</summary>
    /// <param name="model">The registration form data.</param>
    /// <returns>Redirect on success, or the register view with errors.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
        };

        var result = await this.userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            this.logger.LogInformation("New user {Email} registered.", model.Email);
            await this.signInManager.SignInAsync(user, isPersistent: false);
            return this.RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError(string.Empty, error.Description);
        }

        return this.View(model);
    }

    /// <summary>Logs the current user out.</summary>
    /// <returns>Redirect to the home page.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await this.signInManager.SignOutAsync();
        this.logger.LogInformation("User logged out.");
        return this.RedirectToAction("Index", "Home");
    }

    /// <summary>Displays the access denied page.</summary>
    /// <returns>The access denied view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return this.View();
    }
}
