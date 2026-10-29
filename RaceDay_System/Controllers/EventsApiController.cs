using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_System.Data;    // Needed for ApplicationDbContext
using RaceDay_System.Models;  // Needed for the Event class

namespace RaceDay_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsApiController : ControllerBase
    {
        // THIS IS THE MISSING LINE (Your database connection):
        private readonly ApplicationDbContext _context;

        // THIS IS THE MISSING CONSTRUCTOR:
        public EventsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EventsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        // GET: api/EventsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return eventItem;
        }

        // POST: api/EventsApi
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
        }
    }
}