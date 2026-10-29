using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_System.Data;
using RaceDay_System.Models;
using System.Security.Claims;

namespace RaceDay_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrolmentsApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/EnrolmentsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetEnrolments()
        {
            // Includes related data so you can see the Participant and Category details
            return await _context.Enrolments
                .Include(e => e.Participant)  // Changed from Event to Participant
                .Include(e => e.Category)
                .ToListAsync();
        }

        // POST: api/EnrolmentsApi
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Enrolment>> CreateEnrolment(Enrolment newEnrolment)
        {
            // 1. Get the currently logged-in user's ID from the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            // 2. Force the ParticipantId to be the logged-in user
            newEnrolment.ParticipantId = userId; // Changed from UserId to ParticipantId

            // 3. Check if they are already enrolled in this Category
            var alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e => e.CategoryId == newEnrolment.CategoryId && e.ParticipantId == userId); // Changed UserId to ParticipantId

            if (alreadyEnrolled)
            {
                return BadRequest("You are already enrolled in this category.");
            }

            // 4. Save to database
            _context.Enrolments.Add(newEnrolment);
            await _context.SaveChangesAsync();

            return Ok(newEnrolment);
        }
    }
}