using System.ComponentModel.DataAnnotations;

namespace RaceDay_System.Dtos
{
    // ── Response for GET /api/events  &  GET /api/events/{id} ──
    public class EventDto
    {
        public int EventId { get; set; }
        public string OrganiserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    // ── POST /api/events ───────────────────────────────────────
    public class CreateEventDto
    {
        [Required(ErrorMessage = "Event name is required.")]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Venue is required.")]
        [MaxLength(200)]
        public string Venue { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Upcoming|Open|Closed|Completed)$",
            ErrorMessage = "Status must be Upcoming, Open, Closed, or Completed.")]
        public string Status { get; set; } = "Upcoming";
    }

    // ── PUT /api/events/{id} ───────────────────────────────────
    public class UpdateEventDto : CreateEventDto { }
}
