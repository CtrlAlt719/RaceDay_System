using Microsoft.AspNetCore.Mvc;
using RaceDay_Web.Services;
using RaceDay_Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace RaceDay_Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Events/Browse (For Participants and Public)
        public async Task<IActionResult> Browse()
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>("api/events");
            if (events == null) events = new List<EventViewModel>();
            return View(events);
        }

        // GET: /Events/Index (For Organisers to manage their events)
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>("api/events");
            if (events == null) events = new List<EventViewModel>();
            return View(events);
        }

        // GET: /Events/Create
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Events/Create
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var eventData = new
            {
                Name = model.Name,
                Description = model.Description,
                EventDate = model.EventDate,
                Venue = model.Venue,
                Status = "Upcoming"
            };

            var success = await _apiService.PostAsync("api/events", eventData);

            if (success)
            {
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to create event.");
            return View(model);
        }

        // GET: /Events/AddCategory?eventId=1
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult AddCategory(int eventId)
        {
            var model = new CreateCategoryViewModel { EventId = eventId };
            return View(model);
        }

        // POST: /Events/AddCategory
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> AddCategory(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var categoryData = new
            {
                Name = model.Name,
                DistanceKm = model.DistanceKm,
                Capacity = model.Capacity,
                EntryFee = model.EntryFee
            };

            var success = await _apiService.PostAsync($"api/events/{model.EventId}/categories", categoryData);

            if (success)
            {
                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to add category.");
            return View(model);
        }

        // GET: /Events/Enrolments?eventId=1
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public async Task<IActionResult> Enrolments(int eventId)
        {
            var eventDetails = await _apiService.GetAsync<EventViewModel>($"api/events/{eventId}");
            if (eventDetails == null) return NotFound();

            var enrolments = await _apiService.GetAsync<List<OrganiserEnrolmentViewModel>>($"api/events/{eventId}/enrolments");

            var model = new EventEnrolmentsViewModel
            {
                EventId = eventId,
                EventName = eventDetails.Name,
                Enrolments = enrolments ?? new List<OrganiserEnrolmentViewModel>()
            };

            return View(model);
        }

        // POST: /Events/AddResult
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> AddResult(int enrolmentId, int eventId, string finishTime, int position)
        {
            var resultData = new
            {
                EnrolmentId = enrolmentId,
                FinishTime = finishTime,
                Position = position
            };

            var success = await _apiService.PostAsync("api/results", resultData);

            if (success)
            {
                TempData["SuccessMessage"] = "Result captured successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to capture result.";
            }

            return RedirectToAction("Enrolments", new { eventId = eventId });
        }
    }
}