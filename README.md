# RevvUp - Premium Car Browsing and Discovery Platform

Welcome to RevvUp, a curated premium car discovery and browsing platform where buyers browse, search, and inquire about professionally listed luxury, sports, and supercars. Designed to look and feel extremely premium, RevvUp bridges modern UI design with Clean Architecture principles.

---

## Core Features

### Curated Professional Inventory
- **SQLite Database Auto-Seeding**: A premium inventory of 52 deterministic, luxury and sports vehicles automatically populated on startup. Includes brands like Porsche, Tesla, BMW, Audi, Mercedes-Benz, Lexus, and Toyota.
- **Detailed Car Spotlights**: View comprehensive, rich listings complete with advanced details, high-fidelity gallery carousels, and pre-configured specifications.
- **Dynamic EMI Amortization**: Instant real-time loan calculator for principal, customizable interest rates (4.5% to 15.0%), and flexible amortization periods.

### Elite Search and Discovery
- **Dynamic Filter Sidebar**: Interactive multi-criteria filters including Brand, Mileage, Pricing, Fuel Type, and Transmissions.
- **HTMX Real-time Search**: Search and filter the premium inventory with instant page grid updates without full page refreshes.

### Premium Buyer Ecosystem
- **Dealer Inquiry Engine**: Direct, professional dealer contact inquiries capturing Name, Email, Phone, and Message, fully persisting to the SQLite database.
- **Real-Time Inquiry Chat Messenger**: Dedicated communication console where buyers chat in real-time with dealership coordinators and support specialists regarding their sent inquiries.
- **Interactive Saved Vehicles**: A synchronized wishlist with instant favorites toggling, live navbar count notifications, and an intuitive "Undo" toast notifier.
- **Multi-Slot Spec Comparison**: Side-by-side comparative dashboard comparing specifications, pricing, and performance of up to 4 models.

---

## Architecture and Project Structure

RevvUp is built on Clean Architecture patterns, ensuring complete separation of domain code and database logic.

```
RevvUp Workspace Structure:
├── src/
│   ├── RevvUp.Core/                  ← Core domain entities, contracts, and repository interfaces
│   ├── RevvUp.Application/           ← App services logic and orchestration rules
│   ├── RevvUp.Infrastructure/        ← Data persistence repositories (SQLite, Entity Framework Core)
│   └── RevvUp.Web/                   ← ASP.NET MVC controllers, views, assets, and layouts
│   └── revvup.db                     ← SQLite database containing the seeded premium catalog
└── RevvUp.slnx                       ← Solution configuration
```

---

## Technology Stack

- **Backend**: C# 12, ASP.NET Core 10 MVC, ASP.NET Identity Core, SignalR.
- **Database**: SQLite, Entity Framework Core (auto-migrated and auto-seeded in development mode).
- **Frontend**: Tailwind CSS 3, Alpine.js, HTMX.
- **Assets**: Google Fonts (Inter + Outfit), Unsplash premium photography.

---

## How to Run the App (Step-by-Step)

### 1. Build and Restore
Restore dependencies and compile the solution:
```bash
dotnet build RevvUp.slnx
```

### 2. Auto-Seeding and Launching
Launch the web application project. The first run will automatically compile, migrate, and seed the SQLite database file:
```bash
dotnet run --project src/RevvUp.Web
```

### 3. Experience RevvUp
Open your browser and navigate to:
- **Marketplace Landing**: http://localhost:5106
- **Advanced Browse**: http://localhost:5106/Cars/Browse
- **Side-by-Side Comparison**: http://localhost:5106/Cars/Compare
- **Buyer Dashboard**: http://localhost:5106/Dashboard
