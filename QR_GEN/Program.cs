using Gtk;

class Program
{
    static void Main(string[] args)
    {
        Application.Init();
        var mainWindow = new HauptFenster();
        mainWindow.DeleteEvent += (o, args) => { Application.Quit(); };
        mainWindow.ShowAll();
        Application.Run();
    }
}
