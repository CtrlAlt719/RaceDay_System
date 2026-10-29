using Microsoft.AspNetCore.Mvc;
using RaceDay_System.Controllers;
using RaceDay_System.Dtos;
using RaceDay_System.Models;
using RaceDay_System.Test.Infrastructure;
using Xunit;
using System;
using System.Threading.Tasks;

namespace RaceDay_System.Test
{
    public class ResultTests
    {
        [Fact]
        public async Task CreateResult_ValidEnrolment_ReturnsCreated()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var organiserId = "organiser-123";

            var ev = new Event { Name = "Test", OrganiserId = organiserId, Status = "Open" };
            var cat = new Category { Name = "10k", Event = ev, Capacity = 10, DistanceKm = 10, EntryFee = 0 };
            var enrolment = new Enrolment { ParticipantId = "participant-1", Category = cat };
            context.Enrolments.Add(enrolment);
            await context.SaveChangesAsync();

            var controller = new ResultsController(context);
            controller.ControllerContext = RaceDay_SystemFactory.GetOrganiserContext(organiserId);

            var dto = new CreateResultDto { FinishTime = TimeSpan.FromMinutes(50), Position = 1, ResultStatus = "Finished" };

            // Act
            var result = await controller.CreateResult(enrolment.EnrolmentId, dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
        }
    }
}