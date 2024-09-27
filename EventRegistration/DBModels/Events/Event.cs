using EventRegistration.DBModels.Users;

namespace EventRegistration.DBModels.Events
{
    /// <summary> Модель мероприятия </summary>
    public class Event
    {
        /// <summary> Уникальный идентификатор события </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary> Название мероприятия </summary>
        public required string Name { get; set; }

        /// <summary> Дата и время проведения </summary>
        public required DateTime Date { get; set; }

        /// <summary> Организатор </summary>
        /// <remarks> Навигационное свойство </remarks>
        public required User Host { get; set; }
    }
}
