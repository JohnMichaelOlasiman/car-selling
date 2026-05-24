// ============================================================
// RevvUp.Web — DashboardController
// User dashboard with sidebar navigation (Buyer Only)
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RevvUp.Core.Entities;
using RevvUp.Application.Interfaces;
using RevvUp.Infrastructure.Data;

namespace RevvUp.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICarService _carService;
    private readonly ILogger<DashboardController> _logger;
    private readonly RevvUp.Web.Services.NotificationService _notifService;
    private readonly ApplicationDbContext _context;

    public DashboardController(
        UserManager<ApplicationUser> userManager,
        ICarService carService,
        ILogger<DashboardController> logger,
        RevvUp.Web.Services.NotificationService notifService,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _carService = carService;
        _logger = logger;
        _notifService = notifService;
        _context = context;
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

        // 3. Seller listings & sold statuses (dynamic fallbacks for existing data)
        var sellerCars = await _carService.GetCarsBySellerIdAsync(userId);
        foreach (var car in sellerCars)
        {
            activities.Add(new DashboardActivity
            {
                Title = $"Listed {car.Brand} {car.Model}",
                Description = $"You created a premium listing for ₱{car.Price:N0}.",
                Timestamp = car.DateAdded,
                Color = "green",
                Link = $"/Cars/Details/{car.Id}"
            });

            if (car.Status == "Sold")
            {
                activities.Add(new DashboardActivity
                {
                    Title = $"Marked {car.Brand} {car.Model} as Sold",
                    Description = "You marked this premium vehicle listing as Sold.",
                    Timestamp = car.DateAdded.AddHours(2) > DateTime.UtcNow ? DateTime.UtcNow : car.DateAdded.AddHours(2),
                    Color = "accent",
                    Link = $"/Cars/Details/{car.Id}"
                });
            }
        }

        // 4. Explicitly logged Activity Notifications (e.g. deletions, accurate timestamps)
        var activityNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.Type.StartsWith("Activity"))
            .ToListAsync();

        foreach (var act in activityNotifications)
        {
            var parts = act.Type.Split(':');
            var color = parts.Length > 1 ? parts[1] : "primary";

            // Avoid double-rendering exact matches from the fallback sellerCars query
            if (activities.Any(a => a.Title == act.Message && Math.Abs((a.Timestamp - act.CreatedAt).TotalSeconds) < 120))
            {
                continue;
            }

            string desc = "Activity logged on your dashboard.";
            if (act.Type == "Activity:green")
            {
                desc = "Successfully posted this premium listing to the live inventory catalog.";
            }
            else if (act.Type == "Activity:accent")
            {
                desc = "Marked this premium vehicle listing as Sold.";
            }
            else if (act.Type == "Activity:pink")
            {
                desc = "Permanently removed this listing from the active showroom.";
            }

            activities.Add(new DashboardActivity
            {
                Title = act.Message,
                Description = desc,
                Timestamp = act.CreatedAt,
                Color = color,
                Link = act.Link
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
            price = $"₱{c.Price:N0}",
            image = (c.ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty,
            mileage = $"{c.Mileage:N0} km",
            transmission = c.Transmission,
            fuelType = c.FuelType,
            location = "Dealership Center",
            isSold = c.IsSold
        }));

        // Featured cars for the empty-state suggestion grid (up to 8 from catalog)
        var browseCars = await _carService.GetAllCarsAsync();
        ViewBag.BrowseCarsJson = System.Text.Json.JsonSerializer.Serialize(
            browseCars
                .OrderByDescending(c => c.DateAdded)
                .Take(8)
                .Select(c => new {
                    id = c.Id,
                    name = $"{c.Year} {c.Brand} {c.Model}",
                    price = $"₱{c.Price:N0}",
                    image = (c.ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty,
                    mileage = $"{c.Mileage:N0} km",
                    transmission = c.Transmission,
                    fuelType = c.FuelType,
                    isSold = c.IsSold
                }));

        return View();
    }

    /// <summary>Mailbox for submitted inquiries</summary>
    public async Task<IActionResult> Messages()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = _userManager.GetUserId(User) ?? string.Empty;
        ViewData["UserName"] = user?.DisplayName ?? "Buyer";

        var threads = new List<ChatThreadViewModel>();

        // 1. As a Buyer: Inquiries sent by the current user
        var buyerInquiries = await _carService.GetInquiriesByUserIdAsync(userId);
        foreach (var inq in buyerInquiries)
        {
            var car = await _carService.GetCarByIdAsync(inq.CarId);
            
            // Get actual seller name if available, otherwise fallback to support team
            string otherPartyName = car != null ? (car.SellerDisplayName ?? "Dealer Support Team") : "Dealer Support Team";
            string otherPartyRole = car != null && !string.IsNullOrEmpty(car.SellerId) ? "Seller" : "Coordinator";

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
                CarImageUrl = car != null ? ((car.ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty) : string.Empty,
                CarPrice = car != null ? $"₱{car.Price:N0}" : string.Empty,
                OtherPartyName = otherPartyName,
                OtherPartyRole = otherPartyRole,
                LastMessageSnippet = lastSnippet,
                LastMessageTime = lastTime,
                IsSeller = false,
                CarMileage = car != null ? $"{car.Mileage:N0} km" : string.Empty,
                CarTransmission = car?.Transmission ?? string.Empty,
                CarFuelType = car?.FuelType ?? string.Empty
            });
        }

        // 2. As a Seller: Inquiries received for the user's listed cars
        var sellerCars = await _carService.GetCarsBySellerIdAsync(userId);
        foreach (var car in sellerCars)
        {
            var carInquiries = await _carService.GetInquiriesByCarIdAsync(car.Id);
            foreach (var inq in carInquiries)
            {
                if (inq.IsDeletedBySeller)
                {
                    continue;
                }

                // Buyer's details (the sender of the inquiry)
                string otherPartyName = inq.Name;
                string otherPartyRole = "Buyer";

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
                    CarName = $"{car.Year} {car.Brand} {car.Model}",
                    CarImageUrl = (car.ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty,
                    CarPrice = $"₱{car.Price:N0}",
                    OtherPartyName = otherPartyName,
                    OtherPartyRole = otherPartyRole,
                    LastMessageSnippet = lastSnippet,
                    LastMessageTime = lastTime,
                    IsSeller = true,
                    CarMileage = $"{car.Mileage:N0} km",
                    CarTransmission = car.Transmission,
                    CarFuelType = car.FuelType
                });
            }
        }

        // Sort all threads by latest message time
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

        // Authorize: buyer OR the seller of the car
        var car = await _carService.GetCarByIdAsync(inquiry.CarId);
        bool isBuyer  = inquiry.UserId == currentUserId;
        bool isSeller = car != null && car.SellerId == currentUserId;
        if (!isBuyer && !isSeller)
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
            isCurrentUser = inquiry.UserId == currentUserId
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
    [ValidateAntiForgeryToken]
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

        // Authorize: buyer OR the seller of the car
        var car = await _carService.GetCarByIdAsync(inquiry.CarId);
        bool isBuyer  = inquiry.UserId == currentUserId;
        bool isSeller = car != null && car.SellerId == currentUserId;
        if (!isBuyer && !isSeller)
        {
            return Forbid();
        }

        // Auto-restore logic: When a new message arrives from the buyer, reset the soft delete flag
        if (isBuyer && inquiry.IsDeletedBySeller)
        {
            inquiry.IsDeletedBySeller = false;
            inquiry.SellerDeletedAt = null;
            await _carService.UpdateInquiryAsync(inquiry);
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

        // TRIGGER 3: Send a message notification to the other party (recipient)
        string? recipientId = isBuyer ? car?.SellerId : inquiry.UserId;
        if (!string.IsNullOrEmpty(recipientId) && recipientId != currentUserId)
        {
            var carInfo = car != null ? $"{car.Year} {car.Brand} {car.Model}" : "vehicle";
            var messageText = $"{senderName} sent you a message about {carInfo}";
            var link = $"/Dashboard/Messages?inquiryId={inquiry.Id}";
            await _notifService.CreateAsync(recipientId, messageText, link, "Message", inquiry.Id.ToString());
        }

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

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConversation(Guid id)
    {
        var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
        var inquiry = await _carService.GetInquiryByIdAsync(id);
        if (inquiry == null)
        {
            return NotFound("Inquiry thread not found.");
        }
 
        var car = await _carService.GetCarByIdAsync(inquiry.CarId);
        bool isBuyer  = inquiry.UserId == currentUserId;
        bool isSeller = car != null && car.SellerId == currentUserId;
        if (!isBuyer && !isSeller)
        {
            return Forbid();
        }
 
        if (isSeller)
        {
            inquiry.IsDeletedBySeller = true;
            inquiry.SellerDeletedAt = DateTime.UtcNow;
            await _carService.UpdateInquiryAsync(inquiry);
        }
        else
        {
            await _carService.DeleteInquiryAsync(id);
        }
        return Content("");
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
    public string CarMileage { get; set; } = string.Empty;
    public string CarTransmission { get; set; } = string.Empty;
    public string CarFuelType { get; set; } = string.Empty;
}

public class SendMessageModel
{
    public Guid InquiryId { get; set; }
    public string MessageText { get; set; } = string.Empty;
}
