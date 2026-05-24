# RevvUp 🚗
> A premium car marketplace web application engineered with ASP.NET Core 10 MVC.

---

## 📌 About The Project
**RevvUp** is an elite, high-performance digital showroom and car marketplace designed for automotive purists. The platform enables seamless buying, selling, and managing of curated vehicle collections. 

By unifying buyers and sellers into a single, high-fidelity experience, RevvUp bridges the gap between private listings, dealerships, and active automotive enthusiasts.

### Key Features
*   **Intuitive Inventory Showroom:** Sleek, visual browse and search pages with dynamic filters.
*   **Unified Account Roles:** Unified Buyer and Seller roles in one combined account—any registered user can both browse wishlist items and list high-end vehicles.
*   **Real-time Communications:** Bidirectional, real-time user messaging powered by **SignalR** connection networks.
*   **Instant Notifications:** Real-time push alerts for listings, inquiries, favorites, and new messages.
*   **Side-by-Side Spec Comparison:** A visual multi-vehicle spec evaluator to compare up to 4 models.
*   **Interactive Financial Estimator:** Built-in dynamic EMI calculator to estimate monthly loans and interest.
*   **Seller Command Center:** Dedicated seller listing dashboard complete with viewer statistics, inquiry thread managers, and soft-delete controls.
*   **Status Management:** One-click functionality to mark listings as **Sold** or **Available**.
*   **Premium Security:** Validate Anti-Forgery Token protection on all critical forms and AJAX endpoints with elegant modal confirmations.

---

## 🛠️ Tech Stack

| Layer | Technology | Description |
| :--- | :--- | :--- |
| **Language** | C# 12 | Core programming language |
| **Framework** | ASP.NET Core 10 MVC | Model-View-Controller backend architecture |
| **Auth** | ASP.NET Identity Core | Secure user registration, cookie auth, and profile controls |
| **Real-time** | SignalR | Low-latency WebSockets communication network |
| **ORM** | Entity Framework Core | Database queries and migration handling |
| **Database** | SQLite | Serverless, local relational data storage |
| **Frontend** | Tailwind CSS 3 | High-fidelity, modern utility-first CSS |
| **Reactivity** | Alpine.js & HTMX | Lightweight client-side DOM interactions |
| **Typography**| Google Fonts | Premium typography pairing (Outfit + Inter) |
| **Assets** | Unsplash / Placehold.co | Visual placeholders and high-resolution images |

---

## ✅ Requirements
Before launching the application, ensure you have the following installed on your system:

1.  **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)**
    *   Required to compile and run the application.
