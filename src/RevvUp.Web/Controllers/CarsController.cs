// ============================================================
// RevvUp.Web — CarsController
// Phase 3: Browse, Search, Filter, and Car Details
// ============================================================

using Microsoft.AspNetCore.Mvc;
using RevvUp.Application.Interfaces;
using RevvUp.Web.Models;

namespace RevvUp.Web.Controllers;

public class CarsController : Controller
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    /// <summary>Browse all cars with filters + pagination</summary>
    [HttpGet]
    public async Task<IActionResult> Browse(
        string? search, string? make, string? bodyType, string? fuelType,
        string? transmission, string? color, decimal? minPrice, decimal? maxPrice,
        int? minYear, int? maxYear, int? maxMileage, string sortBy = "newest", int page = 1)
    {
        var allCars = await _carService.GetAllCarsAsync();
        var carsList = allCars.ToList();

        // ── Build filter options from all data ──
        var vm = new BrowseViewModel
        {
            Search = search, Make = make, BodyType = bodyType, FuelType = fuelType,
            Transmission = transmission, Color = color, MinPrice = minPrice, MaxPrice = maxPrice,
            MinYear = minYear, MaxYear = maxYear, MaxMileage = maxMileage, SortBy = sortBy, Page = page,
            AvailableMakes = carsList.Select(c => c.Make).Distinct().OrderBy(x => x).ToList(),
            AvailableBodyTypes = carsList.Select(c => c.BodyType).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList(),
            AvailableFuelTypes = carsList.Select(c => c.FuelType).Distinct().OrderBy(x => x).ToList(),
            AvailableTransmissions = carsList.Select(c => c.Transmission).Distinct().OrderBy(x => x).ToList(),
            AvailableColors = carsList.Select(c => c.Color).Distinct().OrderBy(x => x).ToList()
        };

        // ── Apply filters ──
        var filtered = carsList.AsEnumerable();
        if (!string.IsNullOrEmpty(search))
            filtered = filtered.Where(c =>
                c.Make.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(make)) filtered = filtered.Where(c => c.Make == make);
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
            _ => filtered.OrderByDescending(c => c.CreatedAt)
        };

        var filteredList = filtered.ToList();
        vm.TotalCount = filteredList.Count;
        vm.Cars = filteredList.Skip((page - 1) * vm.PageSize).Take(vm.PageSize)
            .Select(c => new CarCardViewModel
            {
                Id = c.Id, Make = c.Make, Model = c.Model, Year = c.Year,
                Price = c.Price, ImageUrl = c.ImageUrl, Mileage = c.Mileage,
                FuelType = c.FuelType, Transmission = c.Transmission, BodyType = c.BodyType,
                Color = c.Color, Location = c.Location, IsFeatured = c.IsFeatured,
                Description = c.Description, CreatedAt = c.CreatedAt
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
            name = $"{c.Year} {c.Make} {c.Model}",
            price = $"₱{c.Price:N0}",
            image = c.ImageUrl,
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
            Id = car.Id, Make = car.Make, Model = car.Model, Year = car.Year,
            Price = car.Price, ImageUrl = car.ImageUrl, Mileage = car.Mileage,
            FuelType = car.FuelType, Transmission = car.Transmission, BodyType = car.BodyType,
            Color = car.Color, Location = car.Location, IsFeatured = car.IsFeatured,
            Description = car.Description, CreatedAt = car.CreatedAt
        };

        // Fetch similar cars (same body type or brand, excluding current car)
        var allCars = await _carService.GetAllCarsAsync();
        var similarCars = allCars
            .Where(c => c.Id != id && (c.BodyType == car.BodyType || c.Make == car.Make))
            .Take(3)
            .Select(c => new CarCardViewModel
            {
                Id = c.Id, Make = c.Make, Model = c.Model, Year = c.Year,
                Price = c.Price, ImageUrl = c.ImageUrl, Mileage = c.Mileage,
                FuelType = c.FuelType, Transmission = c.Transmission, BodyType = c.BodyType,
                Color = c.Color, Location = c.Location, IsFeatured = c.IsFeatured,
                Description = c.Description, CreatedAt = c.CreatedAt
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
                        Id = car.Id, Make = car.Make, Model = car.Model, Year = car.Year,
                        Price = car.Price, ImageUrl = car.ImageUrl, Mileage = car.Mileage,
                        FuelType = car.FuelType, Transmission = car.Transmission, BodyType = car.BodyType,
                        Color = car.Color, Location = car.Location, IsFeatured = car.IsFeatured,
                        Description = car.Description, CreatedAt = car.CreatedAt
                    });
                }
            }
        }
        return View(carViewModels);
    }
}
