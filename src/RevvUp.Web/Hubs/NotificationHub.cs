using Microsoft.AspNetCore.SignalR;

namespace RevvUp.Web.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotification(string title, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", title, message);
    }
}
