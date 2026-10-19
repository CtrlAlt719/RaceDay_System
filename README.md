# 🏁 RaceDay System

A comprehensive ASP.NET Core MVC application designed to manage race day events, routes, participants, and results. 

## 📋 Features
*   **Event Management:** Create and manage upcoming races and venues.
*   **Route Planning:** Define start/end locations and distances for race routes.
*   **Enrolments:** Handle participant registrations for specific categories.
*   **Results Tracking:** Log and view race results.
*   **Role-Based Access:** Secure endpoints for Organisers and Participants.
*   **Automated Database Seeding:** Comes pre-configured with a Seeder to populate initial categories, events, and users.

## 🛠️ Tech Stack
*   **Framework:** ASP.NET Core 8.0 (MVC)
*   **ORM:** Entity Framework Core 8.0
*   **Database:** SQL Server (`localhost\SQLEXPRESS`)
*   **Authentication:** ASP.NET Core Identity
*   **Testing:** xUnit & EF Core In-Memory Database

## 🚀 Getting Started

### Prerequisites
*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or standard Express)
*   Visual Studio 2022 (with ASP.NET and web development workload)

### 🗄️ Database Setup & Configuration
This project uses Entity Framework Core Code-First migrations. 

1. Open `appsettings.json` and verify your connection string. By default, it uses Windows Authentication for a local SQL Express instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS06;Database=RaceDayDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
