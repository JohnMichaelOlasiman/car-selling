// ============================================================
// RevvUp.Web — AccountController
// Handles Login, Register, Logout, Profile
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RevvUp.Core.Entities;
using RevvUp.Web.Models;
using RevvUp.Application.Interfaces;

namespace RevvUp.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICarService _carService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ICarService carService,
        IWebHostEnvironment webHostEnvironment,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _carService = carService;
        _webHostEnvironment = webHostEnvironment;
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

        ModelState.AddModelError(string.Empty, "Invalid email or password. Please try again.");
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
            JoinedAt = user.JoinedAt
        };

        var favorites = await _carService.GetFavoritesByUserIdAsync(user.Id);
        var inquiries = await _carService.GetInquiriesByUserIdAsync(user.Id);
        ViewBag.FavoritesCount = favorites.Count();
        ViewBag.InquiriesCount = inquiries.Count();

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login");

        model.Email = user.Email ?? string.Empty;
        model.AvatarUrl = user.AvatarUrl;
        model.JoinedAt = user.JoinedAt;

        var favorites = await _carService.GetFavoritesByUserIdAsync(user.Id);
        var inquiries = await _carService.GetInquiriesByUserIdAsync(user.Id);
        ViewBag.FavoritesCount = favorites.Count();
        ViewBag.InquiriesCount = inquiries.Count();

        if (!ModelState.IsValid)
            return View(model);

        user.DisplayName = model.DisplayName;
        user.Bio = model.Bio;
        user.Location = model.Location;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
    {
        if (avatarFile == null || avatarFile.Length == 0)
        {
            return BadRequest("Please select a file to upload.");
        }

        // Validate image size (max 2MB)
        if (avatarFile.Length > 2 * 1024 * 1024)
        {
            return BadRequest("Image must be under 2MB.");
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest("Only JPG, JPEG, and PNG images are allowed.");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        // Save image to wwwroot/uploads/avatars
        var avatarsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars");
        if (!Directory.Exists(avatarsFolder))
        {
            Directory.CreateDirectory(avatarsFolder);
        }

        // Delete old avatar file if exists
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, user.AvatarUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
            {
                try { System.IO.File.Delete(oldPath); } catch { /* ignore */ }
            }
        }

        var uniqueFileName = Guid.NewGuid().ToString() + extension;
        var filePath = Path.Combine(avatarsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await avatarFile.CopyToAsync(fileStream);
        }

        user.AvatarUrl = "/uploads/avatars/" + uniqueFileName;
        await _userManager.UpdateAsync(user);

        return Ok(new { avatarUrl = user.AvatarUrl });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteAvatar()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, user.AvatarUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
            {
                try { System.IO.File.Delete(oldPath); } catch { /* ignore */ }
            }

            user.AvatarUrl = null;
            await _userManager.UpdateAsync(user);
        }

        return Ok();
    }

    // ── ACCESS DENIED ──
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
