// ============================================================
// RevvUp.Web — AccountController
// Handles Login, Register, Logout, Profile
// Phase 2: Beautiful auth flows
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RevvUp.Core.Entities;
using RevvUp.Web.Models;

namespace RevvUp.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    // ── LOGIN ──
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in.", model.Email);
            return LocalRedirect(returnUrl ?? "/Dashboard");
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password. Give it another spin! 🏎️");
        return View(model);
    }

    // ── REGISTER ──
    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName,
            IsDealer = model.IsDealer,
            JoinedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("New user {Email} registered.", model.Email);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl ?? "/Dashboard");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // ── LOGOUT ──
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }

    // ── PROFILE ──
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login");

        var model = new ProfileViewModel
        {
            DisplayName = user.DisplayName,
            Email = user.Email ?? string.Empty,
            Bio = user.Bio,
            Location = user.Location,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            JoinedAt = user.JoinedAt,
            TotalListings = user.TotalListings,
            TotalSales = user.TotalSales,
            IsDealer = user.IsDealer
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login");

        user.DisplayName = model.DisplayName;
        user.Bio = model.Bio;
        user.Location = model.Location;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Profile updated successfully! 🎉";
            return RedirectToAction("Profile");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // ── ACCESS DENIED ──
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
