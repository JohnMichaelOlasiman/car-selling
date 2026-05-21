// ============================================================
// RevvUp.Web — CarsController
// Phase 3: Browse, Search, Filter, and Car Details
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevvUp.Application.Interfaces;
using RevvUp.Core.Entities;
using RevvUp.Web.Models;

namespace RevvUp.Web.Controllers;

public class CarsController : Controller
{
    private readonly ICarService _carService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CarsController(ICarService carService, UserManager<ApplicationUser> userManager)
    {
        _carService = carService;
        _userManager = userManager;
    }

    /// <summary>Browse all cars with filters + pagination</summary>
    [HttpGet]
    public async Task<IActionResult> Browse(
        string? search, string? brand, string? bodyType, string? fuelType,
        string? transmission, string? color, decimal? minPrice, decimal? maxPrice,
        int? minYear, int? maxYear, int? maxMileage, string sortBy = "newest", int page = 1)
    {
        var allCars = await _carService.GetAllCarsAsync();
        var carsList = allCars.ToList();

        // ── Build filter options from all data ──
        var vm = new BrowseViewModel
        {
            Search = search, Brand = brand, BodyType = bodyType, FuelType = fuelType,
            Transmission = transmission, Color = color, MinPrice = minPrice, MaxPrice = maxPrice,
            MinYear = minYear, MaxYear = maxYear, MaxMileage = maxMileage, SortBy = sortBy, Page = page,
            AvailableBrands = carsList.Select(c => c.Brand).Distinct().OrderBy(x => x).ToList(),
            AvailableBodyTypes = carsList.Select(c => c.BodyType).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList(),
            AvailableFuelTypes = carsList.Select(c => c.FuelType).Distinct().OrderBy(x => x).ToList(),
            AvailableTransmissions = carsList.Select(c => c.Transmission).Distinct().OrderBy(x => x).ToList(),
            AvailableColors = carsList.Select(c => c.Color).Distinct().OrderBy(x => x).ToList()
        };

        // ── Apply filters ──
        var filtered = carsList.AsEnumerable();
        if (!string.IsNullOrEmpty(search))
            filtered = filtered.Where(c =>
                c.Brand.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(brand)) filtered = filtered.Where(c => c.Brand == brand);
        if (!string.IsNullOrEmpty(bodyType)) filtered = filtered.Where(c => c.BodyType == bodyType);
        if (!string.IsNullOrEmpty(fuelType)) filtered = filtered.Where(c => c.FuelType == fuelType);
        if (!string.IsNullOrEmpty(transmission)) filtered = filtered.Where(c => c.Transmission == transmission);
        if (!string.IsNullOrEmpty(color)) filtered = filtered.Where(c => c.Color == color);
        if (minPrice.HasValue) filtered = filtered.Where(c => c.Price >= minPrice.Value);
        if (maxPrice.HasValue) filtered = filtered.Where(c => c.Price <= maxPrice.Value);
        if (minYear.HasValue) filtered = filtered.Where(c => c.Year >= minYear.Value);
        if (maxYear.HasValue) filtered = filtered.Where(c => c.Year <= maxYear.Value);
        if (maxMileage.HasValue) filtered = filtered.Where(c => c.Mileage <= maxMileage.Value);

        // ── Sort ──
        filtered = sortBy switch
        {
            "price-low" => filtered.OrderBy(c => c.Price),
            "price-high" => filtered.OrderByDescending(c => c.Price),
            "year-new" => filtered.OrderByDescending(c => c.Year),
            "mileage-low" => filtered.OrderBy(c => c.Mileage),
            _ => filtered.OrderByDescending(c => c.DateAdded)
        };

        var filteredList = filtered.ToList();
        vm.TotalCount = filteredList.Count;
        vm.Cars = filteredList.Skip((page - 1) * vm.PageSize).Take(vm.PageSize)
            .Select(c => new CarCardViewModel
            {
                Id = c.Id, Brand = c.Brand, Model = c.Model, Year = c.Year,
                Price = c.Price, ImageUrls = c.ImageUrls, Mileage = c.Mileage,
                FuelType = c.FuelType, Transmission = c.Transmission, BodyType = c.BodyType,
                Color = c.Color, Condition = c.Condition, IsFeatured = c.IsFeatured,
                Description = c.Description, DateAdded = c.DateAdded, Features = c.Features, Status = c.Status
            }).ToList();

        // ── HTMX partial for infinite scroll ──
        if (Request.Headers.ContainsKey("HX-Request"))
            return PartialView("_CarGrid", vm);

        return View(vm);
    }

    /// <summary>Search autocomplete API (JSON)</summary>
    [HttpGet]
    public async Task<IActionResult> SearchSuggestions(string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Json(Array.Empty<object>());

        var cars = await _carService.SearchCarsAsync(q);
        var results = cars.Take(6).Select(c => new
        {
            id = c.Id,
            name = $"{c.Year} {c.Brand} {c.Model}",
            price = $"${c.Price:N0}",
            image = c.ImageUrls.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty,
            bodyType = c.BodyType
        });
        return Json(results);
    }

