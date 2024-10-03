using System.ComponentModel.DataAnnotations;
using EventRegistration.Database.Models;

namespace EventRegistration.Requests
{
    public class CreateEventRequest
    {
        /// <summary> Название </summary>
        [Required]
        [MaxLength(Constraints.MaxEventNameLength)]
        public required string Name { get; set; }

        /// <summary> Дата проведения </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary> Организатор </summary>
        [Required]
        [MaxLength(Constraints.MaxEventHostNameLength)]
        public required string HostName { get; set; }

        /// <summary> Описание </summary>
        [Required]
        public required string Description { get; set; }
    }
}
