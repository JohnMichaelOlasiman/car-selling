// ============================================================
// RevvUp.Core — Favorite Entity (Domain Model)
// Clean Architecture: Core layer — no dependencies
// ============================================================

using System;

namespace RevvUp.Core.Entities;

/// <summary>
/// Represents a buyer's saved/wishlisted premium car.
/// </summary>
public class Favorite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public Guid CarId { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
