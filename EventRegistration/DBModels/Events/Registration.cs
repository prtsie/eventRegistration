using EventRegistration.DBModels.Users;

namespace EventRegistration.DBModels.Events
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
    }
}
