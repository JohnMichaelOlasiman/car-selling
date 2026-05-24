using System;

namespace RevvUp.Core.Entities;

/// <summary>
/// Represents a user notification for favorites, inquiries, and chat messages.
/// </summary>
public class Notification
{
    public int Id { get; set; }
    
    /// <summary>
    /// The ID of the recipient user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of notification: "Favorite", "Inquiry", "Message"
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    public bool IsRead { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ApplicationUser? User { get; set; }
}
