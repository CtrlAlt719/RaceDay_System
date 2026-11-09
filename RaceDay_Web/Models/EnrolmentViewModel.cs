namespace RaceDay_Web.Models
{
    public class EnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public string Status { get; set; } = string.Empty;

        // Assuming your API returns the Event details nested inside the Enrolment
        public EventViewModel? Event { get; set; }
        public CategoryViewModel? Category { get; set; }
    }

}