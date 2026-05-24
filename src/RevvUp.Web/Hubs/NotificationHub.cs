using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RevvUp.Web.Hubs;

public class NotificationHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> ConnectionUserMap = new();
    private static readonly ConcurrentDictionary<string, string> ActiveConversations = new(); // ConnectionId -> InquiryId

    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            ConnectionUserMap[Context.ConnectionId] = userId;
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectionUserMap.TryRemove(Context.ConnectionId, out _);
        ActiveConversations.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    public void JoinConversation(string inquiryId)
    {
        ActiveConversations[Context.ConnectionId] = inquiryId;
    }

    public void LeaveConversation()
    {
        ActiveConversations.TryRemove(Context.ConnectionId, out _);
    }

    public static bool IsUserViewingConversation(string userId, string inquiryId)
    {
        foreach (var pair in ActiveConversations)
        {
            if (ConnectionUserMap.TryGetValue(pair.Key, out var user) && user == userId && pair.Value == inquiryId)
            {
                return true;
            }
        }
        return false;
    }
}
