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
    private Entry colorEntryFront;
    private Entry colorEntryBack;
    private CheckButton changeLogoColorCheckbox;
    private CheckButton transparentBackgroundCheckbox;
    private TextView consoleTextView; // TextView zur Anzeige der Konsolenausgaben

    private FileChooserDialog fileChooserEingabe;
    private FileChooserDialog fileChooserAusgabe;




    public HauptFenster() : base(Gtk.WindowType.Toplevel)
    {
        SetDefaultSize(400, 200);


        var mainVBox = new VBox();

        textEntry = new Entry();
        textEntry.PlaceholderText = "QR Text";
        textEntry.Text = "Test";
        mainVBox.PackStart(textEntry, false, false, 10);

        // Button zum Öffnen des Logo-Dateiauswahldialogs
        var openFileButtonEingabe = new Button("Dateipfad zu Logo png auswählen");
        openFileButtonEingabe.Clicked += OnOpenFileButtonClicked;
        mainVBox.PackStart(openFileButtonEingabe, false, false, 10);
        // Dateiauswahldialog initialisieren
        fileChooserEingabe = new FileChooserDialog("Datei auswählen", this, FileChooserAction.Open, "Abbrechen", ResponseType.Cancel, "Öffnen", ResponseType.Accept);
        fileChooserEingabe.Filter = new FileFilter();
        fileChooserEingabe.Filter.AddPattern("*.png");
        fileChooserEingabe.Filter.Name = "PNG-Dateien";
        // Eingabepfad
        logoPathEntry = new Entry();
        logoPathEntry.PlaceholderText = "Logo Path";
        logoPathEntry.Text = "logo4.png";
        mainVBox.PackStart(logoPathEntry, false, false, 10);

        // Button zum Öffnen des Ausgabe-Dateiauswahldialogs
        var openFileButtonAusgabe = new Button("Ausgabedatei auswählen");
        openFileButtonAusgabe.Clicked += OnAusgabeFileButtonClicked;
        mainVBox.PackStart(openFileButtonAusgabe, false, false, 10);
        // Dateiauswahldialog für die Ausgabe initialisieren
        fileChooserAusgabe = new FileChooserDialog("Datei speichern", this, FileChooserAction.Save, "Abbrechen", ResponseType.Cancel, "Speichern", ResponseType.Accept);
        fileChooserAusgabe.Filter = new FileFilter();
        fileChooserAusgabe.Filter.AddPattern("*.png");
        fileChooserAusgabe.Filter.Name = "PNG-Dateien";
        fileChooserAusgabe.DoOverwriteConfirmation = true; // Damit der Dialog bei Überschreiben nachfragt

        outputPathEntry = new Entry();
        outputPathEntry.PlaceholderText = "Output Path";
        outputPathEntry.Text = "o.png";
        mainVBox.PackStart(outputPathEntry, false, false, 10);

        scaleEntry = new Entry();
        scaleEntry.PlaceholderText = "Logo Scale (e.g., 1.5 = 1/1.5)";
        scaleEntry.Text = "3";
        mainVBox.PackStart(scaleEntry, false, false, 10);

        // Dropdown-Menü für ECC-Level erstellen und mit Optionen füllen
        eccLevelComboBox = new ComboBoxText();
        eccLevelComboBox.AppendText("L (Low)");
        eccLevelComboBox.AppendText("M (Medium)");
        eccLevelComboBox.AppendText("Q (Quartile)");
        eccLevelComboBox.AppendText("H (High)");
        mainVBox.PackStart(eccLevelComboBox, false, false, 10);

        // Eingabefeld für die RGB-Farbe Front hinzufügen
        colorEntryFront = new Entry();
        colorEntryFront.PlaceholderText = "RGB Color (e.g., 0,0,0 for Black)"; // Platzhalter
        colorEntryFront.Text = "0,0,0"; // Standardmäßig auf Schwarz setzen
        mainVBox.PackStart(colorEntryFront, false, false, 10);

        // Eingabefeld für die RGB-Farbe Back hinzufügen
        colorEntryBack = new Entry();
        colorEntryBack.PlaceholderText = "RGB Color (e.g., 255,255,255 for White)"; // Platzhalter
        colorEntryBack.Text = "255,255,255"; // Standardmäßig auf weiß setzen
        mainVBox.PackStart(colorEntryBack, false, false, 10);

        // Checkbox Logo
        changeLogoColorCheckbox = new CheckButton("Logo Farbe ändern");
        mainVBox.PackStart(changeLogoColorCheckbox, false, false, 10);

        // Checkbox Hintergrund
        transparentBackgroundCheckbox = new CheckButton("Hintergrund transparent machen");
        mainVBox.PackStart(transparentBackgroundCheckbox, false, false, 10);


        generateButton = new Button("Generate QR Code");
        generateButton.Clicked += OnGenerateButtonClicked;
        mainVBox.PackStart(generateButton, false, false, 10);

        // TextView für die Konsolenausgaben erstellen
        consoleTextView = new TextView();
        consoleTextView.Editable = false; // Das TextView sollte nicht editierbar sein
        mainVBox.PackStart(consoleTextView, true, true, 10);

        Add(mainVBox);
    }
    private void OnGenerateButtonClicked(object sender, EventArgs e)
    {
        string qrText = textEntry.Text;
        string logoPath = logoPathEntry.Text;
        string outputPath = outputPathEntry.Text;
        string selectedEccLevel = eccLevelComboBox.ActiveText;
        ECCLevel eccCode = TranslateEccLevel(selectedEccLevel);
        bool changeLogoColor = changeLogoColorCheckbox.Active;
        bool transparentBackground = transparentBackgroundCheckbox.Active;

        // Leerzeichen hinzufügen und als Trennzeichen verwenden
        string cleanedColorStringFront = colorEntryFront.Text.Trim().Replace(",", ", ");
        string cleanedColorStringBack = colorEntryBack.Text.Trim().Replace(",", ", ");

        if (ColorParser.TryParseRgbColor(cleanedColorStringFront, out Rgba32 qrCodeColorFront))
        {
            if (ColorParser.TryParseRgbColor(cleanedColorStringBack, out Rgba32 qrCodeColorBack))
            {
                if (double.TryParse(scaleEntry.Text, out double scale))
                {
                    Generator.GenerateQRCodeWithLogo(qrText, logoPath, outputPath, scale, eccCode, qrCodeColorFront, qrCodeColorBack, changeLogoColor, transparentBackground);
                    ClearConsole();
                    AppendToConsole("QR Code mit Logo wurde erstellt und gespeichert.");
                }
                else
                {
                    AppendToConsole("Ungültige Skalierung.");
                }
            }
            else
            {
                AppendToConsole("Ungültiger RGB-Farbwert Back. Verwenden Sie das Format 'R, G, B'.");
            }
        }
        else
        {
            AppendToConsole("Ungültiger RGB-Farbwert Front. Verwenden Sie das Format 'R, G, B'.");
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


    // Methode zum Hinzufügen von Text zu TextView
    private void AppendToConsole(string text)
    {
        TextBuffer buffer = consoleTextView.Buffer;
        buffer.Text += text + "\n"; // Zeilenumbruch für neue Ausgaben
        consoleTextView.ScrollToIter(buffer.EndIter, 0, false, 0, 0); // Automatisch scrollen, um die neuesten Ausgaben anzuzeigen
    }

    // Methode zum Leeren des TextViews
    private void ClearConsole()
    {
        TextBuffer buffer = consoleTextView.Buffer;
        buffer.Text = string.Empty;
    }

    private void OnOpenFileButtonClicked(object sender, EventArgs e)
    {
        // Dateiauswahldialog anzeigen und warten, bis der Benutzer eine Datei auswählt
        if (fileChooserEingabe.Run() == (int)ResponseType.Accept)
        {
            string selectedFilePath = fileChooserEingabe.Filename;
            logoPathEntry.Text = selectedFilePath;
        }

        // Dateiauswahldialog schließen
        fileChooserEingabe.Hide();
    }

    private void OnAusgabeFileButtonClicked(object sender, EventArgs e)
{
    // Dateiauswahldialog anzeigen und warten, bis der Benutzer einen Speicherort auswählt
    if (fileChooserAusgabe.Run() == (int)ResponseType.Accept)
    {
        string selectedFilePath = fileChooserAusgabe.Filename;
        outputPathEntry.Text = selectedFilePath;
    }

    // Dateiauswahldialog schließen
    fileChooserAusgabe.Hide();
}

}
