using System;
using System.IO;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.Net;
using ZXing.Rendering;

class Program
{
    static void Main(string[] args)
    {
        // Text in QR code
        string textToEncode = "https://www.carola-zimmer.com";

        // Load  logo as  SKBitmap (SkiaSharp's equivalent to Bitmap)
        using (SKBitmap logo = SKBitmap.Decode("Logo_final.png"))
        {
            // Create writer
            BarcodeWriter<SKBitmap> barcodeWriter = new BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300, // Adjust the size as needed
                    Height = 300, // Adjust the size as needed
                    Margin = 0, // You can adjust the margin if needed
                    PureBarcode = false // This ensures it's a QR code, not a barcode
                },
                Renderer = new BitmapRenderer(),
            };

            // Generate code
            SKBitmap qrCodeBitmap = barcodeWriter.Write(textToEncode);

            // Overlay the logo on the QR code
            int logoWidth = qrCodeBitmap.Width / 4;
            int logoHeight = qrCodeBitmap.Height / 4;
            int x = (qrCodeBitmap.Width - logoWidth) / 2;
            int y = (qrCodeBitmap.Height - logoHeight) / 2;

            using (SKCanvas canvas = new SKCanvas(qrCodeBitmap))
            {
                canvas.DrawBitmap(logo, new SKRect(x, y, x + logoWidth, y + logoHeight));
            }

            // Save the QR code with the logo as an image
            using (SKImage image = SKImage.FromBitmap(qrCodeBitmap))
            using (SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100))
            using (System.IO.Stream stream = System.IO.File.OpenWrite("qr_code_with_logo.png"))
            {
                encoded.SaveTo(stream);
            }
        }
    }
}
