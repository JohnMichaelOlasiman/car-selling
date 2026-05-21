// ============================================================
// RevvUp.Web — DashboardController
// User dashboard with sidebar navigation (Buyer Only)
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
using RevvUp.Core.Entities;
using RevvUp.Application.Interfaces;

namespace RevvUp.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICarService _carService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        UserManager<ApplicationUser> userManager,
        ICarService carService,
        ILogger<DashboardController> logger)
    {
        _userManager = userManager;
        _carService = carService;
        _logger = logger;
    }

    /// <summary>Dashboard overview (Buyer Profile)</summary>
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = _userManager.GetUserId(User) ?? string.Empty;
        ViewData["UserName"] = user?.DisplayName ?? "Buyer";

        var favorites = await _carService.GetFavoritesByUserIdAsync(userId);
        var inquiries = await _carService.GetInquiriesByUserIdAsync(userId);

        ViewBag.FavoritesCount = favorites.Count();
        ViewBag.InquiriesCount = inquiries.Count();

        // ── Chronological Recent Activity Feed ──
        var activities = new List<DashboardActivity>();

        // 1. Saved Cars (Favorites)
        foreach (var fav in favorites)
        {
            var car = await _carService.GetCarByIdAsync(fav.CarId);
            activities.Add(new DashboardActivity
            {
                Title = car != null ? $"Saved {car.Brand} {car.Model}" : "Saved Vehicle",
                Description = "You added this vehicle to your premium catalog favorites.",
                Timestamp = fav.DateAdded,
                Color = "primary",
                Link = car != null ? $"/Cars/Details/{car.Id}" : "#"
            });
        }

        // 2. Sent Inquiries
        foreach (var inq in inquiries)
        {
            var car = await _carService.GetCarByIdAsync(inq.CarId);
            activities.Add(new DashboardActivity
            {
                Title = car != null ? $"Inquired on {car.Brand} {car.Model}" : "Vehicle Inquiry Sent",
                Description = $"You messaged dealership coordinators: \"{inq.Message}\"",
                Timestamp = inq.InquiryDate,
                Color = "accent",
                Link = "/Dashboard/Messages"
            });
        }

        // Sort by newest, take top 5
        ViewBag.RecentActivities = activities
            .OrderByDescending(a => a.Timestamp)
            .Take(5)
            .ToList();

        return View();
    }

    /// <summary>Saved / favorited cars</summary>
    public async Task<IActionResult> SavedCars()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserName"] = user?.DisplayName ?? "Buyer";

        var userId = _userManager.GetUserId(User) ?? string.Empty;
        var favorites = await _carService.GetFavoritesByUserIdAsync(userId);

        var favCars = new List<Car>();
        foreach (var fav in favorites)
        {
            var car = await _carService.GetCarByIdAsync(fav.CarId);
            if (car != null)
            {
                favCars.Add(car);
            }
        }

        ViewBag.AllCarsJson = System.Text.Json.JsonSerializer.Serialize(favCars.Select(c => new {
            id = c.Id,
            name = $"{c.Year} {c.Brand} {c.Model}",
            price = $"${c.Price:N0}",
            image = c.ImageUrls.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty,
            mileage = $"{c.Mileage:N0} mi",
            transmission = c.Transmission,
            fuelType = c.FuelType,
            location = "Dealership Center"
        }));

        return View();
    }

    /// <summary>Mailbox for submitted inquiries</summary>
    public async Task<IActionResult> Messages()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = _userManager.GetUserId(User) ?? string.Empty;
        ViewData["UserName"] = user?.DisplayName ?? "Buyer";

        var inquiries = await _carService.GetInquiriesByUserIdAsync(userId);
        var threads = new List<ChatThreadViewModel>();

        foreach (var inq in inquiries)
        {
            var car = await _carService.GetCarByIdAsync(inq.CarId);
            
            // dealer coordinator name
            string otherPartyName = "Dealer Support Team";
            string otherPartyRole = "Coordinator";

            // Get last message snippet and time
            var chatMsgs = await _carService.GetChatMessagesByInquiryIdAsync(inq.Id);
            var chatList = chatMsgs.ToList();
            
            string lastSnippet = inq.Message;
            DateTime lastTime = inq.InquiryDate;

            if (chatList.Any())
            {
                var lastMsg = chatList.Last();
                lastSnippet = lastMsg.MessageText;
                lastTime = lastMsg.CreatedAt;
            }

            // Clean snippet length
            if (lastSnippet.Length > 60)
            {
                lastSnippet = lastSnippet.Substring(0, 57) + "...";
            }

            threads.Add(new ChatThreadViewModel
            {
                InquiryId = inq.Id,
                CarId = inq.CarId,
                CarName = car != null ? $"{car.Year} {car.Brand} {car.Model}" : "Premium Vehicle",
                CarImageUrl = car != null ? (car.ImageUrls.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty) : string.Empty,
                CarPrice = car != null ? $"${car.Price:N0}" : string.Empty,
                OtherPartyName = otherPartyName,
                OtherPartyRole = otherPartyRole,
                LastMessageSnippet = lastSnippet,
                LastMessageTime = lastTime,
                IsSeller = false
            });
        }

        // Sort by latest message time
        var sortedThreads = threads.OrderByDescending(t => t.LastMessageTime).ToList();

        return View(sortedThreads);
    }

    [HttpGet]
    public async Task<IActionResult> GetChatHistory(Guid inquiryId)
    {
        var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
        var inquiry = await _carService.GetInquiryByIdAsync(inquiryId);
        if (inquiry == null)
        {
            return NotFound();
        }

        // Authorize user
        if (inquiry.UserId != currentUserId)
        {
            return Forbid();
        }

        var dbMessages = await _carService.GetChatMessagesByInquiryIdAsync(inquiryId);

        // Combine initial message and replies
        var history = new List<object>();

        // 1. Initial inquiry message
        history.Add(new
        {
            id = inquiry.Id,
            senderId = inquiry.UserId,
            senderName = inquiry.Name,
            messageText = inquiry.Message,
            createdAt = inquiry.InquiryDate.ToString("o"),
            isCurrentUser = true
        });

        // 2. Subsequent chat messages
        foreach (var msg in dbMessages)
        {
            history.Add(new
            {
                id = msg.Id,
                senderId = msg.SenderId,
                senderName = msg.SenderName,
                messageText = msg.MessageText,
                createdAt = msg.CreatedAt.ToString("o"),
                isCurrentUser = msg.SenderId == currentUserId
            });
        }

        return Json(history);
    }

    [HttpPost]
    public async Task<IActionResult> SendChatMessage([FromBody] SendMessageModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.MessageText))
        {
            return BadRequest("Message text cannot be empty.");
        }

        var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
        var user = await _userManager.GetUserAsync(User);
        var senderName = user?.DisplayName ?? "User";

        var inquiry = await _carService.GetInquiryByIdAsync(model.InquiryId);
        if (inquiry == null)
        {
            return NotFound("Inquiry thread not found.");
        }

        if (inquiry.UserId != currentUserId)
        {
            return Forbid();
        }

        var newMessage = new ChatMessage
        {
            Id = Guid.NewGuid(),
            InquiryId = model.InquiryId,
            SenderId = currentUserId,
            SenderName = senderName,
            MessageText = model.MessageText,
            CreatedAt = DateTime.UtcNow
        };

        await _carService.AddChatMessageAsync(newMessage);

        return Json(new
        {
            id = newMessage.Id,
            senderId = newMessage.SenderId,
            senderName = newMessage.SenderName,
            messageText = newMessage.MessageText,
            createdAt = newMessage.CreatedAt.ToString("o"),
            isCurrentUser = true
        });
    }
}

/// <summary>
/// Model representing a chronological dashboard event.
/// </summary>
public class DashboardActivity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Color { get; set; } = "primary"; // primary, green, accent, pink, purple
    public string Link { get; set; } = "#";
}

public class ChatThreadViewModel
{
    public Guid InquiryId { get; set; }
    public Guid CarId { get; set; }
    public string CarName { get; set; } = string.Empty;
    public string CarImageUrl { get; set; } = string.Empty;
    public string CarPrice { get; set; } = string.Empty;
    public string OtherPartyName { get; set; } = string.Empty;
    public string OtherPartyRole { get; set; } = string.Empty;
    public string LastMessageSnippet { get; set; } = string.Empty;
    public DateTime LastMessageTime { get; set; }
    public bool IsSeller { get; set; }
}

public class SendMessageModel
{
    public Guid InquiryId { get; set; }
    public string MessageText { get; set; } = string.Empty;
}
