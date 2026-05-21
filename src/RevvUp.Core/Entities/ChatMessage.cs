// ============================================================
// RevvUp.Core — ChatMessage Entity (Domain Model)
// Clean Architecture: Core layer — no dependencies
// ============================================================

using System;

namespace RevvUp.Core.Entities;

/// <summary>
/// Represents a message sent inside an Inquiry thread between buyer and seller.
/// </summary>
public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InquiryId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
