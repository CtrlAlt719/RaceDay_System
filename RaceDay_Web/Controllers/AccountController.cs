using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay_Web.Models;
using RaceDay_Web.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RaceDay_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        // GET: /Account/Profile
        [Authorize] // Must be logged in to see this page
        [HttpGet]
        public IActionResult Profile()
        {
            // We can get the user's details directly from the Cookie we created during login!
            var email = User.Identity?.Name ?? "Unknown";
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "No Role";

            var model = new ProfileViewModel
            {
                Email = email,
                Role = role
            };

            return View(model);
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. Call the API
            var success = await _apiService.LoginAsync(model.Email, model.Password);

            if (success)
            {
                // 2. Get the user's role from the token
                var role = _apiService.GetRoleFromToken();

                // 3. Create the user's identity (Cookie Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Role, role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 4. Sign them in (Saves the session!)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}