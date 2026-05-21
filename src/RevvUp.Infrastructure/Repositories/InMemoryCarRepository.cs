// ============================================================
// RevvUp.Infrastructure — In-Memory Car Repository
// Phase 3: Expanded with 20+ diverse car listings for browsing
// ============================================================

using RevvUp.Core.Entities;
using RevvUp.Core.Interfaces;

namespace RevvUp.Infrastructure.Repositories;

public class InMemoryCarRepository : ICarRepository
{
    private readonly List<Car> _cars = new();

    public InMemoryCarRepository()
    {
        _cars.AddRange(new[]
        {
            new Car { Make = "Tesla", Model = "Model S Plaid", Year = 2024, Price = 3_500_000, Mileage = 5_200, FuelType = "Electric", Transmission = "Automatic", Color = "Midnight Black", BodyType = "Sedan", Location = "Metro Manila", IsFeatured = true, Description = "The fastest accelerating production car ever made. Tri-motor AWD with 1,020 hp.", ImageUrl = "https://images.unsplash.com/photo-1617788138017-80ad40651399?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Car { Make = "BMW", Model = "M4 Competition", Year = 2024, Price = 8_200_000, Mileage = 12_000, FuelType = "Gasoline", Transmission = "Automatic", Color = "Isle of Man Green", BodyType = "Coupe", Location = "Cebu City", IsFeatured = true, Description = "Pure driving pleasure with 503 horsepower of twin-turbo inline-6 fury.", ImageUrl = "https://images.unsplash.com/photo-1617814076367-b759c7d7e738?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Car { Make = "Mercedes-Benz", Model = "AMG GT 63", Year = 2023, Price = 12_500_000, Mileage = 8_500, FuelType = "Gasoline", Transmission = "Automatic", Color = "Obsidian Black", BodyType = "Coupe", Location = "Davao City", IsFeatured = true, Description = "Handcrafted AMG 4.0L V8 biturbo meets stunning design. One man, one engine.", ImageUrl = "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new Car { Make = "Porsche", Model = "911 GT3 RS", Year = 2024, Price = 18_900_000, Mileage = 2_100, FuelType = "Gasoline", Transmission = "Automatic", Color = "Racing Yellow", BodyType = "Coupe", Location = "Metro Manila", IsFeatured = true, Description = "Track-bred precision with 518 hp naturally aspirated flat-six. Swan-neck rear wing.", ImageUrl = "https://images.unsplash.com/photo-1614162692292-7ac56d7f7f1e?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Car { Make = "Toyota", Model = "GR Supra 3.0", Year = 2024, Price = 4_200_000, Mileage = 7_800, FuelType = "Gasoline", Transmission = "Manual", Color = "Renaissance Red", BodyType = "Coupe", Location = "Metro Manila", IsFeatured = true, Description = "BMW-sourced B58 inline-6 with 382 hp. Finally available with a 6-speed manual.", ImageUrl = "https://images.unsplash.com/photo-1632245889029-e406faaa34cd?w=800&q=80", CreatedAt = DateTime.UtcNow.AddHours(-6) },
            new Car { Make = "Honda", Model = "Civic Type R", Year = 2024, Price = 3_100_000, Mileage = 9_500, FuelType = "Gasoline", Transmission = "Manual", Color = "Championship White", BodyType = "Hatchback", Location = "Quezon City", Description = "The ultimate front-wheel-drive machine. 315 hp K20C1 turbo with rev-matching.", ImageUrl = "https://images.unsplash.com/photo-1679239872428-dec0ae958658?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new Car { Make = "Ford", Model = "Mustang GT", Year = 2024, Price = 3_800_000, Mileage = 15_000, FuelType = "Gasoline", Transmission = "Manual", Color = "Grabber Blue", BodyType = "Coupe", Location = "Metro Manila", Description = "5.0L Coyote V8 producing 480 hp. The quintessential American muscle car.", ImageUrl = "https://images.unsplash.com/photo-1584345604476-8ec5e12e42dd?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-7) },
            new Car { Make = "Audi", Model = "RS e-tron GT", Year = 2024, Price = 9_500_000, Mileage = 8_100, FuelType = "Electric", Transmission = "Automatic", Color = "Daytona Grey", BodyType = "Sedan", Location = "Makati", IsFeatured = true, Description = "646 hp dual-motor electric grand tourer. 0-100 in 3.3 seconds of silent fury.", ImageUrl = "https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Car { Make = "Lexus", Model = "LC 500", Year = 2024, Price = 7_900_000, Mileage = 3_800, FuelType = "Gasoline", Transmission = "Automatic", Color = "Structural Blue", BodyType = "Coupe", Location = "Taguig", Description = "5.0L V8 with 471 hp in the most beautiful car Lexus has ever made.", ImageUrl = "https://images.unsplash.com/photo-1549399542-7e3f8b79c341?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-4) },
            new Car { Make = "Subaru", Model = "WRX STI", Year = 2023, Price = 2_800_000, Mileage = 22_000, FuelType = "Gasoline", Transmission = "Manual", Color = "WR Blue Pearl", BodyType = "Sedan", Location = "Pampanga", Description = "EJ257 boxer turbo with 310 hp and symmetrical AWD. Rally legend on the road.", ImageUrl = "https://images.unsplash.com/photo-1580274455191-1c62238ce452?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new Car { Make = "Mazda", Model = "MX-5 Miata RF", Year = 2024, Price = 2_200_000, Mileage = 6_300, FuelType = "Gasoline", Transmission = "Manual", Color = "Soul Red Crystal", BodyType = "Convertible", Location = "Cebu City", Description = "The world's best-selling roadster. 2.0L Skyactiv-G with perfect weight balance.", ImageUrl = "https://images.unsplash.com/photo-1555215695-3004980ad54e?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new Car { Make = "Nissan", Model = "GT-R Nismo", Year = 2024, Price = 14_500_000, Mileage = 4_200, FuelType = "Gasoline", Transmission = "Automatic", Color = "Stealth Grey", BodyType = "Coupe", Location = "Metro Manila", IsFeatured = true, Description = "Godzilla with 600 hp twin-turbo VR38DETT. Hand-assembled by one Takumi craftsman.", ImageUrl = "https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=800&q=80", CreatedAt = DateTime.UtcNow.AddHours(-12) },
            new Car { Make = "Toyota", Model = "Land Cruiser 300", Year = 2024, Price = 5_200_000, Mileage = 18_000, FuelType = "Diesel", Transmission = "Automatic", Color = "Precious White", BodyType = "SUV", Location = "Davao City", Description = "The ultimate go-anywhere luxury SUV. 3.3L V6 twin-turbo diesel with TNGA-F.", ImageUrl = "https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-6) },
            new Car { Make = "Range Rover", Model = "Sport P530", Year = 2024, Price = 11_800_000, Mileage = 6_500, FuelType = "Gasoline", Transmission = "Automatic", Color = "Carpathian Grey", BodyType = "SUV", Location = "Makati", Description = "Twin-turbo V8 with 523 hp in the most desirable luxury SUV. Dynamic Air Suspension.", ImageUrl = "https://images.unsplash.com/photo-1606016159991-dfe4f2746ad5?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-4) },
            new Car { Make = "Hyundai", Model = "Ioniq 5 N", Year = 2025, Price = 3_900_000, Mileage = 1_200, FuelType = "Electric", Transmission = "Automatic", Color = "Performance Blue", BodyType = "Hatchback", Location = "Taguig", Description = "601 hp dual-motor electric hot hatch with simulated gear shifts and exhaust sounds.", ImageUrl = "https://images.unsplash.com/photo-1593941707882-a5bba14938c7?w=800&q=80", CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new Car { Make = "Volkswagen", Model = "Golf R", Year = 2024, Price = 3_400_000, Mileage = 11_000, FuelType = "Gasoline", Transmission = "Automatic", Color = "Lapiz Blue", BodyType = "Hatchback", Location = "Quezon City", Description = "EA888 2.0T with 315 hp and 4MOTION AWD. The refined hot hatch benchmark.", ImageUrl = "https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-8) },
            new Car { Make = "Chevrolet", Model = "Corvette Z06", Year = 2024, Price = 9_800_000, Mileage = 3_500, FuelType = "Gasoline", Transmission = "Automatic", Color = "Torch Red", BodyType = "Coupe", Location = "Metro Manila", Description = "5.5L flat-plane crank V8 screaming to 8,600 RPM with 670 hp. Mid-engine masterpiece.", ImageUrl = "https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new Car { Make = "Lamborghini", Model = "Huracán STO", Year = 2023, Price = 28_000_000, Mileage = 1_800, FuelType = "Gasoline", Transmission = "Automatic", Color = "Arancio Borealis", BodyType = "Coupe", Location = "Metro Manila", IsFeatured = true, Description = "Track-focused V10 with 631 hp. Super Trofeo Omologata — built for the racetrack.", ImageUrl = "https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Car { Make = "Volvo", Model = "XC90 Recharge", Year = 2024, Price = 5_800_000, Mileage = 9_000, FuelType = "Hybrid", Transmission = "Automatic", Color = "Thunder Grey", BodyType = "SUV", Location = "Pasig", Description = "T8 plug-in hybrid with 455 hp combined. Scandinavian luxury meets sustainability.", ImageUrl = "https://images.unsplash.com/photo-1606016159991-dfe4f2746ad5?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-9) },
            new Car { Make = "Jeep", Model = "Wrangler Rubicon", Year = 2024, Price = 4_100_000, Mileage = 20_000, FuelType = "Gasoline", Transmission = "Manual", Color = "Sarge Green", BodyType = "SUV", Location = "Pampanga", Description = "The icon reborn. 3.6L Pentastar V6 with Dana 44 axles and electronic lockers.", ImageUrl = "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?w=800&q=80", CreatedAt = DateTime.UtcNow.AddDays(-11) },
            new Car { Make = "Ferrari", Model = "296 GTB", Year = 2024, Price = 22_500_000, Mileage = 2_500, FuelType = "Hybrid", Transmission = "Automatic", Color = "Rosso Corsa", BodyType = "Coupe", Location = "Metro Manila", IsFeatured = true, Description = "V6 hybrid with 819 hp. The future of Ferrari. Assetto Fiorano package included.", ImageUrl = "https://images.unsplash.com/photo-1583121274602-3e2820c69888?w=800&q=80", CreatedAt = DateTime.UtcNow.AddHours(-8) },
        });
    }

    public Task<IEnumerable<Car>> GetAllAsync()
        => Task.FromResult<IEnumerable<Car>>(_cars.Where(c => !c.IsSold).OrderByDescending(c => c.CreatedAt));

    public Task<Car?> GetByIdAsync(Guid id)
        => Task.FromResult(_cars.FirstOrDefault(c => c.Id == id));

    public Task<IEnumerable<Car>> SearchAsync(string query)
    {
        var results = _cars.Where(c => !c.IsSold && (
            c.Make.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Model.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.BodyType.Contains(query, StringComparison.OrdinalIgnoreCase)));
        return Task.FromResult(results);
    }

    public Task<IEnumerable<Car>> GetFeaturedAsync()
        => Task.FromResult<IEnumerable<Car>>(_cars.Where(c => c.IsFeatured && !c.IsSold).OrderByDescending(c => c.CreatedAt));

    public Task AddAsync(Car car) { _cars.Add(car); return Task.CompletedTask; }
    public Task UpdateAsync(Car car) { var i = _cars.FindIndex(c => c.Id == car.Id); if (i >= 0) _cars[i] = car; return Task.CompletedTask; }
    public Task DeleteAsync(Guid id) { _cars.RemoveAll(c => c.Id == id); return Task.CompletedTask; }
}
