using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_System.Models
{
    public class Results
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        // Maps to SQL TIME column
        [Required]
        [Column(TypeName = "time")]
        public TimeSpan FinishTime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Position { get; set; }

        [Required]
        [MaxLength(30)]
        public string ResultStatus { get; set; } = "Finished";

        // ── Navigation property ───────────────────────────────
        [ForeignKey(nameof(EnrolmentId))]
        public Enrolment? Enrolment { get; set; }
    }
}