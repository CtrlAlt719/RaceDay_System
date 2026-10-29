using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_System.Data;
using RaceDay_System.Dtos;
using RaceDay_System.Models;
using System.Security.Claims;

namespace RaceDay_System.Controllers
{
    [ApiController]
    [Route("api")]
    public class EnrolmentController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public EnrolmentController(ApplicationDbContext db) => _db = db;

        // POST: /api/categories/{categoryId}/enrolments  (Participant only)
        [HttpPost("categories/{categoryId}/enrolments")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Enrol(int categoryId, [FromBody] CreateEnrolmentDto dto)
        {
            // Validate the body categoryId matches the URL (if provided)
            if (dto.CategoryId != 0 && dto.CategoryId != categoryId)
                return BadRequest(new { message = "Category ID in body does not match URL." });

            var participantId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var category = await _db.Categories.FindAsync(categoryId);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            // Duplicate enrolment check
            var already = await _db.Enrolments.AnyAsync(e =>
                e.ParticipantId == participantId && e.CategoryId == categoryId);
            if (already)
                return Conflict(new { message = "You are already enrolled in this category." });

            // Capacity check
            var currentCount = await _db.Enrolments.CountAsync(e =>
                e.CategoryId == categoryId && e.Status != "Cancelled");
            if (currentCount >= category.Capacity)
                return Conflict(new { message = "This category is full." });

            var enrolment = new Enrolment
            {
                ParticipantId = participantId,
                CategoryId = categoryId,
                Status = "Confirmed",
                EnrolmentDate = DateTime.UtcNow
            };

            _db.Enrolments.Add(enrolment);
            await _db.SaveChangesAsync();

            return StatusCode(201, new
            {
                enrolment.EnrolmentId,
                enrolment.CategoryId,
                enrolment.EnrolmentDate,
                enrolment.Status
            });
        }

        // GET: /api/enrolments/me  (Participant only)
        [HttpGet("enrolments/me")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> MyEnrolments()
        {
            var participantId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var list = await _db.Enrolments
                .Where(e => e.ParticipantId == participantId)
                .OrderByDescending(e => e.EnrolmentDate)
                .Select(e => new EnrolmentDto
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    CategoryId = e.CategoryId,
                    EnrolmentDate = e.EnrolmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: /api/events/{eventId}/enrolments  (Organiser only)
        [HttpGet("events/{eventId}/enrolments")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> EventEnrolments(int eventId)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (ev.OrganiserId != currentUserId)
                return Forbid();

            var list = await _db.Enrolments
                .Where(e => e.Category != null && e.Category.EventId == eventId)
                .OrderBy(e => e.CategoryId)
                .ThenBy(e => e.EnrolmentDate)
                .Select(e => new EnrolmentDto
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    CategoryId = e.CategoryId,
                    EnrolmentDate = e.EnrolmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}