// ============================================================
// RevvUp.Core — Car Entity (Domain Model)
// Clean Architecture: Core layer — no dependencies
// ============================================================

namespace RevvUp.Core.Entities;

/// <summary>
/// Represents a car listing in the RevvUp marketplace.
/// This is the core domain entity — pure business logic, no framework deps.
/// </summary>
public class Car
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsSold { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
