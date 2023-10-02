using System;
using System.IO;
using SixLabors.ImageSharp;
using QRCoder;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.IO;


public class Generator
{
    public static void GenerateQRCodeWithLogo(string textToEncode, string logoPath, string outputPath, double fractionOfScale, QRCodeGenerator.ECCLevel eccLevel,
     int[] frontColor, int[] backColor,
     bool changeLogoColor, bool transparentBackground)
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
                if (changeLogoColor)
                {
                    // Position des Logos im QR-Code
                    double xPos = (qrCodeImage.Width - (qrCodeImage.Width / fractionOfScale)) / 2;
                    double yPos = (qrCodeImage.Height - (qrCodeImage.Height / fractionOfScale)) / 2;

                    // Logo auf 1/3 des QR-Codes skalieren
                    logo.Mutate(ctx => ctx.Resize(new ResizeOptions
                    {
                        Size = new Size((int)((Convert.ToDouble(qrCodeImage.Width)) / fractionOfScale), (int)((Convert.ToDouble(qrCodeImage.Height) / fractionOfScale))),
                        Mode = ResizeMode.Max
                    }));
                    // Logo in den QR-Code einfügen
                    qrCodeImage.Mutate(ctx => ctx
                        .DrawImage(logo, new Point((int)xPos, (int)yPos), 1f)
                    );
                }

                // Loop durch jedes pixel um farbe zu aendern
                for (int y = 0; y < qrCodeImage.Height; y++)
                {
                    for (int x = 0; x < qrCodeImage.Width; x++)
                    {
                        // Get the pixel color at (x, y)
                        Rgba32 pixelColor = qrCodeImage[x, y];

                        // Check if the pixel is white 
                        if (pixelColor.R > 240 && pixelColor.G > 240 && pixelColor.B > 240)
                        {
                            // farbe des Hintergrunds bzw transparent
                            // pixelColor.A = 0;
                            if (transparentBackground)
                            { pixelColor.A = 0; }
                            else
                            {
                                pixelColor.R = (byte)backColor[0];
                                pixelColor.G = (byte)backColor[1];
                                pixelColor.B = (byte)backColor[2];
                            }

                        }
                        else
                        {
                            // Farbe des qr
                            pixelColor.R = (byte)frontColor[0];
                            pixelColor.G = (byte)frontColor[1];
                            pixelColor.B = (byte)frontColor[2];
                        }

                        // Update the pixel color in the image
                        qrCodeImage[x, y] = pixelColor;
                    }
                }
                if (!changeLogoColor)
                {
                    // Position des Logos im QR-Code
                    double xPos = (qrCodeImage.Width - (qrCodeImage.Width / fractionOfScale)) / 2;
                    double yPos = (qrCodeImage.Height - (qrCodeImage.Height / fractionOfScale)) / 2;

                    // Logo auf 1/3 des QR-Codes skalieren
                    logo.Mutate(ctx => ctx.Resize(new ResizeOptions
                    {
                        Size = new Size((int)((Convert.ToDouble(qrCodeImage.Width)) / fractionOfScale), (int)((Convert.ToDouble(qrCodeImage.Height) / fractionOfScale))),
                        Mode = ResizeMode.Max
                    }));
                    // Logo in den QR-Code einfügen
                    qrCodeImage.Mutate(ctx => ctx
                        .DrawImage(logo, new Point((int)xPos, (int)yPos), 1f)
                    );
                }


                // Bild speichern
                using (FileStream stream = File.Create(outputPath))
                {
                    qrCodeImage.Save(stream, new PngEncoder());
                }
            }
        }
    }
}
