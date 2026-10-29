using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay_System.Models;

namespace RaceDay_System.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Apply pending migrations
            await context.Database.MigrateAsync();

            // 2. Seed roles
            foreach (var role in new[] { "Organiser", "Participant" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 3. Sample organisers + participants (only if users table empty)
            if (!await userManager.Users.AnyAsync())
            {
                await EnsureUser(userManager, "Thabo", "Mkhize", "thabo.mkhize@raceday.co.za", "Organiser", "Passw0rd!");
                await EnsureUser(userManager, "Naledi", "Dlamini", "naledi.dlamini@raceday.co.za", "Organiser", "Passw0rd!");
                await EnsureUser(userManager, "Sipho", "Zulu", "sipho.zulu@example.com", "Participant", "Passw0rd!");
                await EnsureUser(userManager, "Amahle", "Ndlovu", "amahle.ndlovu@example.com", "Participant", "Passw0rd!");
            }

            // 4. Seed Sample Events and Categories (only if events table empty)
            if (!await context.Events.AnyAsync())
            {
                // Find the organiser we just created to link the events to them
                var organiser = await userManager.FindByEmailAsync("thabo.mkhize@raceday.co.za");

                if (organiser != null)
                {
                    var event1 = new Event
                    {
                        OrganiserId = organiser.Id,
                        Name = "Cape Town Marathon",
                        Description = "A scenic 42km run along the coast.",
                        EventDate = DateTime.UtcNow.AddDays(30), // 30 days from now
                        Venue = "Cape Town Stadium",
                        Status = "Upcoming"
                    };

                    var event2 = new Event
                    {
                        OrganiserId = organiser.Id,
                        Name = "Durban Cycle Tour",
                        Description = "100km cycling race through the city.",
                        EventDate = DateTime.UtcNow.AddDays(60),
                        Venue = "Durban Beachfront",
                        Status = "Upcoming"
                    };

                    var event3 = new Event
                    {
                        OrganiserId = organiser.Id,
                        Name = "Joburg 10k Night Run",
                        Description = "A fast flat 10k run under the city lights.",
                        EventDate = DateTime.UtcNow.AddDays(-10), // Past event
                        Venue = "Sandton CBD",
                        Status = "Completed"
                    };

                    context.Events.AddRange(event1, event2, event3);
                    await context.SaveChangesAsync(); // Save events first to generate their IDs

                    // Add categories to the events
                    var categories = new List<Category>
                    {
                        new Category { EventId = event1.EventId, Name = "Full Marathon", DistanceKm = 42, Capacity = 500, EntryFee = 250 },
                        new Category { EventId = event1.EventId, Name = "Half Marathon", DistanceKm = 21, Capacity = 1000, EntryFee = 150 },
                        new Category { EventId = event2.EventId, Name = "Pro 100km", DistanceKm = 100, Capacity = 200, EntryFee = 400 },
                        new Category { EventId = event3.EventId, Name = "Open 10k", DistanceKm = 10, Capacity = 2000, EntryFee = 100 }
                    };

                    context.Categories.AddRange(categories);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task EnsureUser(
            UserManager<ApplicationUser> userManager,
            string firstName, string lastName, string email, string role, string password)
        {
            if (await userManager.FindByEmailAsync(email) != null) return;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception($"Failed to seed {email}: {string.Join("; ", result.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(user, role);
        }
    }
}