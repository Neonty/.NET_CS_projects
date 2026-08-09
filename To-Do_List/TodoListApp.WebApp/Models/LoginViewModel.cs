using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the data submitted by the login form.
/// </summary>
public class LoginViewModel
{
    /// <summary>Gets or sets the user's email address.</summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's password.</summary>
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the user wants to be remembered.</summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    /// <summary>Gets or sets the URL to redirect to after login.</summary>
    public string? ReturnUrl { get; set; }
}
