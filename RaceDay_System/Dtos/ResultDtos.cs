using System.ComponentModel.DataAnnotations;

namespace RaceDay_System.Dtos
{
    // ── Response for GET /api/results/me & GET /api/events/{eventId}/results
    public class ResultDto
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
        public string ResultStatus { get; set; } = string.Empty;
    }

    // ── POST /api/enrolments/{enrolmentId}/result ──────────────
    public class CreateResultDto
    {
        // "HH:MM:SS" — TimeSpan parses both "00:52:31" and "52:31"
        [Required(ErrorMessage = "Finish time is required.")]
        public TimeSpan FinishTime { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Position must be greater than 0.")]
        public int Position { get; set; }

        [Required(ErrorMessage = "Result status is required.")]
        [MaxLength(30)]
        public string ResultStatus { get; set; } = "Finished";
    }
}