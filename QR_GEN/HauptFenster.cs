using Gtk;
using QRCoder;
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static QRCoder.QRCodeGenerator;
using SixLabors.ImageSharp;

public partial class HauptFenster : Gtk.Window
{
    private Entry textEntry;
    private Entry logoPathEntry;
    private Entry outputPathEntry;
    private Entry scaleEntry;
    private Button generateButton;
    private ComboBoxText eccLevelComboBox;
    private Entry colorEntry;

    public HauptFenster() : base(Gtk.WindowType.Toplevel)
    {
        SetDefaultSize(400, 200);


        var mainVBox = new VBox();

        textEntry = new Entry();
        textEntry.PlaceholderText = "QR Text";
        textEntry.Text = "Test";
        mainVBox.PackStart(textEntry, false, false, 10);

        logoPathEntry = new Entry();
        logoPathEntry.PlaceholderText = "Logo Path";
        logoPathEntry.Text ="logo4.png";
        mainVBox.PackStart(logoPathEntry, false, false, 10);

        outputPathEntry = new Entry();
        outputPathEntry.PlaceholderText = "Output Path";
        outputPathEntry.Text="o.png";
        mainVBox.PackStart(outputPathEntry, false, false, 10);

        scaleEntry = new Entry();
        scaleEntry.PlaceholderText = "Logo Scale (e.g., 1.5 = 1/1.5)";
        scaleEntry.Text="3";
        mainVBox.PackStart(scaleEntry, false, false, 10);

        // Dropdown-Menü für ECC-Level erstellen und mit Optionen füllen
        eccLevelComboBox = new ComboBoxText();
        eccLevelComboBox.AppendText("L (Low)");
        eccLevelComboBox.AppendText("M (Medium)");
        eccLevelComboBox.AppendText("Q (Quartile)");
        eccLevelComboBox.AppendText("H (High)");
        mainVBox.PackStart(eccLevelComboBox, false, false, 10);

        // Eingabefeld für die RGB-Farbe hinzufügen
        colorEntry = new Entry();
        colorEntry.PlaceholderText = "RGB Color (e.g., 255,0,0 for Red)"; // Platzhalter
        colorEntry.Text = "0,0,0"; // Standardmäßig auf Schwarz setzen
        mainVBox.PackStart(colorEntry, false, false, 10);

        generateButton = new Button("Generate QR Code");
        generateButton.Clicked += OnGenerateButtonClicked;
        mainVBox.PackStart(generateButton, false, false, 10);

        Add(mainVBox);
    }
private void OnGenerateButtonClicked(object sender, EventArgs e)
{
    string qrText = textEntry.Text;
    string logoPath = logoPathEntry.Text;
    string outputPath = outputPathEntry.Text;
    string selectedEccLevel = eccLevelComboBox.ActiveText;
    ECCLevel eccCode = TranslateEccLevel(selectedEccLevel);

    // Leerzeichen hinzufügen und als Trennzeichen verwenden
    string cleanedColorString = colorEntry.Text.Trim().Replace(",", ", ");

    if (ColorParser.TryParseRgbColor(cleanedColorString, out Rgba32 qrCodeColor))
    {
        if (double.TryParse(scaleEntry.Text, out double scale))
        {
            Generator.GenerateQRCodeWithLogo(qrText, logoPath, outputPath, scale, eccCode, qrCodeColor);
            Console.WriteLine("QR Code mit Logo wurde erstellt und gespeichert.");
        }
        else
        {
            Console.WriteLine("Ungültige Skalierung.");
        }
    }
    else
    {
        Console.WriteLine("Ungültiger RGB-Farbwert. Verwenden Sie das Format 'R, G, B'.");
    }
}

    private ECCLevel TranslateEccLevel(string selectedEccLevel)
    {
        // Übersetzen Sie das ausgewählte ECC-Level in den entsprechenden QRCoder.ECCLevel
        switch (selectedEccLevel)
        {
            case "L (Low)":
                return ECCLevel.L;
            case "M (Medium)":
                return ECCLevel.M;
            case "Q (Quartile)":
                return ECCLevel.Q;
            case "H (High)":
                return ECCLevel.H;
            default:
                return ECCLevel.M; // Standardwert, wenn keine Übereinstimmung gefunden wird
        }
    }
}
