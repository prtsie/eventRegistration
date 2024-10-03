using EventRegistration.Database.Models.Events;

namespace EventRegistration.Services.MailService
{
    public interface IRegistrationCallbackSender
    {
        Task SendCallbackAsync(Registration registration, CancellationToken cancellationToken);
    }
}
