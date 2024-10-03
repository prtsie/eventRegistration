using EventRegistration.Database.Models.Events;

namespace EventRegistration.Services.QrGenerator
{
    public interface IQrGenerator
    {
        Stream GenerateQrForRegistration(Registration registration);
    }
}
