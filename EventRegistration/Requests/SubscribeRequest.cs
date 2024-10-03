using System.ComponentModel.DataAnnotations;
using EventRegistration.Database.Models;

namespace EventRegistration.Requests
{
    /// <summary> Запрос для записи на событие </summary>
    public class SubscribeRequest
    {
        /// <summary> Имя </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        [MaxLength(Constraints.MaxNameLength, ErrorMessage = "Поле слишком длинное")]
        public required string Firstname { get; set; }

        /// <summary> Фамилия </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        [MaxLength(Constraints.MaxNameLength, ErrorMessage = "Поле слишком длинное")]
        public required string Surname { get; set; }

        /// <summary> Отчество </summary>
        [MaxLength(Constraints.MaxNameLength, ErrorMessage = "Поле слишком длинное")]
        public string? Patronymic { get; set; }

        /// <summary> Номер телефона </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "Введите правильный номер телефона")]
        [MaxLength(Constraints.MaxPhoneNumLength, ErrorMessage = "Поле слишком длинное")]
        public required string PhoneNum { get; set; }

        /// <summary> Почта </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        [EmailAddress(ErrorMessage = "Введите правильный адрес")]
        [MaxLength(Constraints.MaxEmailLength, ErrorMessage = "Поле слишком длинное")]
        public required string Email { get; set; }
    }
}
