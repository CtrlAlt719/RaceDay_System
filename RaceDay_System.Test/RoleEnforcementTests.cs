using Microsoft.AspNetCore.Mvc;
using RaceDay_System.Controllers;
using RaceDay_System.Dtos;
using RaceDay_System.Test.Infrastructure;
using Xunit;
using System.Threading.Tasks;

namespace RaceDay_System.Test
{
    public class RoleEnforcementTests
    {
        [Fact]
        public async Task CreateEvent_AsParticipant_ReturnsForbid()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var controller = new EventsController(context, null);

            // Log in as a Participant
            controller.ControllerContext = RaceDay_SystemFactory.GetParticipantContext();
            var dto = new CreateEventDto { Name = "Unauthorized Event" };

            // Act
            var result = await controller.Create(dto);

            // Assert
            // If your controller uses [Authorize(Roles="Organiser")], the framework blocks it with 403 Forbidden.
            // This unit test ensures that if the controller is called, it respects the claims.
            Assert.IsType<ForbidResult>(result);
            // (Note: If your Create method doesn't explicitly check User.IsInRole, the test might fail here. 
            // If it fails, change this assert to Assert.IsType<ObjectResult>(result) and check for 403).
        }
    }
}