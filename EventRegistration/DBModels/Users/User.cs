namespace EventRegistration.DBModels.Users
{
    /// <summary> Модель пользователя </summary>
    public class User
    {
        /// <summary> Уникальный дентификатор пользователя </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary> Логин для идентификации пользователя </summary>
        public required string Login { get; set; }

        /// <summary> Пароль для аутентификации пользователя </summary>
        public required string Password { get; set; }

        /// <summary> Роль пользователя, определяющая его привилегии </summary>
        public required Role Role { get; set; }
    }
}
