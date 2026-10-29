using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_System.Data;
using RaceDay_System.Dtos;
using System.Security.Claims;

namespace RaceDay_System.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db) => _db = db;

        // PUT: /api/categories/{id}  (Organiser only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cat = await _db.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (cat == null) return NotFound(new { message = "Category not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (cat.Event == null || cat.Event.OrganiserId != currentUserId)
                return Forbid();

            // Uniqueness check (eventId, name) - excluding self
            var duplicate = await _db.Categories.AnyAsync(c =>
                c.EventId == cat.EventId &&
                c.CategoryId != id &&
                c.Name == dto.Name);
            if (duplicate)
                return Conflict(new { message = "Another category with that name already exists for this event." });

            cat.Name = dto.Name;
            cat.DistanceKm = dto.DistanceKm;
            cat.Capacity = dto.Capacity;
            cat.EntryFee = dto.EntryFee;

            await _db.SaveChangesAsync();
            return Ok(new { message = "Category updated.", cat.CategoryId });
        }

        // DELETE: /api/categories/{id}  (Organiser only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (cat == null) return NotFound(new { message = "Category not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (cat.Event == null || cat.Event.OrganiserId != currentUserId)
                return Forbid();

            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
