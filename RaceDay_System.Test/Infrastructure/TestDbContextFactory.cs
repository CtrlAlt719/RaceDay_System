using Microsoft.EntityFrameworkCore;
using RaceDay_System.Data;

namespace RaceDay_System.Test.Infrastructure
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}