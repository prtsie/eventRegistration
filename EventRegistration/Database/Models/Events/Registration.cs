using System.ComponentModel.DataAnnotations;
using EventRegistration.Database.Models.Users;

namespace EventRegistration.Database.Models.Events
{
    /// <summary> Модель записи на мероприятие </summary>
    public class Registration
    {
        /// <summary> Уникальный идентификатор </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary> Записавшийся пользователь </summary>
        /// <remarks> Навигационное свойство </remarks>
        public required User User { get; set; }

        /// <summary> Мероприятие </summary>
        /// <remarks> Навигационное свойство </remarks>
        public required Event Event { get; set; }

        // <summary> Имя </summary>
        [Required]
        [MaxLength(Constraints.MaxNameLength)]
        public required string Firstname { get; set; }

        /// <summary> Фамилия </summary>
        [Required]
        [MaxLength(Constraints.MaxNameLength)]
        public required string Surname { get; set; }

        /// <summary> Отчество </summary>
        [MaxLength(Constraints.MaxNameLength)]
        public string? Patronymic { get; set; }

        /// <summary> Номер телефона </summary>
        [Required]
        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$")]
        [MaxLength(Constraints.MaxPhoneNumLength)]
        public required string PhoneNum { get; set; }

        /// <summary> Почта </summary>
        [Required]
        [EmailAddress]
        [MaxLength(Constraints.MaxEmailLength)]
        public required string Email { get; set; }
    }
}
