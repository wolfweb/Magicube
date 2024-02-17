using System;
using System.IO;
using QRCoder;
using static QRCoder.QRCodeGenerator;

#if NET6_0_OR_GREATER
using SixLabors.ImageSharp;
#else
using System.Drawing;
#endif

namespace Magicube.Media.Images {
    public class QrCodeService : IDisposable{
        private readonly QRCodeGenerator _generator;

        public QrCodeService() {
            _generator = new QRCodeGenerator();
        }

#if NET6_0_OR_GREATER
        public Image QrCode(string data, ECCLevel level) {
            using (QRCodeData qrCodeData = _generator.CreateQrCode(data, level)) {
                using (var qrCode = new BitmapByteQRCode(qrCodeData)) {
                    return Image.Load(new MemoryStream(qrCode.GetGraphic(20)));
                }
            }
        }
#else
        public Image QrCode(string data, ECCLevel level) {
            using (QRCodeData qrCodeData = _generator.CreateQrCode(data, level)) {
                using (QRCode qrCode = new QRCode(qrCodeData)) {
                    return qrCode.GetGraphic(20, Color.Black, Color.White, true);
                }
            }
        }
#endif

        public void Dispose() {
            _generator.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
