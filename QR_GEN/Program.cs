using System;
using SkiaSharp;
using SkiaSharp.QrCode;

class Program
{
    static void Main(string[] args)
    {
        var content = "Your QR Code Content Here";
        var level = ECCLevel.H;
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, level);

        // Define the image size and create a surface and canvas
        var width = 512;
        var height = 512;
        var imageInfo = new SKImageInfo(width, height);
        var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;

        // Set the background color (optional)
        canvas.Clear(SKColors.White);

        // Draw the QR code onto the canvas
        var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Color = SKColors.Black;

        var moduleSize = Math.Min(width, height) / qrCodeData.ModuleMatrix.Count;
        for (int i = 0; i < qrCodeData.ModuleMatrix.Count; i++)
        {
            for (int j = 0; j < qrCodeData.ModuleMatrix[i].Count; j++)
            {
                if (qrCodeData.ModuleMatrix[i][j])
                {
                    canvas.DrawRect(j * moduleSize, i * moduleSize, moduleSize, moduleSize, paint);
                }
            }
        }

        // Save the QR code as an image
        var image = surface.Snapshot();
        var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var stream = System.IO.File.OpenWrite("qr_code.png");
        data.SaveTo(stream);
    }
}
