namespace EventRegistration.Services.DateTimeProvider
{
    /// <summary> Сервис, возвращающий текущее системное время </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now() => DateTimeOffset.Now;
    }
}
