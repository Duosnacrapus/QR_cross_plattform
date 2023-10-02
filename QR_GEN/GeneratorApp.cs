using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.IO;

public class MainWindow : Window
{
    private TextBox textToEncodeBox;
    private TextBox logoPathBox;
    private TextBox outputPathBox;
    private TextBox scaleFractionBox;
    private Button generateButton;

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        InitializeUI();
    }

    private void InitializeUI()
    {
        textToEncodeBox = this.FindControl<TextBox>("TextToEncode");
        logoPathBox = this.FindControl<TextBox>("LogoPath");
        outputPathBox = this.FindControl<TextBox>("OutputPath");
        scaleFractionBox = this.FindControl<TextBox>("ScaleFraction");
        generateButton = this.FindControl<Button>("GenerateButton");

        generateButton.Click += OnGenerateButtonClick;
    }

    private void OnGenerateButtonClick(object sender, RoutedEventArgs e)
    {
        string textToEncode = textToEncodeBox.Text;
        string logoPath = logoPathBox.Text;
        string outputPath = outputPathBox.Text;
        int fractionOfScale = int.Parse(scaleFractionBox.Text);

        Generator.GenerateQRCodeWithLogo(textToEncode, logoPath, outputPath, fractionOfScale);

        // Zeigen Sie eine Erfolgsmeldung an oder aktualisieren Sie die UI entsprechend
    }
}
