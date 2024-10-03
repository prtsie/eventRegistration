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
        public const int MaxEventHostNameLength = 200;
        public const int MaxEventDescriptionLength = 1000;

        //Для записей
        public const int MaxPhoneNumLength = 13;
        public const int MaxEmailLength = 254;
        public const int MaxNameLength = 50;
    }
}
