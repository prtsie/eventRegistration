using System.ComponentModel.DataAnnotations;

namespace EventRegistration.Database.Models.Users
{
    /// <summary> Модель пользователя </summary>
    public class User
    {
        /// <summary> Уникальный дентификатор пользователя </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary> Логин для идентификации пользователя </summary>
        [Required]
        [MaxLength(Constraints.MaxLoginLength)]
        public required string Login { get; set; }

        /// <summary> Пароль для аутентификации пользователя </summary>
        [Required]
        [MaxLength(Constraints.MaxPasswordLength)]
        public required string Password { get; set; }

        /// <summary> Роль пользователя, определяющая его привилегии </summary>
        public required Role Role { get; set; }
    }
}
