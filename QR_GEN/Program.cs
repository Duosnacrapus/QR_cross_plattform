using SixLabors.ImageSharp;
using QRCoder;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


class Program
{
    static void Main(string[] args)
    {
        Generator.GenerateQRCodeWithLogo("QRTEXT", "PFAD/LOGO.png", "PFAD/qr_code_with_logo.png", 7/2); //Int ist skalierung
    }
}
