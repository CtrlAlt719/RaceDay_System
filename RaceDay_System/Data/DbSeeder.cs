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