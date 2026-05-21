// ============================================================
// RevvUp.Web — Account View Models
// Login, Register, and Profile form models
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace RevvUp.Web.Models;

/// <summary>Login form model</summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}

/// <summary>Registration form model</summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Display name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters")]
    [Display(Name = "Display Name")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords don't match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "I'm a dealer")]
    public bool IsDealer { get; set; }
}

/// <summary>Profile edit form model</summary>
public class ProfileViewModel
{
    [Required(ErrorMessage = "Display name is required")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Display Name")]
    public string DisplayName { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [StringLength(200)]
    [Display(Name = "Location")]
    public string? Location { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }
    public DateTime JoinedAt { get; set; }
    public int TotalListings { get; set; }
    public int TotalSales { get; set; }
    public bool IsDealer { get; set; }
}
