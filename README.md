# RaceDay System

Welcome to the RaceDay System! This is a full-stack ASP.NET Core MVC web application I built to manage race events, categories, and participant enrolments

It handles everything from an Organiser creating a race, to a Participant logging in and booking their spot on the starting line

## what it can do

*   **Different roles, different views:** Organisers get a dashboard to manage events; Participants get the ability to enrol. No one sees buttons they aren't allowed to click
*   **Full Event Management:** Organisers can create, edit, or delete races right from the web page
*   **Race Categories:** You can add multiple distances to a single event (like a 10km Fun Run or a 42km Marathon), complete with their own entry fees and capacity limits
*   **Smart Enrolments:** The system actually stops people from double-booking a race, and it checks if the category is already full before letting someone in 
*   **Built-in API:** There's a fully documented Swagger UI for anyone who wants to interact with the system via API
*   **Auto-Seeding:** Nobody wants an empty database. The app automatically populates itself with sample events, categories, and test users the very first time you run it
*   **CI/CD Pipeline:** I set up GitHub Actions to automatically build the project every time I push code, ensuring nothing breaks

## what's under the hood

*   **Backend:** ASP.NET Core 8.0 (MVC + Web API)
*   **Database:** Entity Framework Core 8.0 with SQL Server
*   **Security:** ASP.NET Core Identity & JWT Bearer Tokens
*   **Frontend:** Razor Views and Bootstrap 5
*   **Testing:** xUnit and EF Core In-Memory databases
*   **DevOps:** GitHub Actions

## how to run it locally

Want to spin this up on your own machine? Here's how:

**what you'll need:**
*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   SQL Server Express (or a local SQL Server instance)
*   Visual Studio 2022

**the steps:**
1. Clone the repo: `git clone https://github.com/CtrlAlt719/RaceDay_System.git`
2. Open `appsettings.json` and make sure your connection string points to your local SQL Server
3. Open the Package Manager Console in Visual Studio and run `Update-Database` to build the database
4. Press **F5** to run the app. The Seeder will handle the rest!

## test Accounts
If you want to log in and play around, I've set up some test accounts. The password for all of them is `Passw0rd!`

*   **Organisers:** `thabo.mkhize@raceday.co.za` or `naledi.dlamini@raceday.co.za`
*   **Participants:** `sipho.zulu@example.com` or `amahle.ndlovu@example.com`

## project Structure

*   `Controllers/` - Where the web pages and API endpoints live
*   `Data/` - The database context, migrations, and the seeder logic
*   `Models/` - The core entities (Events, Categories, Enrolments, Users)
*   `Views/` - The HTML/Razor front-end.
*   `RaceDay_System.Test/` - The unit tests
*   `.github/workflows/` - The automated build pipeline

## swagger API
Once the app is running, just go to `/swagger` in your browser. You can log in via the API to get a JWT token, click the green "Authorize" padlock, and test out the endpoints
