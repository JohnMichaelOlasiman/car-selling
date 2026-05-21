// ============================================================
// RevvUp.Web — HomeController
// Phase 3: Landing page with Featured + Recently Added cars
// ============================================================

using Microsoft.AspNetCore.Mvc;
using RevvUp.Application.Interfaces;
using RevvUp.Web.Models;

namespace RevvUp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICarService _carService;

    public HomeController(ILogger<HomeController> logger, ICarService carService)
    {
        _logger = logger;
        _carService = carService;
    }

    /// <summary>Landing page with featured + recent cars</summary>
    public async Task<IActionResult> Index()
    {
        var featured = await _carService.GetFeaturedCarsAsync();
        var all = await _carService.GetAllCarsAsync();

        ViewBag.FeaturedCars = featured.Take(4).Select(c => new CarCardViewModel
        {
            Id = c.Id, Make = c.Make, Model = c.Model, Year = c.Year,
            Price = c.Price, ImageUrl = c.ImageUrl, Mileage = c.Mileage,
            FuelType = c.FuelType, Transmission = c.Transmission, BodyType = c.BodyType,
            Color = c.Color, Location = c.Location, IsFeatured = c.IsFeatured,
            Description = c.Description, CreatedAt = c.CreatedAt
        }).ToList();

        ViewBag.RecentCars = all.Take(6).Select(c => new CarCardViewModel
        {
            Id = c.Id, Make = c.Make, Model = c.Model, Year = c.Year,
            Price = c.Price, ImageUrl = c.ImageUrl, Mileage = c.Mileage,
            FuelType = c.FuelType, Transmission = c.Transmission, BodyType = c.BodyType,
            Color = c.Color, Location = c.Location, IsFeatured = c.IsFeatured,
            Description = c.Description, CreatedAt = c.CreatedAt
        }).ToList();

        return View();
    }

    /// <summary>Error handler</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
