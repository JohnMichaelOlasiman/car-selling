// ============================================================
// RevvUp.Core — ApplicationUser Entity
// Extends IdentityUser with RevvUp-specific profile fields
// ============================================================

using Microsoft.AspNetCore.Identity;
using System;

namespace RevvUp.Core.Entities;

/// <summary>
/// Extended user entity for RevvUp.
/// Adds display name, avatar, bio, and profile customization.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? PhoneNumber2 { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // ── Seller Info ──
    public string? SellerDisplayName { get; set; }
    public string? SellerPhoneNumber { get; set; }
    public string? SellerLocation { get; set; }
    public string? SellerBio { get; set; }
}
