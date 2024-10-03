using EventRegistration.Database.Models.Events;
using MailKit.Net.Smtp;
using MimeKit;

namespace EventRegistration.Services.MailService
{
    public class MailService : IRegistrationCallbackSender
    {
        public static MailOptions Options { get; set; } = null!;

        public async Task SendCallbackAsync(Registration registration, CancellationToken cancellationToken)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(Options.OrganizationName, Options.OrganizationMail));
            emailMessage.To.Add(new MailboxAddress($"{registration.Firstname} {registration.Surname} {registration.Patronymic}".TrimEnd(),
                registration.Email));
            emailMessage.Subject = "Запись на мероприятие";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Вы записались на мероприятие {registration.Event.Name}" };

            using var client = new SmtpClient();
            await client.ConnectAsync(Options.SmtpServer, Options.Port, Options.UseSsl, cancellationToken);
            await client.AuthenticateAsync(Options.SmtpServerLogin, Options.SmtpServerPassword, cancellationToken);
            await client.SendAsync(emailMessage, cancellationToken);

            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
