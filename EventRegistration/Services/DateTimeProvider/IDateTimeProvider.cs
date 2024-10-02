namespace EventRegistration.Services.DateTimeProvider
{
    /// <summary> Интерфейс для сервиса, возвращающего текущее время </summary>
    public interface IDateTimeProvider
    {
        DateTimeOffset Now();
    }
}
