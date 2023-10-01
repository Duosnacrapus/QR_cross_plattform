// using System;
// using System.Drawing;
// using QRCoder;

// class Program
// {
//     static void Main(string[] args)
//     {
//         // Text, you want to encode in the QR code
//         string textToEncode = "Your text here";

//         // Load your logo as a Bitmap
//         using (var logo = new Bitmap("path_to_your_logo.png"))
//         {
//             // Create a QRCodeGenerator
//             QRCodeGenerator qrGenerator = new QRCodeGenerator();
//             QRCodeData qrCodeData = qrGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.Q);

//             // Create a QRCodeRenderer
//             QRCode qrCode = new QRCode(qrCodeData);

//             // Create the QR code with a logo
//             Bitmap qrCodeWithLogo = qrCode.GetGraphic(
//                 20,
//                 Color.Black,
//                 Color.White,
//                 logo,
//                 20
//             );

//             // Save the QR code with the logo as an image
//             qrCodeWithLogo.Save("qr_code_with_logo.png");

//             // Display the QR code on the console (optional)
//             Console.WriteLine("QR code with logo created:");
//             Console.WriteLine(textToEncode);
//         }

//         // Wait for user input so the console doesn't close immediately
//         Console.ReadLine();
//     }
// }
