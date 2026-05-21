// ============================================================
// RevvUp.Web — Browse View Models
// Filter state, paging, and car card data for the browse page
// ============================================================

namespace RevvUp.Web.Models;

public class BrowseViewModel
{
    // ── Filter State ──
    public string? Search { get; set; }
    public string? Make { get; set; }
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
    public int PageSize { get; set; } = 9;
    public bool HasMore => Page * PageSize < TotalCount;

    // ── Filter Options (for sidebar dropdowns) ──
    public List<string> AvailableMakes { get; set; } = new();
    public List<string> AvailableBodyTypes { get; set; } = new();
    public List<string> AvailableFuelTypes { get; set; } = new();
    public List<string> AvailableTransmissions { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
}

public class CarCardViewModel
{
    public Guid Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string FullName => $"{Year} {Make} {Model}";
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string FormattedPrice => $"₱{Price:N0}";
    public string ImageUrl { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string FormattedMileage => $"{Mileage:N0} km";
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string TimeAgo
    {
        get
        {
            var diff = DateTime.UtcNow - CreatedAt;
            if (diff.TotalHours < 1) return "Just now";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
            return CreatedAt.ToString("MMM dd");
        }
    }
}
