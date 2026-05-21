// ============================================================
// RevvUp.Web — DashboardController
// User dashboard with sidebar navigation
// Phase 2: My Listings, Saved Cars, Selling Dashboard (UI only)
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RevvUp.Core.Entities;
using RevvUp.Application.Interfaces;

namespace RevvUp.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICarService _carService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        UserManager<ApplicationUser> userManager,
        ICarService carService,
        ILogger<DashboardController> logger)
    {
        _userManager = userManager;
        _carService = carService;
        _logger = logger;
    }

    /// <summary>Dashboard overview</summary>
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserName"] = user?.DisplayName ?? "Racer";
        return View();
    }

    /// <summary>My car listings</summary>
    public async Task<IActionResult> MyListings()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserName"] = user?.DisplayName ?? "Racer";

        var cars = await _carService.GetAllCarsAsync();
        // Return active cars
        return View(cars.ToList());
    }

    /// <summary>Saved / favorited cars</summary>
    public async Task<IActionResult> SavedCars()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserName"] = user?.DisplayName ?? "Racer";

        // Fetch all cars to match liked IDs on client
        var cars = await _carService.GetAllCarsAsync();
        ViewBag.AllCarsJson = System.Text.Json.JsonSerializer.Serialize(cars.Select(c => new {
            id = c.Id,
            name = $"{c.Year} {c.Make} {c.Model}",
            price = $"₱{c.Price:N0}",
            image = c.ImageUrl,
            mileage = $"{c.Mileage:N0} km",
            transmission = c.Transmission,
            fuelType = c.FuelType,
            location = c.Location
        }));

        return View();
    }

    /// <summary>Create listing page (Sell flow)</summary>
    [HttpGet]
    public async Task<IActionResult> CreateListing()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserName"] = user?.DisplayName ?? "Racer";
        return View();
    }

    /// <summary>Post new listing</summary>
    [HttpPost]
    public async Task<IActionResult> CreateListing(string make, string model, int year, decimal price, int mileage, string fuelType, string transmission, string bodyType, string color, string location, string description, string imageUrl)
    {
        var newCar = new Car
        {
            Id = Guid.NewGuid(),
            Make = make,
            Model = model,
            Year = year,
            Price = price,
            Mileage = mileage,
            FuelType = fuelType,
            Transmission = transmission,
            BodyType = bodyType,
            Color = color,
            Location = location,
            Description = description,
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? "https://images.unsplash.com/photo-1617788138017-80ad40651399?w=800&q=80" : imageUrl,
            CreatedAt = DateTime.UtcNow,
            IsFeatured = false
        };

        await _carService.AddCarAsync(newCar);
        return RedirectToAction("MyListings");
    }

    /// <summary>Mark listing as sold</summary>
    [HttpPost]
    public async Task<IActionResult> MarkSold(Guid id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car != null)
        {
            // Set Description prefix as Sold indicator for in-memory repo
            car.Description = "[SOLD] " + car.Description;
            await _carService.UpdateCarAsync(car);
        }
        return RedirectToAction("MyListings");
    }

    /// <summary>Delete listing</summary>
    [HttpPost]
    public async Task<IActionResult> DeleteListing(Guid id)
    {
        await _carService.DeleteCarAsync(id);
        return RedirectToAction("MyListings");
    }

    /// <summary>Selling dashboard with analytics</summary>
    public async Task<IActionResult> Selling()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserName"] = user?.DisplayName ?? "Racer";
        return View();
    }
}
