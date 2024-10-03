using System.Text;
using EventRegistration.Database.Models.Events;
using EventRegistration.Services.QrGenerator;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EventRegistration.Services.CardGenerator
{
    public class PdfCardGenerator(IQrGenerator qrGenerator) : ICardGenerator
    {
        public Stream Generate(Registration registration)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('\t', 2);
            stringBuilder.Append($"{registration.Firstname} {registration.Surname} {registration.Patronymic}".TrimEnd());
            stringBuilder.Append($", вы участвуете в мероприятии {registration.Event.Name}");
            stringBuilder.Append(", вам понадобится QR-код ниже, чтобы попасть на мероприятие.");

            var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.DefaultTextStyle(s => s.FontSize(20));

                    page.Header()
                        .Text("Запись на мероприятие")
                        .AlignCenter()
                        .FontSize(30);

                    page.Content()
                        .Text(stringBuilder.ToString());

                    page.Footer()
                        .Image(qrGenerator.GenerateQrForRegistration(registration));
                });
            }).GeneratePdf(stream);

            return stream;
        }
    }
}
