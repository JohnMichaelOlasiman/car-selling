// ============================================================
// RevvUp.Core — Car Entity (Domain Model)
// Clean Architecture: Core layer — no dependencies
// ============================================================

using System;

namespace RevvUp.Core.Entities;

/// <summary>
/// Represents a car in the premium curated RevvUp collection.
/// </summary>
public class Car
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public int Mileage { get; set; }
    public string BodyType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty; // Excellent, Good, Fair
    public string Description { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty; // Comma-separated list
    public int ViewCount { get; set; }
    public int FavoriteCount { get; set; }
    public string ImageUrls { get; set; } = string.Empty; // Semicolon-separated URLs
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public bool IsFeatured { get; set; }
    public string Status { get; set; } = "Available"; // Available, Sold
}
