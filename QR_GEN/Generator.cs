using System;
using System.IO;
using SixLabors.ImageSharp;
using QRCoder;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

public class Generator
{
    public static void GenerateQRCodeWithLogo(string textToEncode, string logoPath, string outputPath, int fractionOfScale)
    {
        // QRCodeGenerator erstellen
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.H);

        // QRCode in ein ImageSharp-Bild konvertieren
        using (QRCode qrCode = new QRCode(qrCodeData))
        {
            int qrCodeSize = qrCodeData.ModuleMatrix.Count; // Größe des QR-Codes

            using (Image<Rgba32> qrCodeImage = (Image<Rgba32>)qrCode.GetGraphic(20)) // QR-Code-Größe einstellen
            using (Image<Rgba32> logo = Image.Load<Rgba32>(logoPath)) // Pfad zu Ihrem Logo-Bild
            {
                // Position des Logos im QR-Code
                int xPos = (qrCodeImage.Width - (qrCodeImage.Width / fractionOfScale)) / 2;
                int yPos = (qrCodeImage.Height - (qrCodeImage.Height / fractionOfScale)) / 2;

                // Logo auf 1/3 des QR-Codes skalieren
                logo.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(qrCodeImage.Width / fractionOfScale, qrCodeImage.Height / fractionOfScale),
                    Mode = ResizeMode.Max
                }));

                // Logo in den QR-Code einfügen
                qrCodeImage.Mutate(ctx => ctx
                    .DrawImage(logo, new Point(xPos, yPos), 1f)
                );

                // Bild speichern
                using (FileStream stream = File.Create(outputPath))
                {
                    qrCodeImage.Save(stream, new PngEncoder());
                }
            }
        }

        Console.WriteLine("QR-Code mit Logo wurde erstellt und gespeichert.");
    }
}
