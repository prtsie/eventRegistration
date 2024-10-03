using EventRegistration.Database.Models.Events;
using EventRegistration.Services.CardGenerator;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace EventRegistration.Services.MailService
{
    public class MailService(ICardGenerator cardGenerator) : IRegistrationCallbackSender
    {
        public static MailOptions Options { get; set; } = null!;

        public async Task SendCallbackAsync(Registration registration, CancellationToken cancellationToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Options.OrganizationName, Options.OrganizationMail));
            message.To.Add(new MailboxAddress($"{registration.Firstname} {registration.Surname} {registration.Patronymic}".TrimEnd(),
                registration.Email));
            message.Subject = "Запись на мероприятие";;

            var body = new TextPart(TextFormat.Plain)
            {
                Text = $"Вы записались на мероприятие {registration.Event.Name}, к этому письму приложен бейдж участника в формате pdf"
            };

            var attachment = new MimePart("file", "pdf")
            {
                Content = new MimeContent(cardGenerator.Generate(registration)),
                ContentDisposition = new(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "event member card.pdf"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(attachment);

            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(Options.SmtpServer, Options.Port, Options.UseSsl, cancellationToken);
            await client.AuthenticateAsync(Options.SmtpServerLogin, Options.SmtpServerPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);

            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
