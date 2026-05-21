// ============================================================
// RevvUp.Infrastructure — Application DbContext
// EF Core + ASP.NET Identity database context
// ============================================================

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RevvUp.Core.Entities;

namespace RevvUp.Infrastructure.Data;

/// <summary>
/// EF Core DbContext with ASP.NET Identity support.
/// Uses SQLite for development — swap provider for production.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <summary>Car listings table</summary>
    public DbSet<Car> Cars => Set<Car>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Car entity configuration ──
        builder.Entity<Car>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Make).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Model).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Price).HasColumnType("decimal(18,2)");
            entity.Property(c => c.Description).HasMaxLength(2000);
        });

        // ── Rename Identity tables for cleaner schema ──
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.DisplayName).HasMaxLength(100);
            entity.Property(u => u.Bio).HasMaxLength(500);
            entity.Property(u => u.Location).HasMaxLength(200);
        });
    }
}
