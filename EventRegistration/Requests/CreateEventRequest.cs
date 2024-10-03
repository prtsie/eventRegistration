using System.ComponentModel.DataAnnotations;
using EventRegistration.Database.Models;

namespace EventRegistration.Requests
{
    public class CreateEventRequest
    {
        /// <summary> Название </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        [MaxLength(Constraints.MaxEventNameLength, ErrorMessage = "Поле слишком длинное")]
        public required string Name { get; set; }

        /// <summary> Дата проведения </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        public DateTime Date { get; set; }

        /// <summary> Организатор </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        [MaxLength(Constraints.MaxEventHostNameLength, ErrorMessage = "Поле слишком длинное")]
        public required string HostName { get; set; }

        /// <summary> Описание </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        public required string Description { get; set; }
    }
}
