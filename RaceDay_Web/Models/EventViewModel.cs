using System.Collections.Generic;

namespace RaceDay_Web.Models
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Venue { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}