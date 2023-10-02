using Gtk;
using Pango;
using SixLabors.ImageSharp.PixelFormats;
using static QRCoder.QRCodeGenerator;

public partial class HauptFenster : Gtk.Window
{
    private Entry textEntry;
    private Entry scaleEntry;
    private Button generateButton;
    private ComboBoxText eccLevelComboBox;
    private Entry colorEntryFront;
    private Entry colorEntryBack;
    private CheckButton changeLogoColorCheckbox;
    private CheckButton transparentBackgroundCheckbox;
    private TextView consoleTextView; // TextView zur Anzeige der Konsolenausgaben

    private FileChooserDialog fileChooserEingabe;
    private Label eingabeLabel;
    private FileChooserDialog fileChooserAusgabe;
    private Label ausgabeLabel;




    public HauptFenster() : base(Gtk.WindowType.Toplevel)
    {
        SetDefaultSize(400, 200);


        var mainBox = new Box(Orientation.Vertical, 0);

        var QRLabel = new Label();
        QRLabel.Text = "Was soll der QR-Code darstellen?";
        mainBox.PackStart(QRLabel, false, false, 10);
        textEntry = new Entry();
        textEntry.PlaceholderText = "QR Text";
        mainBox.PackStart(textEntry, false, false, 10);


        var inputBox = new Box(Orientation.Vertical, 0); // Erstelle eine vertikale Box (VBox) für die Widgets und setze den Abstand auf 0
        mainBox.PackStart(inputBox, false, false, 10); // Füge die VBox zur Hauptbox hinzu und setze den Abstand auf 0
        // Button zum Öffnen des Logo-Dateiauswahldialogs
        var openFileButtonEingabe = new Button("Dateipfad zu Logo png auswählen");
        openFileButtonEingabe.Clicked += OnOpenFileButtonClicked;
        inputBox.PackStart(openFileButtonEingabe, false, false, 0); // Füge den Button zur VBox hinzu und setze den Abstand auf 0
        // Dateiauswahldialog initialisieren
        fileChooserEingabe = new FileChooserDialog("Datei auswählen", this, FileChooserAction.Open, "Abbrechen", ResponseType.Cancel, "Öffnen", ResponseType.Accept)
        {
            Filter = new FileFilter()
        };
        fileChooserEingabe.Filter.AddPattern("*.png");
        fileChooserEingabe.Filter.Name = "PNG-Dateien";
        // Eingabepfadlabel
        eingabeLabel = new Label
        {
            Text = "Pfad zum Logo: ",
            Ellipsize = EllipsizeMode.Start, // Falls der Text zu lang wird, wird er am Anfang abgeschnitten
            Selectable = true, // Damit der Text ausgewählt und kopiert werden kann
            Xalign = 0, // Linksbündig ausrichten
            Yalign = 0.5f // Vertikal zentriert ausrichten
        };
        // Fügen Sie das Label-Widget zur Ausgabe-Box hinzu
        inputBox.PackStart(eingabeLabel, false, false, 0);


        var outputBox = new Box(Orientation.Vertical, 0); // Erstelle eine vertikale Box (VBox) für die Widgets und setze den Abstand auf 0
        mainBox.PackStart(outputBox, false, false, 10); // Füge die VBox zur Hauptbox hinzu und setze den Abstand auf 0
        // Button zum Öffnen des Ausgabe-Dateiauswahldialogs
        var openFileButtonAusgabe = new Button("Ausgabedatei auswählen");
        openFileButtonAusgabe.Clicked += OnAusgabeFileButtonClicked;
        outputBox.PackStart(openFileButtonAusgabe, false, false, 0);
        // Dateiauswahldialog für die Ausgabe initialisieren
        fileChooserAusgabe = new FileChooserDialog("Datei speichern", this, FileChooserAction.Save, "Abbrechen", ResponseType.Cancel, "Speichern", ResponseType.Accept);
        fileChooserAusgabe.Filter = new FileFilter();
        fileChooserAusgabe.Filter.AddPattern("*.png");
        fileChooserAusgabe.Filter.Name = "PNG-Dateien";
        fileChooserAusgabe.DoOverwriteConfirmation = true; // Damit der Dialog bei Überschreiben nachfragt
        // Ausgabepfad
        ausgabeLabel = new Label
        {
            Text = "Ausgabepfad: ",
            Ellipsize = EllipsizeMode.Start, // Falls der Text zu lang wird, wird er am Anfang abgeschnitten
            Selectable = true, // Damit der Text ausgewählt und kopiert werden kann
            Xalign = 0, // Linksbündig ausrichten
            Yalign = 0.5f // Vertikal zentriert ausrichten
        };
        outputBox.PackStart(ausgabeLabel, false, false, 0);


        scaleEntry = new Entry
        {
            PlaceholderText = "Logo Scale (e.g., 1.5 = 1/1.5)",
        };
        mainBox.PackStart(scaleEntry, false, false, 10);

        // Dropdown-Menü für ECC-Level erstellen und mit Optionen füllen

        var eccLevelLabel = new Label();
        eccLevelLabel.Text = "Wählen Sie das ECC-Level:";
        mainBox.PackStart(eccLevelLabel, false, false, 10);
        eccLevelComboBox = new ComboBoxText();
        eccLevelComboBox.AppendText("L (Low)");
        eccLevelComboBox.AppendText("M (Medium)");
        eccLevelComboBox.AppendText("Q (Quartile)");
        eccLevelComboBox.AppendText("H (High)");
        mainBox.PackStart(eccLevelComboBox, false, false, 10);

        // Eingabefeld für die RGB-Farbe Front hinzufügen
        colorEntryFront = new Entry();
        colorEntryFront.PlaceholderText = "QR-Farbe in RGB (e.g., 0,0,0 for Black)"; // Platzhalter
        mainBox.PackStart(colorEntryFront, false, false, 10);

        // Eingabefeld für die RGB-Farbe Back hinzufügen
        colorEntryBack = new Entry();
        colorEntryBack.PlaceholderText = "Hintergrund in RGB (e.g., 255,255,255 for White)"; // Platzhalter
        mainBox.PackStart(colorEntryBack, false, false, 10);

        // Checkbox Logo
        changeLogoColorCheckbox = new CheckButton("Logo Farbe ändern");
        mainBox.PackStart(changeLogoColorCheckbox, false, false, 10);

        // Checkbox Hintergrund
        transparentBackgroundCheckbox = new CheckButton("Hintergrund transparent machen");
        mainBox.PackStart(transparentBackgroundCheckbox, false, false, 10);


        generateButton = new Button("Generate QR Code");
        generateButton.Clicked += OnGenerateButtonClicked;
        mainBox.PackStart(generateButton, false, false, 10);

        // TextView für die Konsolenausgaben erstellen
        consoleTextView = new TextView();
        consoleTextView.Editable = false; // Das TextView sollte nicht editierbar sein
        mainBox.PackStart(consoleTextView, true, true, 10);

        Add(mainBox);
    }


