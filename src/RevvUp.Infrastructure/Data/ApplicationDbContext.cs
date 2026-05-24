// ============================================================
// RevvUp.Infrastructure — Application DbContext
// EF Core + ASP.NET Identity database context
// ============================================================

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RevvUp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevvUp.Infrastructure.Data;

/// <summary>
/// EF Core DbContext with ASP.NET Identity support.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <summary>Curated premium car listings table</summary>
    public DbSet<Car> Cars => Set<Car>();

    /// <summary>Buyer inquiries table</summary>
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();

    /// <summary>Inquiry chat thread messages table</summary>
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    /// <summary>Buyer saved favorites table</summary>
    public DbSet<Favorite> Favorites => Set<Favorite>();

    /// <summary>User notifications table</summary>
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Car entity configuration ──
        builder.Entity<Car>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Brand).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Model).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Price).HasColumnType("decimal(18,2)");
            entity.Property(c => c.Description).HasMaxLength(2000);
            entity.Property(c => c.Status).HasMaxLength(50);
            entity.Property(c => c.Condition).HasMaxLength(50);
            entity.Property(c => c.Engine).HasMaxLength(150);
        });

        // ── Inquiry entity configuration ──
        builder.Entity<Inquiry>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Name).IsRequired().HasMaxLength(100);
            entity.Property(i => i.Email).IsRequired().HasMaxLength(256);
            entity.Property(i => i.Phone).IsRequired().HasMaxLength(50);
            entity.Property(i => i.Message).IsRequired().HasMaxLength(2000);
            entity.Property(i => i.UserId).HasMaxLength(450);
            entity.Property(i => i.Status).HasMaxLength(50);
        });

        // ── ChatMessage entity configuration ──
        builder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.SenderId).IsRequired().HasMaxLength(450);
            entity.Property(m => m.SenderName).IsRequired().HasMaxLength(100);
            entity.Property(m => m.MessageText).IsRequired().HasMaxLength(2000);
            entity.Property(m => m.InquiryId).IsRequired();
        });

        // ── Favorite entity configuration ──
        builder.Entity<Favorite>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.UserId).IsRequired().HasMaxLength(450);
            entity.Property(f => f.CarId).IsRequired();
            entity.HasIndex(f => new { f.UserId, f.CarId }).IsUnique();
        });

        // ── Notification entity configuration ──
        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.UserId).IsRequired().HasMaxLength(450);
            entity.Property(n => n.Message).IsRequired().HasMaxLength(500);
            entity.Property(n => n.Link).IsRequired().HasMaxLength(250);
            entity.Property(n => n.Type).IsRequired().HasMaxLength(50);
            entity.HasOne(n => n.User)
                  .WithMany()
                  .HasForeignKey(n => n.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Rename Identity tables for cleaner schema ──
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.DisplayName).HasMaxLength(100);
            entity.Property(u => u.Bio).HasMaxLength(500);
            entity.Property(u => u.Location).HasMaxLength(200);
            entity.Property(u => u.SellerDisplayName).HasMaxLength(100);
            entity.Property(u => u.SellerPhoneNumber).HasMaxLength(50);
            entity.Property(u => u.SellerLocation).HasMaxLength(200);
            entity.Property(u => u.SellerBio).HasMaxLength(500);
        });

        // ── SEED DATA GENERATION: exactly 52 premium cars ──
        var seedSpecs = new (string Brand, string Model, int Year, decimal Price, int Mileage, string BodyType, string FuelType, string Transmission, string Color, string Condition, bool IsFeatured, string Features, string Description)[]
        {
            ("Toyota", "Camry Hybrid", 2023, 2380000m, 12000, "Sedan", "Hybrid", "CVT", "White", "Excellent", true, "Leather Seats, Sunroof, Backup Camera, Adaptive Cruise Control, Heated Seats, Apple CarPlay", "A pristine, highly efficient Toyota Camry Hybrid. Excellent fuel economy paired with legendary reliability and smooth ride comfort. The interior is spacious and well-appointed, making it perfect for daily commutes or long road trips."),
            ("Toyota", "RAV4 AWD", 2022, 2150000m, 24000, "SUV", "Gasoline", "Automatic", "Silver", "Excellent", false, "Backup Camera, Bluetooth, Heated Seats, Keyless Entry, Lane Keeping Assist, Apple CarPlay", "This Toyota RAV4 AWD offers versatile cargo space and stellar all-weather traction. Perfect for weekend getaways and active lifestyles. Featuring a suite of advanced safety features and a spacious cabin, it is ready for your next adventure."),
            ("Toyota", "GR Supra", 2024, 5390000m, 5200, "Coupe", "Gasoline", "Manual", "Red", "Excellent", true, "Leather Seats, Navigation System, Backup Camera, Bluetooth, Apple CarPlay, Premium Sound System", "An exhilarating Toyota GR Supra with a track-inspired 6-speed manual transmission. Finished in Renaissance Red with low mileage and excellent care. The turbocharged engine delivers jaw-dropping acceleration while the premium cabin keeps you firmly in place."),
            ("Toyota", "Corolla LE", 2020, 1050000m, 45000, "Sedan", "Gasoline", "CVT", "Gray", "Good", false, "Backup Camera, Bluetooth, Keyless Entry, Lane Keeping Assist, Apple CarPlay, Android Auto", "A budget-friendly and highly dependable Toyota Corolla. Finished in Magnetic Gray, this sedan has been well-maintained and offers excellent highway fuel economy. Features active safety assists and comfortable seating for five passengers."),
            ("Toyota", "Land Cruiser", 2021, 6790000m, 31000, "SUV", "Gasoline", "Automatic", "Black", "Excellent", true, "Leather Seats, Sunroof, Navigation System, Heated Seats, Keyless Entry, Remote Start, Premium Sound System", "The legendary Toyota Land Cruiser. Uncompromising off-road engineering meets flagship luxury interior. This black beast is powered by a smooth V8 engine and features crawl control, kinetic dynamic suspension, and high-fidelity passenger entertainment systems."),
            
            ("Honda", "Civic Type R", 2024, 3880000m, 8100, "Hatchback", "Gasoline", "Manual", "White", "Excellent", true, "Navigation System, Backup Camera, Bluetooth, Heated Seats, Apple CarPlay, Remote Start, Lane Keeping Assist", "The pinnacle of front-wheel-drive performance, this Honda Civic Type R is finished in Championship White. Offers blistering track capability balanced with everyday hatchback versatility. Features carbon fiber accents and a highly responsive manual shifter."),
            ("Honda", "Accord Sport", 2021, 1890000m, 32000, "Sedan", "Gasoline", "CVT", "Black", "Good", false, "Sunroof, Backup Camera, Bluetooth, Heated Seats, Android Auto, Keyless Entry, Lane Keeping Assist", "A beautiful Honda Accord Sport in modern Crystal Black. Features a responsive turbocharged engine, premium black wheels, and a spacious cockpit loaded with modern driver assistance features. Mechanically perfect and extremely clean."),
            ("Honda", "CR-V Hybrid", 2023, 2290000m, 15000, "SUV", "Hybrid", "CVT", "Blue", "Excellent", false, "Sunroof, Backup Camera, Adaptive Cruise Control, Heated Seats, Apple CarPlay, Android Auto, Keyless Entry", "This CR-V Hybrid delivers exceptional efficiency and daily utility. With intelligent all-wheel drive, spacious rear cargo space, and active noise cancellation, it offers a tranquil ride under all conditions. Perfect for families looking to cut fuel costs."),
            ("Honda", "Civic Sedan", 2019, 1180000m, 58000, "Sedan", "Gasoline", "CVT", "Silver", "Good", false, "Backup Camera, Bluetooth, Android Auto, Apple CarPlay, Keyless Entry, Bluetooth", "A sleek Lunar Silver Honda Civic. A practical, low-maintenance sedan that handles daily commuting gracefully. Features an efficient four-cylinder engine, sharp steering feel, and modern infotainment. Perfect first car or economical commuter."),
            
            ("BMW", "M4 Competition", 2023, 9790000m, 9300, "Coupe", "Gasoline", "Automatic", "Green", "Excellent", true, "Leather Seats, Navigation System, Adaptive Cruise Control, Heated Seats, Premium Sound System, Apple CarPlay", "Finished in Isle of Man Green over carbon bucket seats, this BMW M4 Competition is an absolute masterpiece. Driven by a twin-turbo inline-6 putting down 503 horsepower. Comes loaded with track telemetry, advanced adaptive suspension, and laser headlights."),
            ("BMW", "330i xDrive", 2021, 3890000m, 29000, "Sedan", "Gasoline", "Automatic", "Blue", "Excellent", false, "Sunroof, Navigation System, Backup Camera, Heated Seats, Apple CarPlay, Keyless Entry, Premium Sound System", "The benchmark sports sedan. Finished in Phytonic Blue Metallic, this 330i offers an ideal balance of sharp handling, turbo performance, and digital luxury. Standard equipped with a live cockpit display, ambient lighting, and driving aids."),
            ("BMW", "X5 xDrive40i", 2022, 6690000m, 22000, "SUV", "Hybrid", "Automatic", "Black", "Excellent", false, "Leather Seats, Sunroof, Navigation System, Heated Seats, Remote Start, Apple CarPlay, Android Auto", "This luxury BMW X5 midsize SUV is finished in Sapphire Black. Blends high-performance mild-hybrid engineering with a versatile, executive-class interior. Packed with premium acoustic glass, panoramic roof, and adaptive air suspension."),
            ("BMW", "540i M Sport", 2020, 4890000m, 42000, "Sedan", "Gasoline", "Automatic", "Gray", "Good", false, "Leather Seats, Sunroof, Backup Camera, Adaptive Cruise Control, Heated Seats, Keyless Entry", "A sophisticated BMW 540i M Sport. Offers executive-level legroom, a smooth turbocharged inline-6, and precise cornering capability. Well-maintained body and fresh tires make this gray sedan an exceptional value for business professionals."),
            
            ("Mercedes-Benz", "C300", 2022, 3990000m, 18500, "Sedan", "Gasoline", "Automatic", "Silver", "Excellent", false, "Sunroof, Backup Camera, Bluetooth, Heated Seats, Apple CarPlay, Keyless Entry, Lane Keeping Assist", "Redesigned Mercedes-Benz C300 in Iridium Silver. Inspired by the flagship S-Class, it introduces a vertical portrait touchscreen, customized multi-zone ambient lighting, and a fuel-efficient mild-hybrid system. Experience high-end luxury at a reasonable cost."),
            ("Mercedes-Benz", "AMG GT 63", 2024, 15890000m, 4500, "Coupe", "Gasoline", "Automatic", "Black", "Excellent", true, "Leather Seats, Navigation System, Heated Seats, Remote Start, Premium Sound System, Apple CarPlay", "An astonishing AMG GT 63 4-door Coupe. Powered by a handcrafted AMG 4.0L V8 Biturbo engine. Outfitted with aggressive carbon packages, active rear-axle steering, and premium sound systems. Pristine condition with absolute show-stopping curb appeal."),
            ("Mercedes-Benz", "GLE 450", 2023, 7290000m, 14000, "SUV", "Hybrid", "Automatic", "White", "Excellent", false, "Leather Seats, Sunroof, Navigation System, Heated Seats, Remote Start, Apple CarPlay, Premium Sound System", "This GLE 450 SUV combines room for five with state-of-the-art mild hybrid propulsion. The Polar White exterior contrasts beautifully with the premium espresso interior. Features standard panoramic roof, premium driver assistance, and dynamic dampening."),
            ("Mercedes-Benz", "E450 All-Terrain", 2021, 5290000m, 35000, "Wagon", "Gasoline", "Automatic", "Gray", "Good", false, "Leather Seats, Sunroof, Navigation System, Backup Camera, Heated Seats, Premium Sound System", "The refined executive Mercedes E450 All-Terrain luxury wagon. Featuring variable ride height, standard 4MATIC all-wheel drive, and massive rear luggage capacity. Finished in Selenite Gray. The perfect alternative to bulky family SUVs."),
            
            ("Audi", "RS e-tron GT", 2024, 10400000m, 6200, "Sedan", "Electric", "Automatic", "Gray", "Excellent", true, "Leather Seats, Navigation System, Backup Camera, Heated Seats, Premium Sound System, Apple CarPlay", "The future is electric with this breathtaking Audi RS e-tron GT. Dual-motor quattro system launching 637 horsepower in boost mode. Beautiful Daytona Gray paint, carbon fiber roof, and high-performance ceramic brakes. Extremely rapid charging capacity."),
            ("Audi", "A4 S-Line", 2021, 3290000m, 33000, "Sedan", "Gasoline", "Automatic", "Blue", "Good", false, "Sunroof, Backup Camera, Bluetooth, Heated Seats, Apple CarPlay, Android Auto, Keyless Entry", "This Navarra Blue Audi A4 S-Line offers standard quattro all-wheel drive and a sporty aesthetic package. Excellent interior materials with standard virtual cockpit. A tight, responsive handling sport sedan that makes every commute pleasurable."),
            ("Audi", "Q7 Premium", 2022, 6190000m, 26000, "SUV", "Gasoline", "Automatic", "Black", "Excellent", false, "Leather Seats, Sunroof, Navigation System, Heated Seats, Remote Start, Apple CarPlay, Android Auto", "A magnificent 3-row Audi Q7 SUV. Offers maximum passenger comfort, a quiet cabin environment, and advanced towing capabilities. The black paint has a deep mirror shine. Serviced regularly by official dealerships."),
            ("Audi", "A6 Allroad", 2020, 4990000m, 48000, "Wagon", "Gasoline", "Automatic", "Silver", "Good", false, "Leather Seats, Sunroof, Navigation System, Heated Seats, Premium Sound System, Apple CarPlay", "Unique Audi A6 Allroad premium wagon. Built to tackle dirt roads and gravel with adaptive air suspension, while offering top-tier highway refinement. Features high-grade valcona leather and executive wooden dashboard inserts."),
            
            ("Tesla", "Model 3 LR", 2023, 3690000m, 11000, "Sedan", "Electric", "Automatic", "Blue", "Excellent", false, "Sunroof, Navigation System, Backup Camera, Heated Seats, Premium Sound System, Remote Start", "A brilliant Deep Blue Metallic Tesla Model 3 Long Range. Standard dual-motor AWD delivering instant torque, long electric range, and access to Tesla's global Supercharger network. Minimalist black interior in perfect showroom condition."),
            ("Tesla", "Model Y Performance", 2022, 4290000m, 19000, "SUV", "Electric", "Automatic", "White", "Excellent", false, "Sunroof, Navigation System, Backup Camera, Heated Seats, Premium Sound System, Remote Start, Keyless Entry", "A fast, practical white Tesla Model Y Performance. Featuring larger sport wheels, lowered suspension, and blistering acceleration. Spacious hatchback layout offering plenty of utility and top-tier safety scores. Zero emissions driving."),
            ("Tesla", "Model S Plaid", 2023, 8590000m, 7100, "Sedan", "Electric", "Automatic", "Black", "Excellent", true, "Leather Seats, Sunroof, Navigation System, Heated Seats, Remote Start, Premium Sound System", "An absolute drag-strip champion. Tri-motor powertrain putting out 1,020 horsepower. Matte black exterior wrap, standard yoke steering wheel, and triple-zone executive screens. Accelerates faster than supercars while carrying the whole family."),
            
            ("Ford", "Mustang GT", 2024, 4490000m, 6000, "Coupe", "Gasoline", "Manual", "Red", "Excellent", true, "Leather Seats, Navigation System, Backup Camera, Heated Seats, Premium Sound System, Apple CarPlay", "Redesigned seventh-generation Ford Mustang GT Premium. Finished in Race Red, this fastback is powered by the legendary 5.0L Coyote V8 engine mated to a 6-speed manual. Features massive digital displays and customizable exhaust notes."),
            ("Ford", "Explorer XLT", 2022, 3190000m, 28000, "SUV", "Gasoline", "Automatic", "Gray", "Good", false, "Backup Camera, Bluetooth, Heated Seats, Keyless Entry, Lane Keeping Assist, Android Auto", "This Ford Explorer XLT offers comfortable seating for seven passengers. Features an efficient EcoBoost engine, terrain management controls, and modern driver assistance. A dependable and rugged midsize family cruiser."),
            ("Ford", "Bronco Outer Banks", 2023, 4590000m, 13000, "SUV", "Gasoline", "Automatic", "Green", "Excellent", false, "Sunroof, Backup Camera, Heated Seats, Keyless Entry, Remote Start, Apple CarPlay, Android Auto", "Ready for extreme off-road trails and open-air fun. This Ford Bronco Outer Banks is finished in Eruption Green Metallic with a matching modular hardtop. Features a high-clearance suspension package and intuitive trail control modes."),
            ("Ford", "F-150 Lariat", 2021, 3390000m, 39000, "Truck", "Gasoline", "Automatic", "Black", "Good", false, "Leather Seats, Navigation System, Backup Camera, Heated Seats, Premium Sound System, Remote Start", "The king of utility. This Ford F-150 Lariat features a spacious SuperCrew cabin, a durable bed liner, and heavy-duty towing packages. Powered by a twin-turbo EcoBoost V6 offering massive torque. Excellent condition inside and out."),
            
            ("Hyundai", "Ioniq 5 N", 2024, 4690000m, 5500, "Hatchback", "Electric", "Automatic", "Blue", "Excellent", true, "Leather Seats, Navigation System, Backup Camera, Heated Seats, Premium Sound System, Apple CarPlay", "The revolutionary Hyundai Ioniq 5 N. Delivers 641 horsepower of twin-motor performance. Programmed with synthetic gear shifts and drift modes. Finished in Performance Blue, this hot hatch is highly praised by automotive journalists worldwide."),
            ("Hyundai", "Elantra N", 2023, 2890000m, 10200, "Sedan", "Gasoline", "Manual", "Black", "Excellent", false, "Backup Camera, Heated Seats, Keyless Entry, Apple CarPlay, Android Auto, Lane Keeping Assist", "A track-ready street warrior. The Hyundai Elantra N is finished in Phantom Black and equipped with a 6-speed manual. Featuring an active valved exhaust system that crackles, electronic limited-slip differential, and heavy-duty brakes."),
            ("Hyundai", "Tucson Hybrid", 2022, 1990000m, 29000, "SUV", "Hybrid", "Automatic", "Silver", "Good", false, "Backup Camera, Bluetooth, Heated Seats, Keyless Entry, Lane Keeping Assist, Android Auto", "A handsome and highly efficient Hyundai Tucson Hybrid. Features a futuristic front lighting signature, comfortable seating, and excellent fuel mileage. Ideal SUV for city families seeking style and economy."),
            ("Hyundai", "Palisade Calligraphy", 2023, 3990000m, 16000, "SUV", "Gasoline", "Automatic", "White", "Excellent", false, "Leather Seats, Sunroof, Navigation System, Heated Seats, Remote Start, Premium Sound System, Apple CarPlay", "Flagship Hyundai Palisade in top-tier Calligraphy trim. Offers executive-level captain's chairs, a premium suede headliner, digital rearview mirror, and an advanced all-wheel-drive system. Finished in pristine Hyper White."),
            
            ("Mazda", "CX-5 Turbo", 2023, 2390000m, 14500, "SUV", "Gasoline", "Automatic", "Red", "Excellent", false, "Leather Seats, Sunroof, Backup Camera, Heated Seats, Apple CarPlay, Keyless Entry, Remote Start", "Finished in signature Soul Red Crystal paint, this Mazda CX-5 Turbo offers near-luxury refinement and engaging driving dynamics. Standard equipped with high-grade black leather, premium HUD, and predictive i-ACTIV AWD."),
            ("Mazda 3", "Premium", 2021, 1590000m, 36000, "Sedan", "Gasoline", "Automatic", "Gray", "Good", false, "Sunroof, Backup Camera, Bluetooth, Heated Seats, Apple CarPlay, Keyless Entry, Lane Keeping Assist", "A gorgeous Machine Gray Mazda 3 Sedan. Features an award-winning organic exterior design and a silent, driver-centric premium cockpit. Equipped with active safety radars and a smooth, efficient naturally aspirated engine."),
            ("Mazda", "MX-5 Miata", 2023, 2290000m, 8000, "Convertible", "Gasoline", "Manual", "Blue", "Excellent", false, "Backup Camera, Heated Seats, Apple CarPlay, Android Auto, Keyless Entry, Bluetooth", "Pure analog joy. This Deep Crystal Blue Miata convertible features a crisp 6-speed manual gearbox, a perfectly balanced rear-wheel-drive chassis, and a quick manual soft top. A blast to drive on winding mountain roads."),
            
            ("Nissan", "GT-R Premium", 2020, 9890000m, 15200, "Coupe", "Gasoline", "Automatic", "Silver", "Excellent", true, "Leather Seats, Navigation System, Backup Camera, Premium Sound System, Keyless Entry, Heated Seats", "The legendary 'Godzilla'. This Ultimate Silver Nissan GT-R is powered by a hand-assembled twin-turbo V6 pushing 565 horsepower. Standard all-wheel-drive grip allows face-melting acceleration. Meticulously cared for with comprehensive dealer logs."),
            ("Nissan", "Altima SR", 2021, 1690000m, 41000, "Sedan", "Gasoline", "CVT", "Black", "Good", false, "Backup Camera, Bluetooth, Heated Seats, Keyless Entry, Apple CarPlay, Android Auto", "A sharp Nissan Altima SR in Super Black. Features sport-tuned suspension, steering wheel paddle shifters, and highly comfortable zero-gravity seats. Offers stellar highway cruising range and daily dependability."),
            ("Nissan", "Rogue SV", 2022, 1990000m, 27000, "SUV", "Gasoline", "CVT", "White", "Good", false, "Backup Camera, Bluetooth, Heated Seats, Keyless Entry, Apple CarPlay, Lane Keeping Assist", "This white Nissan Rogue SV features a spacious cabin, exceptional cargo flexibility with divide-n-hide shelves, and intelligent driver safety assists. Ideal for running daily family errands or long weekend trips."),
            
            ("Porsche", "911 Carrera S", 2022, 11990000m, 9800, "Coupe", "Gasoline", "Automatic", "Gray", "Excellent", true, "Leather Seats, Sunroof, Navigation System, Heated Seats, Premium Sound System, Apple CarPlay", "Finished in stunning Agate Gray Metallic, this Porsche 911 Carrera S represents the absolute benchmark of sports cars. Features standard active suspension, carbon interior packages, and a lightning-fast PDK gearbox. Masterful road dynamics."),
            ("Porsche", "Cayenne Coupe", 2021, 8890000m, 23000, "SUV", "Gasoline", "Automatic", "Black", "Excellent", false, "Leather Seats, Sunroof, Navigation System, Heated Seats, Premium Sound System, Apple CarPlay, Remote Start", "A sportier take on the classic luxury SUV. This Porsche Cayenne Coupe is finished in Jet Black Metallic. Delivers sportscar-like handling combined with family utility. Equipped with premium chronos, air suspension, and active spoiler."),
            ("Porsche", "Taycan 4S", 2023, 9990000m, 11500, "Sedan", "Electric", "Automatic", "Red", "Excellent", false, "Leather Seats, Navigation System, Backup Camera, Heated Seats, Premium Sound System, Apple CarPlay", "This all-electric Porsche Taycan 4S is finished in Carmine Red. Standard 800V architecture allows rapid charging, while dual motors deliver instantaneous, neck-snapping torque. Exceptional chassis tuning that honors the Porsche crest."),
            
            ("Lexus", "LC 500", 2023, 10290000m, 5100, "Coupe", "Gasoline", "Automatic", "Yellow", "Excellent", true, "Leather Seats, Navigation System, Heated Seats, Premium Sound System, Keyless Entry, Apple CarPlay", "Finished in breathtaking Flare Yellow over tan alcantara, this Lexus LC 500 is a rolling piece of art. Powered by a glorious naturally aspirated 5.0L V8 that makes a thrilling mechanical sound. Incredible hand-stitched interior luxury."),
            ("Lexus", "RX 350 F Sport", 2022, 5190000m, 21000, "SUV", "Gasoline", "Automatic", "White", "Excellent", false, "Leather Seats, Sunroof, Navigation System, Heated Seats, Premium Sound System, Apple CarPlay, Remote Start", "Ultra-smooth and stylish Lexus RX 350 F Sport. Finished in Ultra White with a bold dark grille. The ride quality is exceptionally quiet, and the interior materials are built to survive decades. High-value luxury family SUV."),
            ("Lexus", "ES 300h", 2021, 3890000m, 33000, "Sedan", "Hybrid", "CVT", "Silver", "Good", false, "Sunroof, Backup Camera, Heated Seats, Apple CarPlay, Android Auto, Keyless Entry, Lane Keeping Assist", "An incredibly reliable and comfortable Lexus ES 300h hybrid sedan. Outstanding fuel efficiency matched with whisper-quiet cabin acoustics. Serviced exclusively at Lexus dealerships. A perfect long-distance executive cruiser."),
            
            ("Chevrolet", "Corvette Z06", 2023, 12990000m, 3200, "Coupe", "Gasoline", "Automatic", "Red", "Excellent", true, "Leather Seats, Navigation System, Heated Seats, Premium Sound System, Keyless Entry, Apple CarPlay", "A mid-engine supercar in Torch Red. Powered by a flat-plane crank 5.5L V8 that screams to an 8600 RPM redline. Loaded with high-performance carbon aero, magnetic ride control, and executive carbon cabin finishes."),
            ("Chevrolet", "Silverado 1500", 2021, 3890000m, 42000, "Truck", "Gasoline", "Automatic", "Gray", "Good", false, "Backup Camera, Bluetooth, Heated Seats, Keyless Entry, Remote Start, Apple CarPlay, Android Auto", "This heavy-duty Chevrolet Silverado 1500 RST is finished in Satin Steel Metallic. Equipped with a strong V8 engine, automatic locking differential, and high capacity bed. A bulletproof workhorse ready for heavy utility."),
            ("Chevrolet", "Equinox Premier", 2020, 1490000m, 51000, "SUV", "Gasoline", "Automatic", "Blue", "Good", false, "Backup Camera, Bluetooth, Heated Seats, Keyless Entry, Lane Keeping Assist, Android Auto", "A reliable and affordable blue Chevrolet Equinox. Fits five comfortably with dual-zone climate, active safety radars, and power liftgate. Ideal choice for budget-conscious families looking for compact utility."),
            
            ("Volkswagen", "Golf R", 2023, 3890000m, 12000, "Hatchback", "Gasoline", "Automatic", "Blue", "Excellent", false, "Sunroof, Navigation System, Heated Seats, Premium Sound System, Apple CarPlay, Lane Keeping Assist", "The ultimate hot hatch benchmark. Finished in Lapiz Blue Metallic, this Golf R channels 315 horsepower through a highly advanced torque-vectoring AWD system. Incredible daily drivability balanced with track-slaying dynamics."),
            ("Volkswagen", "Tiguan SEL", 2022, 2490000m, 25000, "SUV", "Gasoline", "Automatic", "White", "Good", false, "Sunroof, Backup Camera, Heated Seats, Keyless Entry, Lane Keeping Assist, Apple CarPlay, Android Auto", "A spacious, European-tuned compact SUV. Finished in Pure White, this Tiguan SEL offers a fully digital cockpit, premium leatherette upholstery, and a panoramic sunroof. Rides smoothly with secure, stable handling."),
            
            ("Subaru", "WRX STI", 2021, 2990000m, 28000, "Sedan", "Gasoline", "Manual", "Blue", "Excellent", false, "Backup Camera, Bluetooth, Heated Seats, Apple CarPlay, Keyless Entry, Premium Sound System", "The legendary rally champion. Finished in iconic WR Blue Pearl, this WRX STI features a high-boost turbocharged boxer engine, symmetrical AWD, and a massive rear spoiler. 100% stock and kept in amazing garage-stored condition."),
            ("Subaru", "Outback Onyx", 2022, 2590000m, 26000, "Wagon", "Gasoline", "CVT", "Green", "Good", false, "Backup Camera, Bluetooth, Heated Seats, Keyless Entry, Lane Keeping Assist, Apple CarPlay", "This Subaru Outback Onyx Edition is finished in Autumn Green Metallic. Outfitted with variable ride height, standard 4MATIC all-wheel drive, and massive rear luggage capacity. Finished in Autumn Green. The perfect alternative to bulky family SUVs."),
            ("Subaru", "Forester Sport", 2023, 2290000m, 17000, "SUV", "Gasoline", "CVT", "Gray", "Excellent", false, "Sunroof, Backup Camera, Heated Seats, Keyless Entry, Lane Keeping Assist, Apple CarPlay, Android Auto", "A highly functional Magnetite Gray Forester Sport. Features signature orange exterior accents, standard symmetrical AWD, and class-leading passenger visibility. Spacious cabin with ample headroom for five.")
        };

        var seededCars = new List<Car>();
        for (int i = 0; i < seedSpecs.Length; i++)
        {
            var spec = seedSpecs[i];
            
            // Deterministic Guid generation
            var carId = Guid.Parse($"00000000-0000-0000-0000-000000000{i + 1:D3}");

            // Generate 6 high-quality real image URLs using Unsplash photo IDs matching the car specs
            var primaryUnsplashId = GetPrimaryUnsplashId(spec.Brand, spec.Model);
            var detailIds = new[]
            {
                "1563720223185-11003d516935", // Luxury Interior Seats
                "1600706432502-75a0e08f58b9", // Premium Digital Cockpit
                "1614162692292-7ac56d7f7f1e", // Sporty Alloy Wheel Rim
                "1617814076367-b759c7d7e738", // LED Laser Headlight
                "1580273916550-e323be2ae537"  // Premium Quad Exhaust Taillights
            };

            var urlsList = new List<string>
            {
                $"https://images.unsplash.com/photo-{primaryUnsplashId}?auto=format&fit=crop&w=800&h=600&q=80"
            };
            foreach (var detailId in detailIds)
            {
                urlsList.Add($"https://images.unsplash.com/photo-{detailId}?auto=format&fit=crop&w=800&h=600&q=80");
            }
            var urls = string.Join(";", urlsList);

            seededCars.Add(new Car
            {
                Id = carId,
                Brand = spec.Brand,
                Model = spec.Model,
                Year = spec.Year,
                Price = spec.Price,
                Mileage = spec.Mileage,
                BodyType = spec.BodyType,
                FuelType = spec.FuelType,
                Transmission = spec.Transmission,
                Color = spec.Color,
                Condition = spec.Condition,
                Engine = spec.FuelType == "Electric" ? "Dual Electric Motor" : "Turbocharged Engine",
                Description = spec.Description,
                Features = spec.Features,
                ViewCount = 100 + i * 15,
                FavoriteCount = 5 + (i % 7) * 4,
                ImageUrls = urls,
                DateAdded = new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc).AddDays(-i * 2), // deterministic varied DateAdded
                IsFeatured = spec.IsFeatured,
                Status = "Available"
            });
        }

        builder.Entity<Car>().HasData(seededCars);
    }

    private static string GetPrimaryUnsplashId(string brand, string model)
    {
        return brand.ToLower() switch
        {
            "toyota" => model.ToLower() switch
            {
                "camry hybrid" => "1621007947382-bb3c3994e3fb", // Camry White
                "rav4 awd" => "1606577924006-27d39b132ee6", // RAV4 White
                "gr supra" => "1618843479313-40f8afb4b4d8", // Red Supra
                "corolla le" => "1590362891991-f776e747a588", // Corolla style
                "land cruiser" => "1533473359331-0135ef1b58bf", // Land Cruiser / Offroad SUV
                _ => "1621007947382-bb3c3994e3fb"
            },
            "honda" => model.ToLower() switch
            {
                "civic type r" => "1629897048514-3dd7414fe72a", // Civic Type R White
                "accord sport" => "1592853625527-711cb857ee0e", // Accord Black
                "cr-v hybrid" => "1618278943112-9c9abde6a6e2", // CR-V Blue SUV
                "civic sedan" => "1502877338535-766e1452684a", // Silver sedan
                _ => "1629897048514-3dd7414fe72a"
            },
            "bmw" => model.ToLower() switch
            {
                "m4 competition" => "1617814076367-b759c7d7e738", // BMW M4 Green
                "330i xdrive" => "1555215695-3004980ad54e", // BMW 330i Blue
                "x5 xdrive40i" => "1533473359331-0135ef1b58bf", // BMW X5 Black SUV
                "540i m sport" => "1549399542-7e3f8b79c341", // BMW 5 Series Gray
                _ => "1555215695-3004980ad54e"
            },
            "mercedes-benz" => model.ToLower() switch
            {
                "c300" => "1617531653332-bd46c24f2068", // Mercedes C300 Silver
                "amg gt 63" => "1618843479619-f4198754b209", // Mercedes AMG GT Black
                "gle 450" => "1549399542-7e3f8b79c341", // Mercedes GLE White
                "e450 all-terrain" => "1503376780353-7e6692767b70", // Mercedes E Wagon Gray
                _ => "1617531653332-bd46c24f2068"
            },
            "audi" => model.ToLower() switch
            {
                "rs e-tron gt" => "1606016159991-dfe4f2746ad5", // Audi e-tron GT Gray
                "a4 s-line" => "1606016159991-dfe4f2746ad5", // Audi A4 Blue
                "q7 premium" => "1533473359331-0135ef1b58bf", // Audi Q7 Black
                "a6 allroad" => "1549399542-7e3f8b79c341", // Audi A6 Silver
                _ => "1606016159991-dfe4f2746ad5"
            },
            "tesla" => model.ToLower() switch
            {
                "model 3 lr" => "1617788138017-80ad40651399", // Tesla Model 3 Blue
                "model y performance" => "1563720223185-11003d516935", // Tesla Model Y White
                "model s plaid" => "1619767886558-efdc259cde1a", // Tesla Model S Black
                _ => "1617788138017-80ad40651399"
            },
            "ford" => model.ToLower() switch
            {
                "mustang gt" => "1584345604476-8ec5e12e42dd", // Ford Mustang Red
                "explorer xlt" => "1533473359331-0135ef1b58bf", // Ford Explorer Gray
                "bronco outer banks" => "1606577924006-27d39b132ee6", // Ford Bronco Green
                "f-150 lariat" => "1533560904424-a0c61dc306fc", // Ford F-150 Black
                _ => "1584345604476-8ec5e12e42dd"
            },
            "hyundai" => model.ToLower() switch
            {
                "ioniq 5 n" => "1617788138017-80ad40651399", // Hyundai Ioniq 5 Blue
                "elantra n" => "1621007947382-bb3c3994e3fb", // Hyundai Elantra Black
                "tucson hybrid" => "1606577924006-27d39b132ee6", // Hyundai Tucson Silver
                "palisade calligraphy" => "1533473359331-0135ef1b58bf", // Hyundai Palisade White
                _ => "1621007947382-bb3c3994e3fb"
            },
            "mazda" => model.ToLower() switch
            {
                "cx-5 turbo" => "1606577924006-27d39b132ee6", // Mazda CX-5 Red
                "premium" => "1621007947382-bb3c3994e3fb", // Mazda 3 Gray
                "mx-5 miata" => "1552519507-da3b142c6e3d", // Mazda Miata Blue
                _ => "1552519507-da3b142c6e3d"
            },
            "mazda 3" => "1621007947382-bb3c3994e3fb", // Mazda 3
            "nissan" => model.ToLower() switch
            {
                "gt-r premium" => "1580273916550-e323be2ae537", // Nissan GTR Silver
                "altima sr" => "1502877338535-766e1452684a", // Nissan Altima Black
                "rogue sv" => "1533473359331-0135ef1b58bf", // Nissan Rogue White
                _ => "1580273916550-e323be2ae537"
            },
            "porsche" => model.ToLower() switch
            {
                "911 carrera s" => "1614162692292-7ac56d7f7f1e", // Porsche 911 Gray
                "cayenne coupe" => "1503376780353-7e6692767b70", // Porsche Cayenne Black
                "taycan 4s" => "1611245706969-9069d3b76e27", // Porsche Taycan Red
                _ => "1614162692292-7ac56d7f7f1e"
            },
            "lexus" => model.ToLower() switch
            {
                "lc 500" => "1544829099-b9a0c07fad1a", // Lexus LC 500 Yellow
                "rx 350 f sport" => "1533473359331-0135ef1b58bf", // Lexus RX White
                "es 300h" => "1549399542-7e3f8b79c341", // Lexus ES Silver
                _ => "1544829099-b9a0c07fad1a"
            },
            "chevrolet" => model.ToLower() switch
            {
                "corvette z06" => "1600706432502-75a0e08f58b9", // Chevy Corvette Red
                "silverado 1500" => "1533560904424-a0c61dc306fc", // Chevy Silverado Gray
                "equinox premier" => "1533473359331-0135ef1b58bf", // Chevy Equinox Blue
                _ => "1600706432502-75a0e08f58b9"
            },
            "volkswagen" => model.ToLower() switch
            {
                "golf r" => "1616422285623-13ff0162193c", // VW Golf R Blue
                "tiguan sel" => "1533473359331-0135ef1b58bf", // VW Tiguan White
                _ => "1616422285623-13ff0162193c"
            },
            "subaru" => model.ToLower() switch
            {
                "wrx sti" => "1605559424843-9e4c228bf1c2", // Subaru WRX STI Blue
                "outback onyx" => "1533473359331-0135ef1b58bf", // Subaru Outback Green
                "forester sport" => "1606577924006-27d39b132ee6", // Subaru Forester Gray
                _ => "1605559424843-9e4c228bf1c2"
            },
            _ => "1503376780353-7e6692767b70" // fallback generic luxury black car
        };
    }
}
