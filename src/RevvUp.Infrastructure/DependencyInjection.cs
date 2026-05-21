// ============================================================
// RevvUp.Infrastructure — Dependency Injection Registration
// Clean Architecture: Infrastructure wires up implementations
// Phase 2: Added Identity + EF Core registration
// ============================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RevvUp.Application.Interfaces;
using RevvUp.Application.Services;
using RevvUp.Core.Entities;
using RevvUp.Core.Interfaces;
using RevvUp.Infrastructure.Data;
using RevvUp.Infrastructure.Repositories;

namespace RevvUp.Infrastructure;

/// <summary>
/// Extension method to register all Infrastructure + Application services.
/// Called from Program.cs in the Web layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddRevvUpServices(this IServiceCollection services, string connectionString)
    {
        // ── EF Core + SQLite ──
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        // ── ASP.NET Identity ──
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password: relaxed for dev, tighten for production
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 1;
            options.Password.RequiredUniqueChars = 1;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Sign-in: no email confirmation required for now
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // ── Repositories ──
        services.AddScoped<ICarRepository, EfCarRepository>();

        // ── Application Services ──
        services.AddScoped<ICarService, CarService>();

        return services;
    }
}
