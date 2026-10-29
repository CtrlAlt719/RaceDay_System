using System.ComponentModel.DataAnnotations;

namespace RaceDay_System.Dtos
{
    // ── Response for GET /api/enrolments/me & GET /api/events/{eventId}/enrolments
    public class EnrolmentDto
    {
        public int EnrolmentId { get; set; }
        public string ParticipantId { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // ── POST /api/categories/{categoryId}/enrolments ───────────
    // The categoryId is taken from the URL; the body is optional.
    // If CategoryId is supplied in the body it must match the URL.
    public class CreateEnrolmentDto
    {
        public int CategoryId { get; set; } = 0;
    }
}