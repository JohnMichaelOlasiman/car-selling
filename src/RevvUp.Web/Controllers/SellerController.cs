// ============================================================
// RevvUp.Web — SellerController
// Seller-side CRUD: Create, Edit, Delete listings + Inquiries
// Any authenticated user can be a seller (Unified User model)
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RevvUp.Application.Interfaces;
using RevvUp.Core.Entities;
using RevvUp.Web.Models;

namespace RevvUp.Web.Controllers;

[Authorize]
public class SellerController : Controller
{
    private readonly ICarService _carService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<SellerController> _logger;
    private readonly RevvUp.Web.Services.NotificationService _notifService;

    public SellerController(
        ICarService carService,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env,
        ILogger<SellerController> logger,
        RevvUp.Web.Services.NotificationService notifService)
    {
        _carService = carService;
        _userManager = userManager;
        _env = env;
        _logger = logger;
        _notifService = notifService;
    }

    // ──────────────────────────────────────────────────────────
    // GET /Seller/MyListings
    // ──────────────────────────────────────────────────────────
    public async Task<IActionResult> MyListings(int page = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user?.Id ?? string.Empty;
        ViewData["UserName"] = user?.DisplayName ?? "Seller";

        var cars = await _carService.GetCarsBySellerIdAsync(userId);
        var carList = cars.ToList();

        // Gather inquiry counts per listing
        var listingVms = new List<SellerListingViewModel>();
        foreach (var car in carList)
        {
            var inquiries = await _carService.GetInquiriesByCarIdAsync(car.Id);
            listingVms.Add(new SellerListingViewModel
            {
                Id            = car.Id,
                Brand         = car.Brand,
                Model         = car.Model,
                Year          = car.Year,
                Price         = car.Price,
                Status        = car.Status,
                DateAdded     = car.DateAdded,
                ImageUrl      = (car.ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty,
                InquiryCount  = inquiries.Count(),
                FavoriteCount = car.FavoriteCount,
                ViewCount     = car.ViewCount,
                Mileage       = car.Mileage,
                FuelType      = car.FuelType,
                Transmission  = car.Transmission,
                BodyType      = car.BodyType
            });
        }

        // Sort by DateAdded descending (newest first)
        listingVms = listingVms.OrderByDescending(l => l.DateAdded).ToList();

        ViewBag.TotalListings  = listingVms.Count;
        ViewBag.ActiveListings = listingVms.Count(l => l.Status == "Available");
        ViewBag.SoldListings   = listingVms.Count(l => l.Status == "Sold");
        ViewBag.TotalInquiries = listingVms.Sum(l => l.InquiryCount);

        // Pagination
        int pageSize = 6;
        int totalCount = listingVms.Count;
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        page = Math.Max(1, Math.Min(totalPages > 0 ? totalPages : 1, page));

        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;

        var pagedListings = listingVms.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return View(pagedListings);
    }

    // ──────────────────────────────────────────────────────────
    // GET /Seller/Create
    // ──────────────────────────────────────────────────────────
    public IActionResult Create()
    {
        return View(new SellerCarFormViewModel());
    }

    // ──────────────────────────────────────────────────────────
    // POST /Seller/Create
    // ──────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SellerCarFormViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = await _userManager.GetUserAsync(User);
        var userId = user?.Id ?? string.Empty;

        // Handle image upload + external URLs
        var imageUrls = await ResolveImageUrls(vm.ImageFiles, vm.ImageUrlsRaw);
        if (string.IsNullOrEmpty(imageUrls))
        {
            ModelState.AddModelError("ImageUrlsRaw", "Please provide at least one image URL or upload a photo.");
            return View(vm);
        }

        var car = new Car
        {
            Id                  = Guid.NewGuid(),
            Brand               = vm.Brand.Trim(),
            Model               = vm.Model.Trim(),
            Year                = vm.Year,
            Price               = vm.Price,
            Mileage             = vm.Mileage,
            BodyType            = vm.BodyType,
            FuelType            = vm.FuelType,
            Transmission        = vm.Transmission,
            Color               = vm.Color.Trim(),
            Condition           = vm.Condition,
            Engine              = vm.Engine?.Trim() ?? string.Empty,
            Description         = vm.Description.Trim(),
            Features            = vm.Features?.Trim() ?? string.Empty,
            ImageUrls           = imageUrls,
            IsFeatured          = false,
            Status              = "Available",
            DateAdded           = DateTime.UtcNow,
            SellerId            = userId,
            SellerDisplayName   = user?.DisplayName ?? user?.UserName ?? "Seller"
        };

        await _carService.AddCarAsync(car);

        // Log listing to activity timeline
        await _notifService.CreateAsync(userId, $"Listed {car.Brand} {car.Model}", $"/Cars/Details/{car.Id}", "Activity:green");

        TempData["Success"] = "Your listing is now live!";
        return RedirectToAction(nameof(MyListings));
    }