    /// <summary>Car detail page</summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null) return NotFound();

        var vm = new CarCardViewModel
        {
            Id = car.Id, Brand = car.Brand, Model = car.Model, Year = car.Year,
            Price = car.Price, ImageUrls = car.ImageUrls, Mileage = car.Mileage,
            FuelType = car.FuelType, Transmission = car.Transmission, BodyType = car.BodyType,
            Color = car.Color, Condition = car.Condition, IsFeatured = car.IsFeatured,
            Description = car.Description, DateAdded = car.DateAdded, Features = car.Features, Status = car.Status
        };

        var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
        ViewBag.CurrentUserId = currentUserId;

        // Check if this car is already favorited
        ViewBag.IsFavorite = false;
        if (!string.IsNullOrEmpty(currentUserId))
        {
            ViewBag.IsFavorite = await _carService.IsFavoriteAsync(currentUserId, id);
        }

        // Fetch similar cars (same body type or brand, excluding current car)
        var allCars = await _carService.GetAllCarsAsync();
        var similarCars = allCars
            .Where(c => c.Id != id && (c.BodyType == car.BodyType || c.Brand == car.Brand))
            .Take(3)
            .Select(c => new CarCardViewModel
            {
                Id = c.Id, Brand = c.Brand, Model = c.Model, Year = c.Year,
                Price = c.Price, ImageUrls = c.ImageUrls, Mileage = c.Mileage,
                FuelType = c.FuelType, Transmission = c.Transmission, BodyType = c.BodyType,
                Color = c.Color, Condition = c.Condition, IsFeatured = c.IsFeatured,
                Description = c.Description, DateAdded = c.DateAdded, Features = c.Features, Status = c.Status
            }).ToList();

        ViewBag.SimilarCars = similarCars;

        return View(vm);
    }

    /// <summary>Side-by-side comparison page for up to 4 cars</summary>
    [HttpGet]
    public async Task<IActionResult> Compare(string? ids)
    {
        var carViewModels = new List<CarCardViewModel>();
        if (!string.IsNullOrEmpty(ids))
        {
            var guidIds = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(idStr => Guid.TryParse(idStr, out var g) ? g : Guid.Empty)
                             .Where(g => g != Guid.Empty)
                             .ToList();

            foreach (var guid in guidIds.Take(4))
            {
                var car = await _carService.GetCarByIdAsync(guid);
                if (car != null)
                {
                    carViewModels.Add(new CarCardViewModel
                    {
                        Id = car.Id, Brand = car.Brand, Model = car.Model, Year = car.Year,
                        Price = car.Price, ImageUrls = car.ImageUrls, Mileage = car.Mileage,
                        FuelType = car.FuelType, Transmission = car.Transmission, BodyType = car.BodyType,
                        Color = car.Color, Condition = car.Condition, IsFeatured = car.IsFeatured,
                        Description = car.Description, DateAdded = car.DateAdded, Features = car.Features, Status = car.Status
                    });
                }
            }
        }
        return View(carViewModels);
    }

    /// <summary>Post inquiry for a car listing (AJAX/Fetch API)</summary>
    [HttpPost]
    public async Task<IActionResult> ContactSeller([FromBody] InquiryInputModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Message) || string.IsNullOrWhiteSpace(model.BuyerName) || string.IsNullOrWhiteSpace(model.BuyerEmail) || string.IsNullOrWhiteSpace(model.Phone))
        {
            return BadRequest("Invalid inquiry data.");
        }

        var car = await _carService.GetCarByIdAsync(model.CarId);
        if (car == null)
        {
            return NotFound("Car listing not found.");
        }

        var buyerId = _userManager.GetUserId(User) ?? null;

        var inquiry = new Inquiry
        {
            Id = Guid.NewGuid(),
            UserId = buyerId,
            CarId = car.Id,
            Name = model.BuyerName,
            Email = model.BuyerEmail,
            Phone = model.Phone,
            Message = model.Message,
            InquiryDate = DateTime.UtcNow,
            Status = "Pending"
        };

        await _carService.AddInquiryAsync(inquiry);

        return Ok();
    }

    /// <summary>AJAX favorite toggling for authenticated users</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var car = await _carService.GetCarByIdAsync(id);
        if (car == null)
        {
            return NotFound("Vehicle not found.");
        }

        var isFav = await _carService.IsFavoriteAsync(userId, id);
        if (isFav)
        {
            await _carService.RemoveFavoriteAsync(userId, id);
            return Json(new { favorited = false });
        }
        else
        {
            var fav = new Favorite
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CarId = id,
                DateAdded = DateTime.UtcNow
            };
            await _carService.AddFavoriteAsync(fav);
            return Json(new { favorited = true });
        }
    }
}

public class InquiryInputModel
{
    public Guid CarId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerEmail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
