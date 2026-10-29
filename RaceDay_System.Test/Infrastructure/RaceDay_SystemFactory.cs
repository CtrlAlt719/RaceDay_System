using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_System.Dtos;
using System.Security.Claims;

namespace RaceDay_System.Test.Infrastructure
{
    public static class RaceDay_SystemFactory
    {
        public static ControllerContext GetOrganiserContext(string userId = "organiser-123")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "Organiser")
            }, "mock"));

            return new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        }

        public static ControllerContext GetParticipantContext(string userId = "participant-123")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "Participant")
            }, "mock"));

            return new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        }
    }
}