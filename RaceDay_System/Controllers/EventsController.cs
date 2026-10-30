using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_System.Data;
using RaceDay_System.Dtos;
using RaceDay_System.Models;
using System.Security.Claims;

namespace RaceDay_System.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /api/events  (public)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.Events
                .AsNoTracking()
                .OrderBy(e => e.EventDate)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    OrganiserId = e.OrganiserId,
                    Name = e.Name,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Venue = e.Venue,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: /api/events/{id}  (public)
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _db.Events.AsNoTracking().FirstOrDefaultAsync(x => x.EventId == id);
            if (e == null) return NotFound(new { message = "Event not found." });

            return Ok(new EventDto
            {
                EventId = e.EventId,
                OrganiserId = e.OrganiserId,
                Name = e.Name,
                Description = e.Description,
                EventDate = e.EventDate,
                Venue = e.Venue,
                Status = e.Status
            });
        }

        // POST: /api/events  (Organiser only)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organiserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var ev = new Event
            {
                OrganiserId = organiserId,
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Venue = dto.Venue,
                Status = dto.Status
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return StatusCode(201, new
            {
                ev.EventId,
                ev.Name,
                ev.EventDate,
                ev.Venue,
                ev.Status
            });
        }

        // PUT: /api/events/{id}  (Organiser only — must own the event)
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound(new { message = "Event not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (ev.OrganiserId != currentUserId)
                return Forbid();

            ev.Name = dto.Name;
            ev.Description = dto.Description;
            ev.EventDate = dto.EventDate;
            ev.Venue = dto.Venue;
            ev.Status = dto.Status;

            await _db.SaveChangesAsync();
            return Ok(new { message = "Event updated.", ev.EventId });
        }

        // DELETE: /api/events/{id}  (Organiser only — must own the event)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound(new { message = "Event not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (ev.OrganiserId != currentUserId)
                return Forbid();

            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of all race from the database
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("{eventId}/categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            if (!await _db.Events.AnyAsync(e => e.EventId == eventId))
                return NotFound(new { message = "Event not found." });

            var cats = await _db.Categories
                .Where(c => c.EventId == eventId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    EventId = c.EventId,
                    Name = c.Name,
                    DistanceKm = c.DistanceKm,
                    Capacity = c.Capacity,
                    EntryFee = c.EntryFee
                })
                .ToListAsync();

            return Ok(cats);
        }

        // POST: /api/events/{eventId}/categories  (Organiser only)
        [HttpPost("{eventId}/categories")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> AddCategory(int eventId, [FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (ev.OrganiserId != currentUserId)
                return Forbid();

            // Uniqueness check (eventId, name)
            var duplicate = await _db.Categories.AnyAsync(c => c.EventId == eventId && c.Name == dto.Name);
            if (duplicate)
                return Conflict(new { message = "A category with that name already exists for this event." });

            var cat = new Category
            {
                EventId = eventId,
                Name = dto.Name,
                DistanceKm = dto.DistanceKm,
                Capacity = dto.Capacity,
                EntryFee = dto.EntryFee
            };

            _db.Categories.Add(cat);
            await _db.SaveChangesAsync();

            return StatusCode(201, new
            {
                cat.CategoryId,
                cat.Name,
                cat.DistanceKm,
                cat.Capacity,
                cat.EntryFee
            });
        }
    }
}