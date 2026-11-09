using System.Collections.Generic;

namespace RaceDay_Web.Models
{
    public class OrganiserEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public string ParticipantId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Nested object 
        public CategoryViewModel? Category { get; set; }

        // Fallback flat property 
        public string? CategoryName { get; set; }
    }

    public class EventEnrolmentsViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public List<OrganiserEnrolmentViewModel> Enrolments { get; set; } = new List<OrganiserEnrolmentViewModel>();
    }
}