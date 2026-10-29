using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RaceDay_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Participant"; // "Organiser" or "Participant"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation properties ─────────────────────────────
        // One organiser can create many events
        public ICollection<Event> OrganisedEvents { get; set; } = new List<Event>();

        // One participant can have many enrolments
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}