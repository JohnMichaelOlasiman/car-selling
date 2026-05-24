// ============================================================
// RevvUp.Web — Seller View Models
// Used by SellerController for Create/Edit car listing forms
// ============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RevvUp.Web.Models;

/// <summary>
/// Shared form view model for creating and editing a car listing.
/// </summary>
public class SellerCarFormViewModel
{
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required, Range(1980, 2030)]
    public int Year { get; set; } = DateTime.Now.Year;

    [Required, Range(1, 100_000_000)]
    public decimal Price { get; set; }

    [Required, Range(0, 1_000_000)]
    public int Mileage { get; set; }

    [Required]
    public string BodyType { get; set; } = string.Empty;

    [Required]
    public string FuelType { get; set; } = string.Empty;

    [Required]
    public string Transmission { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Color { get; set; } = string.Empty;

    [Required]
    public string Condition { get; set; } = "Good";

    [StringLength(150)]
    public string? Engine { get; set; }

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Features { get; set; }

    /// <summary>Newline or semicolon separated image URLs (alternative to upload)</summary>
    public string? ImageUrlsRaw { get; set; }

    /// <summary>Uploaded image files (max 6, each max 10 MB)</summary>
    public IList<IFormFile>? ImageFiles { get; set; }

    /// <summary>Only relevant for Edit — Available, Sold, Pending</summary>
    public string Status { get; set; } = "Available";

    // ── Dropdown Options (populated in views) ──
    public static List<string> BodyTypeOptions { get; } = new()
        { "Sedan", "SUV", "Hatchback", "Coupe", "Convertible", "Wagon", "Truck", "Van" };

    public static List<string> FuelTypeOptions { get; } = new()
        { "Gasoline", "Diesel", "Hybrid", "Electric" };

    public static List<string> TransmissionOptions { get; } = new()
        { "Automatic", "Manual", "CVT" };

    public static List<string> ConditionOptions { get; } = new()
        { "Excellent", "Good", "Fair", "Poor" };

    public static List<string> StatusOptions { get; } = new()
        { "Available", "Sold", "Pending" };
}
