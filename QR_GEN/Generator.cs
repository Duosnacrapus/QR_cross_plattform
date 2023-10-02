using System;
using System.IO;
using SixLabors.ImageSharp;
using QRCoder;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

public class Generator
{
    public static void GenerateQRCodeWithLogo(string textToEncode, string logoPath, string outputPath, double fractionOfScale, QRCodeGenerator.ECCLevel eccLevel, SixLabors.ImageSharp.PixelFormats.Rgba32 color)
    {
        // QRCodeGenerator erstellen
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(textToEncode, eccLevel);

        // QRCode in ein ImageSharp-Bild konvertieren
        using (QRCode qrCode = new QRCode(qrCodeData))
        {
            int qrCodeSize = qrCodeData.ModuleMatrix.Count; // Größe des QR-Codes

            using (Image<Rgba32> qrCodeImage = (Image<Rgba32>)qrCode.GetGraphic(20)) // QR-Code-Größe einstellen
            using (Image<Rgba32> logo = Image.Load<Rgba32>(logoPath)) // Pfad zu Ihrem Logo-Bild
            {
                for (int x = 0; x < qrCodeSize; x++)
                {
                    for (int y = 0; y < qrCodeSize; y++)
                    {
                        // Hier setzen wir den Pixel einfach auf die gewünschte Farbe, ohne den Wert zu prüfen
                        qrCodeImage[x, y] = new Rgba32(255, 255, 0); // Gelb
                    }
                }
                // Position des Logos im QR-Code
                double xPos = (qrCodeImage.Width - (qrCodeImage.Width / fractionOfScale)) / 2;
                double yPos = (qrCodeImage.Height - (qrCodeImage.Height / fractionOfScale)) / 2;

                // Logo auf 1/3 des QR-Codes skalieren
                logo.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size((int)((Convert.ToDouble(qrCodeImage.Width)) / fractionOfScale), (int)((Convert.ToDouble(qrCodeImage.Height) / fractionOfScale))),
                    Mode = ResizeMode.Max
                }));
                // Hintergrundfarbe des QR-Codes setzen
                Rgba32 colortest = new Rgba32(255, 0, 0, 255);
                qrCodeImage.Mutate(ctx => ctx.BackgroundColor(colortest));
                // Logo in den QR-Code einfügen
                qrCodeImage.Mutate(ctx => ctx
                    .DrawImage(logo, new Point((int)xPos, (int)yPos), 1f)
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
