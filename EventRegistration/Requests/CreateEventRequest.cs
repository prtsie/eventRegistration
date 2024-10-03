using System.ComponentModel.DataAnnotations;
using EventRegistration.Database.Models;

namespace EventRegistration.Requests
{
    public class CreateEventRequest
    {

        [Required]
        [MaxLength(Constraints.MaxEventNameLength)]
        public required string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(Constraints.MaxEventHostNameLength)]
        public required string HostName { get; set; }

        [Required]
        public required string Description { get; set; }
    }
}
