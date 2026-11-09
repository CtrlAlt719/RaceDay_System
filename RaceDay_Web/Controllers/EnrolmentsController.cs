using Microsoft.AspNetCore.Mvc;
using RaceDay_Web.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RaceDay_Web.Models;

namespace RaceDay_Web.Controllers
{
    [Authorize(Roles = "Participant")]
    public class EnrolmentsController : Controller
    {
        private readonly ApiService _apiService;

        public EnrolmentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Enrolments/Create?eventId=1
        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
        {
            var eventDetails = await _apiService.GetAsync<EventViewModel>($"api/events/{eventId}");
            if (eventDetails == null) return NotFound();

            ViewBag.EventId = eventId;
            ViewBag.EventName = eventDetails.Name;

            var categories = await _apiService.GetAsync<List<CategoryViewModel>>($"api/events/{eventId}/categories");
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            return View();
        }

        // POST: /Enrolments/Enrol
        [HttpPost]
        public async Task<IActionResult> Enrol([FromForm] int eventId, [FromForm] int categoryId)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            // Your API expects the categoryId in the URL, and ParticipantId in the body
            var enrolData = new { ParticipantId = userEmail };

            // Hitting the specific endpoint from your Swagger
            var success = await _apiService.PostAsync($"api/categories/{categoryId}/enrolments", enrolData);

            if (success)
            {
                TempData["SuccessMessage"] = "Successfully enrolled!";
                return RedirectToAction("MyEnrolments");
            }

            TempData["ErrorMessage"] = "Failed to enrol. You might already be enrolled in this category.";
            return RedirectToAction("Browse", "Events");
        }

        // GET: /Enrolments/MyEnrolments
        public async Task<IActionResult> MyEnrolments()
        {
            // Using the correct endpoint "api/enrolments/me"
            var enrolments = await _apiService.GetAsync<List<EnrolmentViewModel>>("api/enrolments/me");

            if (enrolments == null) enrolments = new List<EnrolmentViewModel>();

            return View(enrolments);
        }
    }
}