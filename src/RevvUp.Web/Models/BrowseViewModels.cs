// ============================================================
// RevvUp.Web — Browse View Models
// Filter state, paging, and car card data for the browse page
// ============================================================

using System;
using System.Collections.Generic;

namespace RevvUp.Web.Models;

public class BrowseViewModel
{
    // ── Filter State ──
    public string? Search { get; set; }
    public string? Brand { get; set; }
    public string? BodyType { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public string? Color { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinYear { get; set; }
    public int? MaxYear { get; set; }
    public int? MaxMileage { get; set; }
    public string SortBy { get; set; } = "newest";

    // ── Results ──
    public List<CarCardViewModel> Cars { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 6;
    public bool HasMore => Page * PageSize < TotalCount;

    // ── Filter Options (for sidebar dropdowns) ──
    public List<string> AvailableBrands { get; set; } = new();
    public List<string> AvailableBodyTypes { get; set; } = new();
    public List<string> AvailableFuelTypes { get; set; } = new();
    public List<string> AvailableTransmissions { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
}

public class CarCardViewModel
{
    public Guid Id { get; set; }
    public string? SellerId { get; set; }
    public bool IsFavoritedByCurrentUser { get; set; } = false;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string FullName => $"{Year} {Brand} {Model}";
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string FormattedPrice => $"₱{Price:N0}";
    public string ImageUrls { get; set; } = string.Empty;
    public string PrimaryImageUrl => (ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "https://placehold.co/800x600/1a1f2e/3b82f6?text=No+Image";
    public int Mileage { get; set; }
    public string FormattedMileage => $"{Mileage:N0} mi";
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Engine { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; }
    public string Features { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public bool IsSold => Status == "Sold";
    public string TimeAgo
    {
        get
        {
            var diff = DateTime.UtcNow - DateAdded;
            if (diff.TotalHours < 1) return "Just now";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
            return DateAdded.ToString("MMM dd");
        }
    }
}
