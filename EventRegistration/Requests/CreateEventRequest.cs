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

        public Guid HostId { get; set; } // Идентификатор организатора
    }
}
