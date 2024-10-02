using System.ComponentModel.DataAnnotations;
using EventRegistration.Database.Models;

namespace EventRegistration.Requests
{
    public class AuthenticationRequest
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
