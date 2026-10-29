using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RaceDay_System.Controllers; // Adjust if your AuthController is in a different namespace
using RaceDay_System.Models;
using RaceDay_System.Dtos;
using Xunit; // Keep this if you are using xUnit

namespace RaceDay_System.Test
{
    public class AuthTests
    {
        [Fact]
        public async Task Register_InvalidRole_ReturnsBadRequest()
        {
            // Arrange
            // We pass null! because the controller should reject the invalid role 
            // before it ever tries to use the UserManager or Configuration.
            var userManager = (UserManager<ApplicationUser>)null!;
            var config = (IConfiguration)null!;

            var controller = new AuthController(userManager, config);

            var dto = new RegisterDto
            {
                // MAKE SURE THESE MATCH YOUR RegisterDto PROPERTIES
                Email = "test@example.com",
                Password = "Password123!",
                Role = "InvalidRole"
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // Depending on how your controller returns the error, it might be 
            // badRequestResult.Value.ToString() or you might just check for BadRequest
            Assert.NotNull(badRequestResult.Value);
        }
    }
}