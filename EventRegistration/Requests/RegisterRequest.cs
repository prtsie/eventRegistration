using EventRegistration.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace EventRegistration.Requests
{
    public class RegisterRequest
    {
        /// <summary> Логин для идентификации пользователя </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        [MaxLength(Constraints.MaxLoginLength, ErrorMessage = "Поле слишком длинное")]
        public required string Login { get; set; }

        /// <summary> Пароль для аутентификации пользователя </summary>
        [Required(ErrorMessage = "Это поле обязательно")]
        [MaxLength(Constraints.MaxPasswordLength, ErrorMessage = "Поле слишком длинное")]
        public required string Password { get; set; }
    }
}
