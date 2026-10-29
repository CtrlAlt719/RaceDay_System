using Microsoft.AspNetCore.Mvc;
using RaceDay_System.Controllers;
using RaceDay_System.Dtos;
using RaceDay_System.Test.Infrastructure;
using Xunit;
using System.Threading.Tasks;

namespace RaceDay_System.Test
{
    public class EventTests
    {
        [Fact]
        public async Task CreateEvent_ValidData_ReturnsCreated()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var controller = new EventsController(context, null);
            controller.ControllerContext = RaceDay_SystemFactory.GetOrganiserContext();

            var dto = new CreateEventDto
            {
                Name = "Test Race",
                Venue = "Test Venue",
                EventDate = TestDate.FutureDate,
                Status = "Upcoming"
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }
    }
}