// ============================================================
// RevvUp — Program.cs (Entry Point)
// ASP.NET Core MVC with Identity + Clean Architecture
// Phase 2: Authentication & Dashboard
// ============================================================

using Microsoft.EntityFrameworkCore;
using RevvUp.Infrastructure;
using RevvUp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Register MVC services ──
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Required for Identity UI
builder.Services.AddSignalR(); // Add SignalR support
builder.Services.AddScoped<RevvUp.Web.Services.NotificationService>();

// ── Connection string ──
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=revvup.db";

// ── Register RevvUp services (Clean Architecture + Identity) ──
builder.Services.AddRevvUpServices(connectionString);

// ── Cookie configuration for auth ──
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "RevvUp.Auth";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// ── Auto-migrate database in development ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    
    // Register initial migration so EF doesn't try to recreate existing tables
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
            ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
            ""ProductVersion"" TEXT NOT NULL
        );
        INSERT OR IGNORE INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
        VALUES ('20260522055859_AddEngineToCar', '10.0.8');
        INSERT OR IGNORE INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
        VALUES ('20260523133350_AddSoftDeleteToInquiry', '10.0.8');
        INSERT OR IGNORE INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
        VALUES ('20260524034031_AddSellerFields', '10.0.8');
        INSERT OR IGNORE INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
        VALUES ('20260524065316_AddNotificationSystem', '10.0.8');
    ");
    
    // Apply any new pending migrations automatically on startup
    db.Database.Migrate();

    // Create a demo seller account and reassign all 52 existing car listings to this account (Dev only)
    if (app.Environment.IsDevelopment())
    {
        await DbInitializer.SeedAsync(scope.ServiceProvider);
    }
}

// ── Configure HTTP pipeline ──
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ── Map routes ──
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<RevvUp.Web.Hubs.NotificationHub>("/notificationHub");

app.Run();
