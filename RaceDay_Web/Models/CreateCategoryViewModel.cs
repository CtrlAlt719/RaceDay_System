using System.ComponentModel.DataAnnotations;

namespace RaceDay_Web.Models
{
    public class CreateCategoryViewModel
    {
        public int EventId { get; set; } // We need this to link the category to the event

        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 1000, ErrorMessage = "Distance must be between 0.1 and 1000 km")]
        public decimal DistanceKm { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Entry fee must be 0 or more")]
        public decimal EntryFee { get; set; }
    }
}