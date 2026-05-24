// ============================================================
// RevvUp.Web — HomeController
// Phase 3: Landing page with Featured + Recently Added cars
// ============================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RevvUp.Application.Interfaces;
using RevvUp.Core.Entities;
using RevvUp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevvUp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICarService _carService;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ICarService carService, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _carService = carService;
        _userManager = userManager;
    }

    /// <summary>Landing page with featured + recent cars</summary>
    public async Task<IActionResult> Index()
    {
        var featured = await _carService.GetFeaturedCarsAsync();
        var all = await _carService.GetAllCarsAsync();

        var userId = User.Identity?.IsAuthenticated == true ? _userManager.GetUserId(User) : null;
        var favoriteCarIds = new HashSet<Guid>();
        if (!string.IsNullOrEmpty(userId))
        {
            var favorites = await _carService.GetFavoritesByUserIdAsync(userId);
            favoriteCarIds = new HashSet<Guid>(favorites.Select(f => f.CarId));
        }

        ViewBag.FeaturedCars = featured.Take(4).Select(c => new CarCardViewModel
        {
            Id = c.Id,
            SellerId = c.SellerId,
            IsFavoritedByCurrentUser = favoriteCarIds.Contains(c.Id),
            Brand = c.Brand,
            Model = c.Model,
            Year = c.Year,
            Price = c.Price,
            ImageUrls = c.ImageUrls,
            Mileage = c.Mileage,
            FuelType = c.FuelType,
            Transmission = c.Transmission,
            BodyType = c.BodyType,
            Color = c.Color,
            IsFeatured = c.IsFeatured,
            Description = c.Description,
            DateAdded = c.DateAdded,
            Condition = c.Condition,
            Engine = c.Engine,
            Status = c.Status
        }).ToList();

        ViewBag.RecentCars = all.OrderByDescending(c => c.DateAdded).Take(8).Select(c => new CarCardViewModel
        {
            Id = c.Id,
            SellerId = c.SellerId,
            IsFavoritedByCurrentUser = favoriteCarIds.Contains(c.Id),
            Brand = c.Brand,
            Model = c.Model,
            Year = c.Year,
            Price = c.Price,
            ImageUrls = c.ImageUrls,
            Mileage = c.Mileage,
            FuelType = c.FuelType,
            Transmission = c.Transmission,
            BodyType = c.BodyType,
            Color = c.Color,
            IsFeatured = c.IsFeatured,
            Description = c.Description,
            DateAdded = c.DateAdded,
            Condition = c.Condition,
            Engine = c.Engine,
            Status = c.Status
        }).ToList();

        return View();
    }

    /// <summary>About + How It Works combined page</summary>
    public IActionResult About()
    {
        return View();
    }

    /// <summary>Error handler</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
