using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_System.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentId { get; set; }

        // Note: string because ApplicationUser.Id is a string (Identity)
        [Required]
        public string ParticipantId { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Confirmed"; // Pending | Confirmed | Cancelled

        // ── Navigation properties ─────────────────────────────
        [ForeignKey(nameof(ParticipantId))]
        public ApplicationUser? Participant { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        // One enrolment has zero or one result
        public Results? Result { get; set; }
    }
}