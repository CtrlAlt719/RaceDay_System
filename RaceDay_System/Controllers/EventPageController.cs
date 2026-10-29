using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_System.Data;
using RaceDay_System.Models;
using System.Security.Claims;

namespace RaceDay_System.Controllers
{
    public class EventPageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventPageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /EventPage
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        // GET: /EventPage/Create
        [Authorize(Roles = "Organiser")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /EventPage/Create
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event newEvent)
        {
            // 1. Get the logged-in Organiser's ID
            var organiserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (organiserId != null)
            {
                newEvent.OrganiserId = organiserId;

                // 2. THIS IS THE MAGIC LINE: 
                // We remove OrganiserId from validation so the form can save!
                ModelState.Remove("OrganiserId");
            }

            // 3. Now ModelState will be Valid
            if (ModelState.IsValid)
            {
                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If it still fails, show the errors on the page
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.Errors = errors;

            return View(newEvent);
        }
        // GET: /EventPage/Edit/5
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Edit(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();

            // Security: Ensure the logged-in organiser owns this event
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (eventItem.OrganiserId != userId) return Forbid();

            return View(eventItem);
        }

        // POST: /EventPage/Edit/5
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event updatedEvent)
        {
            if (id != updatedEvent.EventId) return NotFound();

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingEvent.OrganiserId != userId) return Forbid();

            if (ModelState.IsValid)
            {
                existingEvent.Name = updatedEvent.Name;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.EventDate = updatedEvent.EventDate;
                existingEvent.Venue = updatedEvent.Venue;
                existingEvent.Status = updatedEvent.Status;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(updatedEvent);
        }

        // POST: /EventPage/Delete/5
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (eventItem.OrganiserId != userId) return Forbid();

            // Delete related categories/enrolments first to avoid SQL errors
            var categories = await _context.Categories.Where(c => c.EventId == id).ToListAsync();
            _context.Categories.RemoveRange(categories);

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /EventPage/Enrol/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.EventId == eventId);

            if (category == null)
            {
                category = new Category { EventId = eventId, Name = "General Entry", DistanceKm = 10, Capacity = 100, EntryFee = 100 };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            // 1. Check if user is already enrolled
            var alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e => e.CategoryId == category.CategoryId && e.ParticipantId == userId);

            if (alreadyEnrolled)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this event!";
                return RedirectToAction(nameof(Index));
            }

            // 2. Check if the category is full
            var currentEnrolmentsCount = await _context.Enrolments
                .CountAsync(e => e.CategoryId == category.CategoryId);

            if (currentEnrolmentsCount >= category.Capacity)
            {
                TempData["ErrorMessage"] = "Sorry, this race is full!";
                return RedirectToAction(nameof(Index));
            }

            // 3. Save Enrolment
            var enrolment = new Enrolment
            {
                ParticipantId = userId,
                CategoryId = category.CategoryId,
                Status = "Confirmed"
            };
            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Successfully enrolled!";

            return RedirectToAction(nameof(Index));
        }
        // GET: /EventPage/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Get the event and include its categories
            var eventItem = await _context.Events
                .Include(e => e.Categories) // Assuming you have this navigation property
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // GET: /EventPage/MyEnrolments
        [Authorize] // Must be logged in
        public async Task<IActionResult> MyEnrolments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myEnrolments = await _context.Enrolments
                .Include(e => e.Category)
                .ThenInclude(c => c.Event) // Gets the event details via the category
                .Where(e => e.ParticipantId == userId)
                .ToListAsync();

            return View(myEnrolments);
        }
    }

}