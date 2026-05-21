# 🏎️ RevvUp — Premium Car Selling Marketplace

Welcome to **RevvUp**, a fun, catchment, and premium car selling platform where users showcase, buy, and sell vehicles in a state-of-the-art interactive ecosystem. Designed to look and feel 1000x more premium and exciting than Cardekho.com, RevvUp bridges modern UI design with Clean Architecture principles.

---

## 🌟 Visuals & Core Features

### 💎 1. Ultimate Car Details & Customizer
- **simulated 3D Exterior Painter**: Interactive paint color switcher supporting customized finishes (*Championship Red*, *Electric Blue*, *Emerald Green*, *Factory Stock*) using dynamic Alpine hue-rotate shader modifications.
- **360° Cockpit View**: Toggle instantly to high-resolution luxurious interior panoramas.
- **Dynamic EMI Amortization**: Instant real-time loan principal, interest rates (4.5% to 15.0%), and flexible amortization calculator.
- **Similar Car Carousels**: Automated recommender matching make and body configurations.

### 🔍 2. Elite Browsing & Infinite Scroll
- **Dynamic Filter Sidebar**: Interactive multi-criteria filters (Make, Mileage, Pricing, Fuel, Transmissions).
- **HTMX Infinite scroll**: Load next pages smoothly without page refreshes.
- **IntersectionObserver Reveal**: Staggered animated entrances as cars glide into the frame.

### ⚡ 3. Real-Time SignalR Alerts & Wishlists
- **SignalR Alert Bell**: Gorgeous notification bell pushing live messages.
- **Wishlist Undo Toast**: Delete favorites with single-click Undo triggers and timers.
- **Multi-Slot Comparison**: Side-by-side Technical Specification comparative dashboard comparing up to 4 models.

### 💰 4. Multi-Step Seller Wizard
- **Animated Forms**: 3-step listing builder with custom input validation and step trackers.
- **Drag-and-Drop Image Frame**: Modern photography uploader simulation with instant image previews.

---

## 🏗️ Architecture & Project Structure

RevvUp is built on **Clean Architecture** patterns, ensuring complete separation of domain code and database logic.

```
RevvUp Workspace Structure:
├── src/
│   ├── RevvUp.Core/                  ← Core domain entities, contracts, and repository interfaces
│   ├── RevvUp.Application/           ← App services logic and orchestration rules
│   ├── RevvUp.Infrastructure/        ← Data persistence repositories (SQLite, Entity Framework Core)
│   └── RevvUp.Web/                   ← ASP.NET MVC controllers, views, assets, and layouts
└── RevvUp.slnx                       ← Solution configuration
```

---

## 🛠️ Technology Stack

- **Backend**: C# 12, ASP.NET Core 8 MVC, ASP.NET Identity Core, SignalR.
- **Database**: SQLite, Entity Framework Core (auto-migrated in development mode).
- **Frontend**: Tailwind CSS 3, Alpine.js, HTMX.
- **Assets**: Google Fonts (Inter + Outfit), Unsplash premium photography.

---

## 🚦 How to Run the App (Step-by-Step)

### 1. Build and Restore
Restore dependencies and compile the solution:
```bash
dotnet build RevvUp.slnx
```

### 2. Compile Tailwind CSS Styles
Compile and minify the Tailwind stylesheets:
```bash
cd src/RevvUp.Web
npm run css:build
cd ../..
```

### 3. Launch Server
Start the ASP.NET Web App:
```bash
dotnet run --project src/RevvUp.Web
```

### 4. Experience RevvUp
Open your browser and navigate to:
- **Marketplace Landing**: [http://localhost:5106](http://localhost:5106)
- **Advanced Browse**: [http://localhost:5106/Cars/Browse](http://localhost:5106/Cars/Browse)
- **Side-by-Side Comparison**: [http://localhost:5106/Cars/Compare](http://localhost:5106/Cars/Compare)

---

## 🚀 Future Improvement Roadmap
1. **Azure Blob Storage**: Integrate actual cloud photo upload buckets inside step 3 of the seller wizard.
2. **SignalR Backend Integration**: Hook database triggers to the `NotificationHub` to dispatch live alerts whenever a listing gets saved or sold.
3. **Advanced AI Recommendations**: Leverage similarity algorithms to surface ultra-relevant vehicle picks.
