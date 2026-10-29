using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_System.Models
{
    public class RaceDayRoute
    {
        [Key]
        public int RouteId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(200)]
        public string StartLocation { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string EndLocation { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal DistanceKm { get; set; }

        [MaxLength(500)]
        public string? RouteUrl { get; set; }

        // Navigation property
        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }
    }
}