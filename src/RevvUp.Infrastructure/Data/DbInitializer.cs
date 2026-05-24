// ============================================================
// RevvUp.Infrastructure — DbInitializer
// Database initialization and seeding for development
// ============================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RevvUp.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RevvUp.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var email = "cardealer@revvup.com";
        var demoSeller = await userManager.FindByEmailAsync(email);

        if (demoSeller == null)
        {
            demoSeller = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = "RevvUp Demo Dealer",
                SellerDisplayName = "RevvUp Demo Dealer",
                EmailConfirmed = true,
                JoinedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(demoSeller, "Demo@1234");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed demo seller: {errors}");
            }
        }

        // Only assign seeded cars (SellerId is null) — never touch cars listed by real users.
        // Seeded cars use deterministic GUIDs 00000000-0000-0000-0000-000000000001 … 052
        // and are created by HasData() without a SellerId, so SellerId == null is the
        // reliable indicator that a car came from the seed and still needs assignment.
        var unownedSeedCars = await context.Cars
            .Where(c => c.SellerId == null)
            .ToListAsync();

        foreach (var car in unownedSeedCars)
        {
            car.SellerId = demoSeller.Id;
            car.SellerDisplayName = demoSeller.DisplayName;
        }

        // ── One-time repair: undo damage from the old blanket-assignment bug ──
        // Any car that is NOT a seeded car (outside the deterministic GUID range)
        // but is currently assigned to the demo seller was wrongly claimed.
        // We clear its SellerId so the real owner can reclaim it or so it becomes
        // visibly un-owned in the admin view. This is a no-op once all data is clean.
        var seedGuidSet = Enumerable.Range(1, 52)
            .Select(i => Guid.Parse($"00000000-0000-0000-0000-000000000{i:D3}"))
            .ToHashSet();

        var wronglyAssigned = await context.Cars
            .Where(c => c.SellerId == demoSeller.Id)
            .ToListAsync();

        foreach (var car in wronglyAssigned.Where(c => !seedGuidSet.Contains(c.Id)))
        {
            car.SellerId = null;
            car.SellerDisplayName = null;
        }

        await context.SaveChangesAsync();
    }
}
