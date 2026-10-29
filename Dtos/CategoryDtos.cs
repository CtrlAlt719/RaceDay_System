using System.ComponentModel.DataAnnotations;

namespace RaceDay_System.Dtos
{
    // the Response for GET /api/events/{eventId}/categories
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public int Capacity { get; set; }
        public decimal EntryFee { get; set; }
    }

    // ── POST /api/events/{eventId}/categories ──────────────────
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Distance is required.")]
        [Range(0.01, 1000, ErrorMessage = "Distance must be greater than 0 km.")]
        public decimal DistanceKm { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 100000, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Entry fee is required.")]
        [Range(0, 1000000, ErrorMessage = "Entry fee cannot be negative.")]
        public decimal EntryFee { get; set; }
    }

    // ── PUT /api/categories/{id} ───────────────────────────────
    public class UpdateCategoryDto : CreateCategoryDto { }
}