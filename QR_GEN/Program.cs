using Gtk;


class Program
{
    static void Main(string[] args)
    {
        // Generator.GenerateQRCodeWithLogo("QRTEXT", "PFAD/LOGO.png", "PFAD/qr_code_with_logo.png", 7/2); //Int ist skalierung

        Application.Init();
        var mainWindow = new HauptFenster();
        mainWindow.DeleteEvent += (o, args) => { Application.Quit(); };
        mainWindow.ShowAll();
        Application.Run();
    }


   
}
