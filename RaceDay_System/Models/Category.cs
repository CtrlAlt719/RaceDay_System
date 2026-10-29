using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_System.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000)]
        [Column(TypeName = "decimal(6,2)")]
        public decimal DistanceKm { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Capacity { get; set; }

        [Required]
        [Range(0, 1000000)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal EntryFee { get; set; } = 0.00m;

        // ── Navigation properties ─────────────────────────────
        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}