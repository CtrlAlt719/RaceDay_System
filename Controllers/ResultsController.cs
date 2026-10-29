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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ResultsController(ApplicationDbContext db) => _db = db;

        // POST: /api/enrolments/{enrolmentId}/result  (Organiser only)
        [HttpPost("enrolments/{enrolmentId}/result")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateResult(int enrolmentId, [FromBody] CreateResultDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var enrolment = await _db.Enrolments
                .Include(e => e.Category)
                    .ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == enrolmentId);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (enrolment.Category?.Event == null || enrolment.Category.Event.OrganiserId != currentUserId)
                return Forbid();

            // Duplicate result check
            if (await _db.Results.AnyAsync(r => r.EnrolmentId == enrolmentId))
                return Conflict(new { message = "A result already exists for this enrolment." });

            var result = new Models.Results
            {
                EnrolmentId = enrolmentId,
                FinishTime = dto.FinishTime,
                Position = dto.Position,
                ResultStatus = dto.ResultStatus
            };

            _db.Results.Add(result);
            await _db.SaveChangesAsync();

            return StatusCode(201, new
            {
                result.ResultId,
                result.EnrolmentId,
                result.FinishTime,
                result.Position,
                result.ResultStatus
            });
        }

        // GET: /api/results/me  (Participant only)
        [HttpGet("results/me")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> MyResults()
        {
            var participantId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var list = await _db.Results
                .Where(r => r.Enrolment != null && r.Enrolment.ParticipantId == participantId)
                .OrderBy(r => r.Position)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultId,
                    EnrolmentId = r.EnrolmentId,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    ResultStatus = r.ResultStatus
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: /api/events/{eventId}/results  (Organiser only)
        [HttpGet("events/{eventId}/results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> EventResults(int eventId)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (ev.OrganiserId != currentUserId)
                return Forbid();

            var list = await _db.Results
                .Where(r => r.Enrolment != null &&
                            r.Enrolment.Category != null &&
                            r.Enrolment.Category.EventId == eventId)
                .OrderBy(r => r.Position)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultId,
                    EnrolmentId = r.EnrolmentId,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    ResultStatus = r.ResultStatus
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}