    // ──────────────────────────────────────────────────────────
    // GET /Seller/Edit/{id}
    // ──────────────────────────────────────────────────────────
    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        var car    = await _carService.GetCarByIdAsync(id);
        if (car == null) return NotFound();
        if (car.SellerId != userId) return Forbid();

        var vm = new SellerCarFormViewModel
        {
            Id           = car.Id,
            Brand        = car.Brand,
            Model        = car.Model,
            Year         = car.Year,
            Price        = car.Price,
            Mileage      = car.Mileage,
            BodyType     = car.BodyType,
            FuelType     = car.FuelType,
            Transmission = car.Transmission,
            Color        = car.Color,
            Condition    = car.Condition,
            Engine       = car.Engine,
            Description  = car.Description,
            Features     = car.Features,
            Status       = car.Status,
            ImageUrlsRaw = car.ImageUrls.Replace(';', '\n')
        };

        return View("Create", vm);
    }

    // ──────────────────────────────────────────────────────────
    // POST /Seller/Edit/{id}
    // ──────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, SellerCarFormViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("Create", vm);

        var userId = _userManager.GetUserId(User);
        var car    = await _carService.GetCarByIdAsync(id);
        if (car == null) return NotFound();
        if (car.SellerId != userId) return Forbid();

        // Resolve images (keep existing if no new ones provided)
        var imageUrls = await ResolveImageUrls(vm.ImageFiles, vm.ImageUrlsRaw);
        if (string.IsNullOrEmpty(imageUrls))
            imageUrls = car.ImageUrls; // keep existing images

        car.Brand        = vm.Brand.Trim();
        car.Model        = vm.Model.Trim();
        car.Year         = vm.Year;
        car.Price        = vm.Price;
        car.Mileage      = vm.Mileage;
        car.BodyType     = vm.BodyType;
        car.FuelType     = vm.FuelType;
        car.Transmission = vm.Transmission;
        car.Color        = vm.Color.Trim();
        car.Condition    = vm.Condition;
        car.Engine       = vm.Engine?.Trim() ?? string.Empty;
        car.Description  = vm.Description.Trim();
        car.Features     = vm.Features?.Trim() ?? string.Empty;
        car.ImageUrls    = imageUrls;
        car.Status       = vm.Status;

        await _carService.UpdateCarAsync(car);
        TempData["Success"] = "Listing updated successfully.";
        return RedirectToAction(nameof(MyListings));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = _userManager.GetUserId(User) ?? string.Empty;
        var car    = await _carService.GetCarByIdAsync(id);
        if (car == null) return NotFound();
        if (car.SellerId != userId) return Forbid();

        var brand = car.Brand;
        var model = car.Model;

        await _carService.DeleteCarAsync(id);

        // Log deletion to activity timeline
        await _notifService.CreateAsync(userId, $"Removed listing {brand} {model}", "#", "Activity:pink");

        TempData["Success"] = "Listing removed.";
        return RedirectToAction(nameof(MyListings));
    }

    // ──────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkSold(Guid id)
    {
        var userId = _userManager.GetUserId(User) ?? string.Empty;
        var car    = await _carService.GetCarByIdAsync(id);
        if (car == null) return NotFound();
        if (car.SellerId != userId) return Forbid();

        car.Status = "Sold";
        await _carService.UpdateCarAsync(car);

        // Log sold status to activity timeline
        await _notifService.CreateAsync(userId, $"Marked {car.Brand} {car.Model} as Sold", $"/Cars/Details/{car.Id}", "Activity:accent");

        return RedirectToAction(nameof(MyListings));
    }

    // ──────────────────────────────────────────────────────────
    // GET /Seller/Inquiries/{carId}
    // ──────────────────────────────────────────────────────────
    public async Task<IActionResult> Inquiries(Guid carId)
    {
        var userId = _userManager.GetUserId(User);
        var car    = await _carService.GetCarByIdAsync(carId);
        if (car == null) return NotFound();
        if (car.SellerId != userId) return Forbid();

        return Redirect($"/Dashboard/Messages?tab=seller&carId={carId}");
    }

    // ──────────────────────────────────────────────────────────
    // POST /Seller/ReplyToInquiry — Seller replies to a buyer
    // ──────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplyToInquiry([FromBody] SellerReplyModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.MessageText))
            return BadRequest("Empty reply.");

        var userId  = _userManager.GetUserId(User) ?? string.Empty;
        var user    = await _userManager.GetUserAsync(User);

        var inquiry = await _carService.GetInquiryByIdAsync(model.InquiryId);
        if (inquiry == null) return NotFound();

        // Verify the current user IS the seller of that car
        var car = await _carService.GetCarByIdAsync(inquiry.CarId);
        if (car == null || car.SellerId != userId) return Forbid();

        var msg = new ChatMessage
        {
            Id          = Guid.NewGuid(),
            InquiryId   = model.InquiryId,
            SenderId    = userId,
            SenderName  = user?.DisplayName ?? "Seller",
            MessageText = model.MessageText,
            CreatedAt   = DateTime.UtcNow
        };

        await _carService.AddChatMessageAsync(msg);

        // TRIGGER 3: Send a message notification to the buyer (if registered and not self)
        if (!string.IsNullOrEmpty(inquiry.UserId) && inquiry.UserId != userId)
        {
            var senderName = user?.DisplayName ?? "Seller";
            var messageText = $"{senderName} sent you a message about {car.Year} {car.Brand} {car.Model}";
            var link = $"/Dashboard/Messages?inquiryId={inquiry.Id}";
            await _notifService.CreateAsync(inquiry.UserId, messageText, link, "Message", inquiry.Id.ToString());
        }

        return Json(new
        {
            id          = msg.Id,
            senderId    = msg.SenderId,
            senderName  = msg.SenderName,
            messageText = msg.MessageText,
            createdAt   = msg.CreatedAt.ToString("o"),
            isSeller    = true
        });
    }

    // ──────────────────────────────────────────────────────────
    // Private Helpers
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Resolves final semicolon-separated image URL string from either
    /// uploaded files (saved to wwwroot/uploads/cars/) or raw URL input.
    /// </summary>
    private async Task<string> ResolveImageUrls(IList<IFormFile>? files, string? rawUrls)
    {
        var urls = new List<string>();

        // 1. Handle uploaded image files
        if (files != null && files.Count > 0)
        {
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "cars");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            foreach (var file in files.Take(6))
            {
                if (file.Length > 0 && IsValidImage(file))
                {
                    var ext      = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadDir, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    urls.Add($"/uploads/cars/{fileName}");
                }
            }
        }

        // 2. Parse raw URL list (newline or semicolon separated)
        if (!string.IsNullOrWhiteSpace(rawUrls))
        {
            var parsed = rawUrls
                .Split(new[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(u => u.Trim())
                .Where(u => u.StartsWith("http://") || u.StartsWith("https://") || u.StartsWith("/"));
            urls.AddRange(parsed);
        }

        return string.Join(';', urls.Distinct().Take(6));
    }

    private static bool IsValidImage(IFormFile file)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var ext     = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowed.Contains(ext) && file.Length < 10 * 1024 * 1024; // max 10 MB
    }
}

// ── View Models ──────────────────────────────────────────────

public class SellerListingViewModel
{
    public Guid     Id            { get; set; }
    public string   Brand         { get; set; } = string.Empty;
    public string   Model         { get; set; } = string.Empty;
    public int      Year          { get; set; }
    public decimal  Price         { get; set; }
    public string   Status        { get; set; } = "Available";
    public DateTime DateAdded     { get; set; }
    public string   ImageUrl      { get; set; } = string.Empty;
    public int      InquiryCount  { get; set; }
    public int      FavoriteCount { get; set; }
    public int      ViewCount     { get; set; }

    // Additional fields matching CarCardViewModel specs
    public int      Mileage       { get; set; }
    public string   FuelType      { get; set; } = string.Empty;
    public string   Transmission  { get; set; } = string.Empty;
    public string   BodyType      { get; set; } = string.Empty;
    public string   FullName => $"{Year} {Brand} {Model}";
    public string   FormattedPrice => $"₱{Price:N0}";
    public string   FormattedMileage => $"{Mileage:N0} km";
}

public class SellerReplyModel
{
    public Guid   InquiryId   { get; set; }
    public string MessageText { get; set; } = string.Empty;
}
