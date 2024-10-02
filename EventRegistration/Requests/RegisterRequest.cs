using EventRegistration.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace EventRegistration.Requests
{
    public class RegisterRequest
    {
        /// <summary> Логин для идентификации пользователя </summary>
        [Required]
        [MaxLength(Constraints.MaxLoginLength)]
        public required string Login { get; set; }

        /// <summary> Пароль для аутентификации пользователя </summary>
        [Required]
        [MaxLength(Constraints.MaxPasswordLength)]
        public required string Password { get; set; }
    }
}
