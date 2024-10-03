using EventRegistration.Database.Models.Events;

namespace EventRegistration.Services.CardGenerator
{
    public interface ICardGenerator
    {
        Stream Generate(Registration registration);
    }
}
