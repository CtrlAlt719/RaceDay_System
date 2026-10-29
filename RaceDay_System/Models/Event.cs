using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_System.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string OrganiserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Venue { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Upcoming";

        // ── Navigation properties ─────────────────────────────
        [ForeignKey(nameof(OrganiserId))]
        public ApplicationUser? Organiser { get; set; }

        public ICollection<Category> Categories { get; set; } = new List<Category>();

        // One event has zero or one route
        public RaceDayRoute? Route { get; set; }
    }
}