using Microsoft.AspNetCore.Mvc;
using RaceDay_System.Controllers;
using RaceDay_System.Dtos;
using RaceDay_System.Models;
using RaceDay_System.Test.Infrastructure;
using Xunit;
using System.Threading.Tasks;

namespace RaceDay_System.Test
{
    public class EnrolmentTests
    {
        [Fact]
        public async Task Enrol_AlreadyEnrolled_ReturnsConflict()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var category = new Category { Name = "10km", Capacity = 10, DistanceKm = 10, EntryFee = 50 };
            context.Categories.Add(category);

            context.Enrolments.Add(new Enrolment { ParticipantId = "participant-123", CategoryId = category.CategoryId });
            await context.SaveChangesAsync();

            var controller = new EnrolmentController(context);
            controller.ControllerContext = RaceDay_SystemFactory.GetParticipantContext();

            var dto = new CreateEnrolmentDto { CategoryId = category.CategoryId };

            // Act
            var result = await controller.Enrol(category.CategoryId, dto);

            // Assert
            Assert.IsType<ConflictObjectResult>(result); // 409 Conflict
        }
    }
}