using Microsoft.AspNetCore.SignalR;
using RevvUp.Core.Entities;
using RevvUp.Infrastructure.Data;
using RevvUp.Web.Hubs;
using System;
using System.Threading.Tasks;

namespace RevvUp.Web.Services;

public class NotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task CreateAsync(string userId, string message, string link, string type)
    {
        await CreateAsync(userId, message, link, type, null);
    }

    public async Task CreateAsync(string userId, string message, string link, string type, string? inquiryId)
    {
        // TRIGGER 3 Presence check: Do not create notification if the recipient is currently viewing that conversation
        if (type == "Message" && !string.IsNullOrEmpty(inquiryId) && NotificationHub.IsUserViewingConversation(userId, inquiryId))
        {
            return;
        }

        var notif = new Notification
        {
            UserId = userId,
            Message = message,
            Link = link,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notif);
        await _context.SaveChangesAsync();

        // Push to client in real time via SignalR
        await _hubContext.Clients.User(userId)
            .SendAsync("ReceiveNotification", new
            {
                id = notif.Id,
                message = notif.Message,
                link = notif.Link,
                type = notif.Type,
                isRead = notif.IsRead,
                createdAt = notif.CreatedAt
            });
    }
}
