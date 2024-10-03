using System.ComponentModel.DataAnnotations;
using EventRegistration.Database.Models;

namespace EventRegistration.Requests
{
    public class AuthenticationRequest
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
