// ============================================================
// RevvUp.Core — Inquiry Entity (Domain Model)
// Clean Architecture: Core layer — no dependencies
// ============================================================

using System;

namespace RevvUp.Core.Entities;

/// <summary>
/// Represents a buyer's inquiry for a premium car listing.
/// </summary>
public class Inquiry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? UserId { get; set; } // Nullable for guest buyers
    public Guid CarId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Pending, Contacted, Resolved
}
