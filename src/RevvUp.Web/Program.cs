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
app.MapHub<RevvUp.Web.Hubs.NotificationHub>("/hubs/notifications");

app.Run();
