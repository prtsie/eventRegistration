namespace EventRegistration.Database.Models
{
    /// <summary> Статический класс с ограничениями на длину полей </summary>
    public static class Constraints
    {
        // Для модели юзера
        public const int MaxLoginLength = 50;
        public const int MaxPasswordLength = 50;

        // Для модели мероприятия
        public const int MaxEventNameLength = 200;
    }
}