2.  **[Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (v17.10 or later)**
    *   Make sure to install the following workloads during setup:
        *   *ASP.NET and web development*
        *   *.NET desktop development*
3.  **[Node.js](https://nodejs.org)**
    *   Recommended for running the Tailwind CSS CLI compilers locally.
4.  **[Git](https://git-scm.com)**
    *   For version control and repository management.
5.  **SQLite**
    *   No manual install needed! EF Core automatically handles creation, migrations, and seeding out-of-the-box.

---

## 🚀 Getting Started

Follow these steps to run the application locally on your computer:

### Step 1 — Clone the Repository
```bash
git clone https://github.com/YOUR_USERNAME/RevvUp.git
cd RevvUp
```

### Step 2 — Open in Visual Studio
*   Launch **Visual Studio 2022** (or Visual Studio Preview).
*   Select **Open a project or solution**.
*   Navigate to the cloned directory and open `RevvUp.slnx` or the project solution.

### Step 3 — Restore NuGet Packages
Visual Studio will automatically restore required packages upon opening. If needed, you can restore manually via the **Package Manager Console**:
```bash
dotnet restore
```

### Step 4 — Apply Database Migrations
The database operates on SQLite. In `Development` mode, migrations are automatically applied on application startup. To manually generate or apply them:

*   **Via Package Manager Console (Tools → NuGet → Package Manager Console):**
    ```powershell
    Update-Database
    ```
*   **Via Terminal / Command Prompt:**
    ```bash
    dotnet ef database update
    ```

### Step 5 — Run the Project
*   Press **F5** or the **Start** button in Visual Studio.
*   **OR run via terminal:**
    ```bash
    dotnet run --project src/RevvUp.Web
    ```

Once compiled, the application will be hosted locally at:
*   `https://localhost:5106`
*   `http://localhost:5106`

---

## 🗄️ Database
*   **Auto-Creation:** The SQLite database file is automatically created on first-run inside the `/App_Data/revvup.db` directory.
*   **Database Initializer:** The database initializer (`DbInitializer.cs`) runs automatically in development environments. It auto-migrates and seeds a complete starter kit containing **52 vehicle listings** pre-assigned to the default dealer account.
*   **No Manual Setup:** Zero relational configuration, engine, or local database installs are required.

---

## 👤 Sample Accounts

### Demo Seller Account (owns all 52 pre-seeded listings)
| Field | Value |
| :--- | :--- |
| **Email** | `cardealer@revvup.com` |
| **Password** | `Demo@1234` |
| **Role** | Unified Buyer + Seller |
| **Listings** | 52 Premium Cars Preloaded |

### Register Your Own Account
*   Navigate to `/Account/Register` on your local host.
*   All newly registered users are automatically granted a unified account. 
*   To list your own car, simply click **Dashboard** in the navbar, go to **My Listings**, and click **Post a Listing**!

---

## 📁 Project Structure

```text
RevvUp/
├── Controllers/          → MVC Controllers (Routes, actions, model-binding)
├── Models/               → Domain Entities, ViewModels, and Form inputs  
├── Views/                → Razor (.cshtml) layout templates
│   ├── Shared/           → General page framework, headers, footers, & dropdowns
│   ├── Cars/             → Catalog grids, browse search, and car details pages
│   ├── Dashboard/        → Buyer overview, wishlists, and conversation inbox
│   ├── Seller/           → Listings CRUD forms, status controls, and inquiries
│   └── Account/          → Identity views (Login, Registration, Profiles)
├── Hubs/                 → SignalR hubs facilitating real-time WebSockets connections
├── Services/             → Core services (Notification dispatchers)
├── Data/                 → Database Context, initialization seeds, and model constraints
├── Migrations/           → Entity Framework Core schema blueprints
├── wwwroot/              → Distributed static assets
│   ├── css/              → Tailored CSS assets
│   ├── js/               → Reactivity scripts
│   └── images/           → System icons and media placeholders
├── appsettings.json      → System connection strings and environment keys
└── Program.cs            → App builder, pipeline routes, and middleware settings
```

---

## ⚙️ Configuration

The database connection is defined in the `appsettings.json` file inside `src/RevvUp.Web`. No external API keys or credentials are required.

To customize the SQLite database storage path, edit the connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=App_Data/revvup.db"
}
```

---

## 🔑 Key Pages

| Page | URL | Description |
| :--- | :--- | :--- |
| **Showroom Home** | `/` | Responsive landing page and curated catalogs |
| **Browse Inventory** | `/Cars/Browse` | Complete multi-faceted vehicle search |
| **Car Details** | `/Cars/Details/{id}` | Comprehensive view of vehicle specifications and calculators |
| **Buyer Dashboard** | `/Dashboard` | Command center displaying user metrics & recent activity feeds |
| **Message Center** | `/Dashboard/Messages` | Fully reactive inquiry inbox |
| **Seller Listings** | `/Seller/MyListings` | Direct inventory CRUD and dealer status controllers |
| **Profile Settings** | `/Account/Profile` | Custom user metadata and avatar managers |
| **Sign In** | `/Account/Login` | Secure credential authenticator |
| **Registration** | `/Account/Register` | Unified account registration entry point |

---

## 🐛 Known Issues
*   None currently. If you encounter any bugs, feel free to open a new GitHub Issue!

---

## 📄 License
This repository is engineered for educational, demonstration, and professional portfolio purposes. All seeded vehicles and user credentials are safe development assets.