    private void OnGenerateButtonClicked(object? sender, EventArgs e)
    {
        string qrText = textEntry.Text;
        string logoPath = eingabeLabel.Text;
        string outputPath = ausgabeLabel.Text;
        string selectedEccLevel = eccLevelComboBox.ActiveText;
        ECCLevel eccCode = TranslateEccLevel(selectedEccLevel);
        bool changeLogoColor = changeLogoColorCheckbox.Active;
        bool transparentBackground = transparentBackgroundCheckbox.Active;

        // Leerzeichen hinzufügen und als Trennzeichen verwenden
        string cleanedColorStringFront = colorEntryFront.Text.Trim().Replace(",", ", ");
        string cleanedColorStringBack = colorEntryBack.Text.Trim().Replace(",", ", ");

        if (ColorParser.TryParseRgbColor(cleanedColorStringFront, out Rgba32 qrCodeColorFront))
        {   
            int[] rgbfront = ParseRgbColor(cleanedColorStringFront);
            if (ColorParser.TryParseRgbColor(cleanedColorStringBack, out Rgba32 qrCodeColorBack))
            {
                int[] rgbback = ParseRgbColor(cleanedColorStringBack);
                if (double.TryParse(scaleEntry.Text, out double scale))
                {
                    Generator.GenerateQRCodeWithLogo(qrText, logoPath, outputPath, scale, eccCode, rgbfront, rgbback, changeLogoColor, transparentBackground);
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

    private void OnOpenFileButtonClicked(object? sender, EventArgs e)
    {
        // Dateiauswahldialog anzeigen und warten, bis der Benutzer eine Datei auswählt
        if (fileChooserEingabe.Run() == (int)ResponseType.Accept)
        {
            string selectedFilePath = fileChooserEingabe.Filename;
            // Setzen Sie den Ausgabepfad im Label
            eingabeLabel.Text = selectedFilePath;
        }
        // Dateiauswahldialog schließen
        fileChooserEingabe.Hide();
    }

    private void OnAusgabeFileButtonClicked(object? sender, EventArgs e)
    {
        // Dateiauswahldialog anzeigen und warten, bis der Benutzer einen Speicherort auswählt
        if (fileChooserAusgabe.Run() == (int)ResponseType.Accept)
        {
            string selectedFilePath = fileChooserAusgabe.Filename;
            // Überprüfen, ob die Erweiterung ".png" bereits vorhanden ist
            if (!selectedFilePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                // Wenn nicht, fügen Sie ".png" am Ende hinzu
                selectedFilePath += ".png";
            }
            ausgabeLabel.Text = selectedFilePath;
        }
        // Dateiauswahldialog schließen
        fileChooserAusgabe.Hide();
    }


    private int[] ParseRgbColor(string colorString)
    {
        // Zerlegen Sie die Eingabe anhand von Kommas und Leerzeichen und parsen Sie die Teile in Integer-Werte.
        string[] components = colorString.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        int[] rgbValues = new int[components.Length];

        for (int i = 0; i < components.Length; i++)
        {
            if (int.TryParse(components[i], out int value))
            {
                rgbValues[i] = value;
            }           
        }

        return rgbValues;
    }

}
