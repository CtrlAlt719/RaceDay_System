using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay_Web.Models
{
    public class CreateEventViewModel
    {
        [Required(ErrorMessage = "Event Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public string Venue { get; set; } = string.Empty;
    }
}