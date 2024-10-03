using System.Text;
using EventRegistration.Database.Models.Events;
using Net.Codecrete.QrCodeGenerator;

namespace EventRegistration.Services.QrGenerator
{
    public class QrGenerator : IQrGenerator
    {
        public Stream GenerateQrForRegistration(Registration registration)
        {
            var qr = QrCode.EncodeText(registration.Id.ToString(), QrCode.Ecc.Low);
            var bitmap = qr.ToBmpBitmap();
            return new MemoryStream(bitmap);
        }
    }
}
